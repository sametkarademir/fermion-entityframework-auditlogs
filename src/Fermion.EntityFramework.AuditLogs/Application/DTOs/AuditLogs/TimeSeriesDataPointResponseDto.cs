namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class TimeSeriesDataPointResponseDto
{
    public DateTime TimeLabel { get; set; }
    public int Value { get; set; }
}