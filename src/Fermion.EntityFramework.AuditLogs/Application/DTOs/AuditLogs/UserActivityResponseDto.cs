using Fermion.EntityFramework.AuditLogs.Domain.Enums;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class UserActivityResponseDto
{
    public Guid UserId { get; set; }
    public int ChangeCount { get; set; }
    public DateTime LastActivityTime { get; set; }
    public Dictionary<string, int> MostModifiedEntityTypes { get; set; } = new();
    public Dictionary<States, int> MostCommonOperations { get; set; } = new();
}