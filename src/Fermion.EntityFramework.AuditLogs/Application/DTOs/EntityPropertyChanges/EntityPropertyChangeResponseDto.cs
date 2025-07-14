using Fermion.Domain.Shared.DTOs;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.EntityPropertyChanges;

public class EntityPropertyChangeResponseDto : CreationAuditedEntityDto<Guid>
{
    public string PropertyName { get; set; } = null!;
    public string PropertyTypeFullName { get; set; } = null!;
    public string? NewValue { get; set; }
    public string? OriginalValue { get; set; }

    public Guid AuditLogId { get; set; }
}