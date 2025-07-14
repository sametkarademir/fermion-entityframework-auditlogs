using Fermion.EntityFramework.AuditLogs.Domain.Enums;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class ModifiedEntityResponseDto
{
    public string EntityName { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public int ChangeCount { get; set; }
    public DateTime LastModified { get; set; }
    public int UniqueUserCount { get; set; }
    public Dictionary<States, int> StateDistribution { get; set; } = new();
}