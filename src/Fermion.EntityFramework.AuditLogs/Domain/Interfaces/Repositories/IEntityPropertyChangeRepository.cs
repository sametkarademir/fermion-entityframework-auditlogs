using Fermion.EntityFramework.AuditLogs.Domain.Entities;
using Fermion.EntityFramework.Shared.Interfaces;

namespace Fermion.EntityFramework.AuditLogs.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing entity property changes.
/// </summary>
public interface IEntityPropertyChangeRepository : IRepository<EntityPropertyChange, Guid>
{
}