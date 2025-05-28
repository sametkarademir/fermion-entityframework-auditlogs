using Fermion.EntityFramework.AuditLogs.Core.Entities;
using Fermion.EntityFramework.Shared.Repositories.Abstractions;

namespace Fermion.EntityFramework.AuditLogs.Core.Interfaces;

/// <summary>
/// Repository interface for managing entity property changes.
/// </summary>
public interface IEntityPropertyChangeRepository : IRepository<EntityPropertyChange, Guid>
{
}