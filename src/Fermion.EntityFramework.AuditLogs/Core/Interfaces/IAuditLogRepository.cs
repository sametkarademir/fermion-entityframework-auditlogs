using Fermion.EntityFramework.AuditLogs.Core.Entities;
using Fermion.EntityFramework.Shared.Repositories.Abstractions;

namespace Fermion.EntityFramework.AuditLogs.Core.Interfaces;

/// <summary>
/// Repository interface for managing audit logs.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog, Guid>
{
}