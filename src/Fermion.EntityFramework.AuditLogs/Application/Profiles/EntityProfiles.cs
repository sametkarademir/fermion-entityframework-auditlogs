using AutoMapper;
using Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;
using Fermion.EntityFramework.AuditLogs.Application.DTOs.EntityPropertyChanges;
using Fermion.EntityFramework.AuditLogs.Domain.Entities;

namespace Fermion.EntityFramework.AuditLogs.Application.Profiles;

/// <summary>
/// AutoMapper profile for mapping between entity and DTO objects.
/// </summary>
public class EntityProfiles : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityProfiles"/> class.
    /// Configures the mapping profiles for audit log related entities.
    /// </summary>
    public EntityProfiles()
    {
        // Map AuditLog entity to AuditLogResponseDto
        CreateMap<AuditLog, AuditLogResponseDto>();

        // Map EntityPropertyChange entity to EntityPropertyChangeResponseDto
        CreateMap<EntityPropertyChange, EntityPropertyChangeResponseDto>();
    }
}