using Fermion.EntityFramework.AuditLogs.Core.Entities;
using Fermion.EntityFramework.Shared.Interfaces;

namespace Fermion.EntityFramework.AuditLogs.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing entity property changes.
/// </summary>
public interface IEntityPropertyChangeRepository : IRepository<EntityPropertyChange, Guid>
{
}