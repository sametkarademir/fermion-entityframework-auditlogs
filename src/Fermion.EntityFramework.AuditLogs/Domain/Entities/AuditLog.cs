using Fermion.Domain.Shared.Auditing;
using Fermion.Domain.Shared.Filters;
using Fermion.Domain.Shared.Interfaces;
using Fermion.EntityFramework.AuditLogs.Domain.Enums;

namespace Fermion.EntityFramework.AuditLogs.Domain.Entities;

/// <summary>
/// Represents an audit log entry that tracks changes to entities in the system.
/// </summary>
[ExcludeFromProcessing]
public class AuditLog : CreationAuditedEntity<Guid>, IEntitySnapshotId, IEntitySessionId, IEntityCorrelationId
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity being audited.
    /// </summary>
    public string EntityId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the entity type being audited.
    /// </summary>
    public string EntityName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the state of the entity change (Added, Modified, Deleted).
    /// </summary>
    public States State { get; set; }

    /// <summary>
    /// Gets or sets the snapshot identifier associated with this audit log.
    /// </summary>
    public Guid? SnapshotId { get; set; }

    /// <summary>
    /// Gets or sets the session identifier associated with this audit log.
    /// </summary>
    public Guid? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier associated with this audit log.
    /// </summary>
    public Guid? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the collection of property changes associated with this audit log.
    /// </summary>
    public ICollection<EntityPropertyChange> EntityPropertyChanges { get; set; } = new List<EntityPropertyChange>();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLog"/> class.
    /// </summary>
    public AuditLog()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLog"/> class with specified parameters.
    /// </summary>
    /// <param name="id">The unique identifier for the audit log.</param>
    /// <param name="entityId">The unique identifier of the entity being audited.</param>
    /// <param name="entityName">The name of the entity type being audited.</param>
    /// <param name="state">The state of the entity change.</param>
    public AuditLog(Guid id, string entityId, string entityName, States state) : base(id)
    {
        EntityId = entityId;
        EntityName = entityName;
        State = state;
    }
}