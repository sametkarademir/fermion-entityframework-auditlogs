using Fermion.Domain.Shared.Auditing;
using Fermion.Domain.Shared.Filters;

namespace Fermion.EntityFramework.AuditLogs.Domain.Entities;

/// <summary>
/// Represents a change to a property of an entity that is being audited.
/// </summary>
[ExcludeFromProcessing]
public class EntityPropertyChange : CreationAuditedEntity<Guid>
{
    /// <summary>
    /// Gets or sets the name of the property that was changed.
    /// </summary>
    public string PropertyName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the full name of the property type that was changed.
    /// </summary>
    public string PropertyTypeFullName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the new value of the property after the change.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the original value of the property before the change.
    /// </summary>
    public string? OriginalValue { get; set; }

    /// <summary>
    /// Gets or sets the ID of the associated audit log entry.
    /// </summary>
    public Guid AuditLogId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the associated audit log entry.
    /// </summary>
    public AuditLog? AuditLog { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityPropertyChange"/> class.
    /// </summary>
    public EntityPropertyChange()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityPropertyChange"/> class with specified parameters.
    /// </summary>
    /// <param name="id">The unique identifier for the property change.</param>
    /// <param name="auditLogId">The ID of the associated audit log entry.</param>
    /// <param name="propertyName">The name of the property that was changed.</param>
    /// <param name="propertyTypeFullName">The full name of the property type that was changed.</param>
    /// <param name="newValue">The new value of the property after the change.</param>
    /// <param name="originalValue">The original value of the property before the change.</param>
    public EntityPropertyChange(Guid id, Guid auditLogId, string propertyName, string propertyTypeFullName, string? newValue, string? originalValue) : base(id)
    {
        AuditLogId = auditLogId;
        PropertyName = propertyName;
        PropertyTypeFullName = propertyTypeFullName;
        NewValue = newValue;
        OriginalValue = originalValue;
    }
}