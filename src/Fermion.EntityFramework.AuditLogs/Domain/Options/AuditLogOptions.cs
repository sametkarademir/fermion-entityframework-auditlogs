using Fermion.Domain.Shared.Conventions;
using Fermion.EntityFramework.AuditLogs.Domain.Enums;

namespace Fermion.EntityFramework.AuditLogs.Domain.Options;

/// <summary>
/// Options for configuring audit logging behavior.
/// </summary>
public class AuditLogOptions
{
    /// <summary>
    /// Specifies whether audit logging is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Character string used for masking sensitive data
    /// </summary>
    /// <remarks>
    /// This string will replace the sensitive data in the audit logs.
    /// For example, if the sensitive data is "123456", and the MaskPattern is "***MASKED***",
    /// the log will show "***MASKED***" instead of "123456".
    /// </remarks>
    public string MaskPattern { get; set; } = "***MASKED***";

    /// <summary>
    /// List of sensitive property names that will be masked when saving audit logs
    /// </summary>
    /// <remarks>
    /// This list contains property names that are considered sensitive and should be masked in the audit logs.
    /// For example, if the property name is "Password", and the MaskPattern is "***MASKED***",
    /// the log will show "***MASKED***" instead of the actual password value.
    /// </remarks>
    public HashSet<string> SensitiveProperties { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Password", "Token", "Secret", "ApiKey", "Key", "Credential", "Ssn", "Credit", "Card",
        "SecurityCode", "Pin", "Authorization"
    };

    /// <summary>
    /// Collection that specifies which properties should be completely excluded from audit logging based on entity types
    /// </summary>
    /// <remarks>
    /// This dictionary contains entity types as keys and a set of property names as values.
    /// The properties in the set will not be logged for the corresponding entity type.
    /// For example, if the entity type is "User" and the property name is "Password",
    /// the password will not be logged for the User entity.
    /// </remarks>
    public Dictionary<Type, HashSet<string>> ExcludedPropertiesByEntityType { get; set; } =
        new Dictionary<Type, HashSet<string>>();

    /// <summary>
    /// Entity types to be audit logged. If empty, all entities will be logged.
    /// </summary>
    /// <remarks>
    /// This collection contains entity types that should be included in the audit logs.
    /// If this collection is empty, all entities will be logged.
    /// For example, if the entity type is "User", and the collection contains "User",
    /// the User entity will be logged.
    /// </remarks>
    public HashSet<Type> IncludedEntityTypes { get; set; } = new HashSet<Type>();

    /// <summary>
    /// Entity types to be excluded from audit logging
    /// </summary>
    /// <remarks>
    /// This collection contains entity types that should be excluded from the audit logs.
    /// For example, if the entity type is "User", and the collection contains "User",
    /// the User entity will not be logged.
    /// </remarks>
    public HashSet<Type> ExcludedEntityTypes { get; set; } = new HashSet<Type>();

    /// <summary>
    /// Specifies whether change details should be logged.
    /// If set to false, only the information that a change occurred will be logged, without old and new values.
    /// </summary>
    /// <remarks>
    /// This property determines whether the details of the changes made to the entities should be logged.
    /// If this property is set to true, the old and new values of the changed properties will be logged.
    /// If set to false, only the information that a change occurred will be logged, without the actual values.
    /// </remarks>
    public bool LogChangeDetails { get; set; } = true;

    /// <summary>
    /// Maximum length of field values to be logged.
    /// Some fields can be very long (e.g., binary data or large text fields)
    /// </summary>
    /// <remarks>
    /// This property specifies the maximum length of the field values that will be logged.
    /// If a field value exceeds this length, it will be truncated in the logs.
    /// For example, if the maximum length is set to 5000,
    /// and the field value is "1234567890... (10000 characters)",
    /// the logged value will be "1234567890... (5000 characters)".
    /// </remarks>
    public int MaxValueLength { get; set; } = 5000;

    /// <summary>
    /// Specifies which types of changes (Added, Modified, Deleted) should be logged.
    /// </summary>
    /// <remarks>
    /// This property specifies the types of changes that should be logged.
    /// The default value is States.Added | States.Modified | States.Deleted,
    /// which means that all types of changes will be logged.
    /// If you want to log only specific types of changes,
    /// you can set this property to the desired value.
    /// For example, if you want to log only added and modified changes,
    /// you can set this property to States.Added | States.Modified.
    /// </remarks>
    public List<States> LoggedStates { get; set; } = [
        States.Added,
        States.Modified,
        States.Deleted
    ];

    /// <summary>
    /// Determines whether entities with a specific change state should be logged
    /// </summary>
    /// <param name="state">The change state to check</param>
    /// <returns>True: Should be logged, False: Should not be logged</returns>
    public bool ShouldLogState(States state)
    {
        return LoggedStates.Contains(state);
    }

    /// <summary>
    /// Determines whether an entity should be logged
    /// </summary>
    /// <param name="entityType">The entity type</param>
    /// <returns>True: Should be logged, False: Should not be logged</returns>
    public bool ShouldLogEntity(Type entityType)
    {
        // If present in ExcludedEntityTypes, should not be logged
        if (ExcludedEntityTypes.Contains(entityType))
        {
            return false;
        }

        // If IncludedEntityTypes is empty, all entities are logged, otherwise only the specified ones are logged
        return IncludedEntityTypes.Count == 0 || IncludedEntityTypes.Contains(entityType);
    }

    /// <summary>
    /// Determines whether a property of a given entity type should be logged
    /// </summary>
    /// <param name="entityType">The entity type</param>
    /// <param name="propertyName">The property name</param>
    /// <returns>True: Should be logged, False: Should not be logged</returns>
    public bool ShouldLogProperty(Type entityType, string propertyName)
    {
        if (ExcludedPropertiesByEntityType.TryGetValue(entityType, out var excludedProperties))
        {
            return !excludedProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
        }

        return true;
    }

    /// <summary>
    /// Determines whether a property contains sensitive data
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <returns>True: Contains sensitive data, False: Does not contain sensitive data</returns>
    public bool IsSensitiveProperty(string propertyName)
    {
        return SensitiveProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
    }

    public AuditLogControllerOptions AuditLogController { get; set; } = new();
}

public class AuditLogControllerOptions
{
    /// <summary>
    /// If true, the AuditLogController will be enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Route for the AuditLogController
    /// </summary>
    public string Route { get; set; } = "api/audit-logs";

    /// <summary>
    /// Authorization settings for AuditLog Controller
    /// </summary>
    public AuthorizationOptions GlobalAuthorization { get; set; } = new()
    {
        RequireAuthentication = true,
        Policy = null,
        Roles = null
    };

    /// <summary>
    /// Endpoint-specific authorization settings for AuditLog Controller
    /// </summary>
    public List<EndpointOptions>? Endpoints { get; set; }
}