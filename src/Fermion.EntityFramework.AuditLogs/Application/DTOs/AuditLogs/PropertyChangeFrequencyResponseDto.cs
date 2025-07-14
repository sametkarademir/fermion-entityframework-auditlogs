namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class PropertyChangeFrequencyResponseDto
{
    public string PropertyName { get; set; } = null!;
    public string PropertyType { get; set; } = null!;
    public int ChangeCount { get; set; }
}