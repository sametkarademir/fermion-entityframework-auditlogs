using Fermion.Domain.Shared.Filters;
using Fermion.EntityFramework.AuditLogs.Domain.Entities;
using Fermion.EntityFramework.AuditLogs.Domain.Enums;
using Fermion.EntityFramework.AuditLogs.Domain.Options;
using Fermion.Domain.Extensions.Claims;
using Fermion.Domain.Extensions.HttpContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.AuditLogs.Domain.Extensions;

/// <summary>
/// Extension methods for adding audit logging functionality to Entity Framework Domain DbContext.
/// </summary>
public static class DbContextAuditLogExtensions
{
    /// <summary>
    /// Asynchronously processes and logs changes made to entities in the DbContext.
    /// </summary>
    /// <param name="context">The DbContext instance to process.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for accessing current request information.</param>
    /// <param name="auditLogOptions">The audit log configuration options.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method:
    /// 1. Checks if audit logging is enabled
    /// 2. Processes each changed entity in the context
    /// 3. Creates audit log entries for eligible changes
    /// 4. Records property-level changes
    /// 5. Handles sensitive data masking
    /// 6. Applies value length restrictions
    /// </remarks>
    public static async Task SetAuditLogAsync(this DbContext context, IHttpContextAccessor httpContextAccessor, AuditLogOptions auditLogOptions)
    {
        // Skip processing if audit logging is disabled
        if (!auditLogOptions.Enabled)
        {
            return;
        }

        // Get all entities that have changes in the current context
        var auditEntries = context.ChangeTracker.Entries().ToList();
        foreach (var entry in auditEntries)
        {
            var entityType = entry.Entity.GetType();

            // Check if the entity should be audited based on configuration and attributes
            var hasAttribute = entityType.GetCustomAttributes(typeof(ExcludeFromProcessingAttribute), true).Any();
            var shouldLogEntity = auditLogOptions.ShouldLogEntity(entityType);
            var currentState = (States)entry.State;
            var shouldLogState = auditLogOptions.ShouldLogState(currentState);

            // Skip if entity is excluded from processing or doesn't meet logging criteria
            if (hasAttribute || !shouldLogEntity || !shouldLogState)
            {
                continue;
            }

            // Create a new audit log entry with basic information
            var auditLog = new AuditLog
            {
                // Set correlation, snapshot, and session IDs from the current HTTP context
                CorrelationId = httpContextAccessor.HttpContext?.GetCorrelationId(),
                SnapshotId = httpContextAccessor.HttpContext?.GetSnapshotId(),
                SessionId = httpContextAccessor.HttpContext?.GetSessionId(),
                // Get entity identifier and name
                EntityId = entry.OriginalValues[entry.Metadata.FindPrimaryKey()!.Properties.First().Name]!.ToString() ?? "Unknown",
                EntityName = entry.Metadata.GetTableName() ?? "Unknown",
                State = (States)entry.State,
                CreationTime = DateTime.UtcNow,
                CreatorId = httpContextAccessor.HttpContext?.User.GetUserIdToGuid(),
            };

            // Process each property of the entity
            foreach (var property in entry.Properties)
            {
                var propertyName = property.Metadata.Name;
                var propertyInfo = entry.Entity.GetType().GetProperty(propertyName);

                // Skip properties that are excluded from processing or don't meet logging criteria
                if (propertyInfo != null &&
                    (propertyInfo.GetCustomAttributes(typeof(ExcludeFromProcessingAttribute), true).Any() ||
                     !auditLogOptions.ShouldLogProperty(entityType, propertyName)))
                {
                    continue;
                }

                bool ShouldLogChange(string? oldValue, string? newValue, States state)
                {
                    // Skip if property is marked with ExcludeFromProcessing attribute
                    if (property.GetType().GetCustomAttributes(typeof(ExcludeFromProcessingAttribute), true).Any())
                    {
                        return false;
                    }

                    // Log if entity is new or values have changed
                    return state == States.Added || !Equals(oldValue, newValue);
                }

                // Get the old and new values of the property
                var oldValue = property.OriginalValue?.ToString();
                var newValue = property.CurrentValue?.ToString();

                if (ShouldLogChange(oldValue, newValue, auditLog.State))
                {
                    // Handle sensitive data masking
                    if (auditLogOptions.IsSensitiveProperty(propertyName))
                    {
                        oldValue = auditLog.State == States.Added ? null : auditLogOptions.MaskPattern;
                        newValue = auditLogOptions.MaskPattern;
                    }

                    // Truncate values that exceed the maximum length
                    if (oldValue != null && oldValue.Length > auditLogOptions.MaxValueLength)
                    {
                        oldValue = oldValue.Substring(0, auditLogOptions.MaxValueLength) + "... (truncated)";
                    }

                    if (newValue != null && newValue.Length > auditLogOptions.MaxValueLength)
                    {
                        newValue = newValue.Substring(0, auditLogOptions.MaxValueLength) + "... (truncated)";
                    }

                    // Create a new property change record
                    var entityPropertyChange = new EntityPropertyChange
                    {
                        // Only store values if detailed logging is enabled
                        NewValue = auditLogOptions.LogChangeDetails ? newValue : null,
                        OriginalValue = auditLogOptions.LogChangeDetails ? (States.Added == auditLog.State ? null : oldValue) : null,
                        PropertyName = propertyName,
                        PropertyTypeFullName = property.Metadata.ClrType.FullName ?? string.Empty,
                        AuditLogId = auditLog.Id,
                        CreationTime = DateTime.UtcNow,
                        CreatorId = httpContextAccessor.HttpContext?.User.GetUserIdToGuid(),
                    };

                    // Add the property change to the audit log
                    auditLog.EntityPropertyChanges.Add(entityPropertyChange);
                }
            }

            // Save the audit log entry to the database
            await context.Set<AuditLog>().AddAsync(auditLog);
        }
    }
}