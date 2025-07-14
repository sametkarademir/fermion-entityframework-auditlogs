using Fermion.EntityFramework.AuditLogs.Domain.Entities;
using Fermion.EntityFramework.Shared.Interfaces;

namespace Fermion.EntityFramework.AuditLogs.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing audit logs.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog, Guid>
{
}