using Fermion.EntityFramework.AuditLogs.Core.Entities;
using Fermion.EntityFramework.Shared.Interfaces;

namespace Fermion.EntityFramework.AuditLogs.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing audit logs.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog, Guid>
{
}