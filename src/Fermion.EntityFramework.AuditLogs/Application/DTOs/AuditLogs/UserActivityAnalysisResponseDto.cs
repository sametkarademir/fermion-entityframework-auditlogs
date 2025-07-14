namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class UserActivityAnalysisResponseDto
{
    public int TotalActiveUsers { get; set; }
    public int TotalChangeCount { get; set; }
    public List<UserActivityResponseDto> MostActiveUsers { get; set; } = [];
    public double AverageChangesPerUser { get; set; }
    public Dictionary<string, int> ActivityDistribution { get; set; } = new();
}