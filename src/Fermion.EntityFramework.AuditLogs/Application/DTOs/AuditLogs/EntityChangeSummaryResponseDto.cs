namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class EntityChangeSummaryResponseDto
{
    public string EntityId { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public DateTime? CreationDate { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationDate { get; set; }
    public Guid? LastModifierId { get; set; }
    public int TotalChanges { get; set; }
    public List<PropertyChangeFrequencyResponseDto> MostFrequentlyModifiedProperties { get; set; } = [];
}