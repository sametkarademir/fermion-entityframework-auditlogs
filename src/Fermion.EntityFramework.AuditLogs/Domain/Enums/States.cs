namespace Fermion.EntityFramework.AuditLogs.Domain.Enums;

/// <summary>
/// Represents the possible states of an entity in the Entity Framework change tracker.
/// </summary>
[Flags]
public enum States
{
    /// <summary>
    /// The entity is not being tracked by the context.
    /// </summary>
    Detached = 0,

    /// <summary>
    /// The entity is being tracked by the context and exists in the database, and its property values have not changed from the values in the database.
    /// </summary>
    Unchanged = 1,

    /// <summary>
    /// The entity is being tracked by the context and exists in the database, but has been marked for deletion from the database.
    /// </summary>
    Deleted = 2,

    /// <summary>
    /// The entity is being tracked by the context and exists in the database, and some or all of its property values have been modified.
    /// </summary>
    Modified = 3,

    /// <summary>
    /// The entity is being tracked by the context but does not yet exist in the database.
    /// </summary>
    Added = 4
}