using Fermion.EntityFramework.AuditLogs.Domain.Enums;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class UserChangeBehaviorResponseDto
{
    public Guid UserId { get; set; }
    public DateRangeResponseDto AnalysisPeriod { get; set; } = null!;
    public int TotalChanges { get; set; }
    public Dictionary<int, int> ChangesByHourOfDay { get; set; } = new();
    public Dictionary<DayOfWeek, int> ChangesByDayOfWeek { get; set; } = new();
    public Dictionary<string, int> ChangesByEntityType { get; set; } = new();
    public Dictionary<States, int> ChangesByState { get; set; } = new();
    public List<ModifiedEntityResponseDto> MostModifiedEntities { get; set; } = [];
    public List<PropertyChangeFrequencyResponseDto> MostChangedProperties { get; set; } = [];
}