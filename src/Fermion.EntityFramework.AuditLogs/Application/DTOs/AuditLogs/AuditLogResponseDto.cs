using System.Text.Json.Serialization;
using Fermion.Domain.Shared.DTOs;
using Fermion.EntityFramework.AuditLogs.Application.DTOs.EntityPropertyChanges;
using Fermion.EntityFramework.AuditLogs.Domain.Enums;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class AuditLogResponseDto : CreationAuditedEntityDto<Guid>
{
    public string EntityId { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public States State { get; set; }

    public Guid? SnapshotId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? CorrelationId { get; set; }

    public List<EntityPropertyChangeResponseDto> EntityPropertyChanges { get; set; } = [];
}