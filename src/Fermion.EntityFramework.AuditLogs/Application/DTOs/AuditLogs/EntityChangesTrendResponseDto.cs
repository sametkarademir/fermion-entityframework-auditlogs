using Fermion.EntityFramework.AuditLogs.Domain.Enums;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class EntityChangesTrendResponseDto
{
    public string EntityName { get; set; } = null!;
    public DateRangeResponseDto AnalysisPeriod { get; set; } = null!;
    public List<TimeSeriesDataPointResponseDto> ChangesByTimeInterval { get; set; } = [];
    public Dictionary<string, List<TimeSeriesDataPointResponseDto>> ChangesByProperty { get; set; } = new();
    public Dictionary<States, List<TimeSeriesDataPointResponseDto>> ChangesByState { get; set; } = new();
}