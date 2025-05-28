using Fermion.Domain.Shared.DTOs;

namespace WebApi.DTOs.Categories;

public class CategoryResponseDto : FullAuditedEntityDto<Guid>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}