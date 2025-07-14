namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class MostModifiedEntitiesResponseDto
{
    public DateRangeResponseDto AnalysisPeriod { get; set; } = null!;
    public List<ModifiedEntityResponseDto> MostModifiedEntities { get; set; } = new();
    public Dictionary<string, int> ChangesByEntityType { get; set; } = new();
}