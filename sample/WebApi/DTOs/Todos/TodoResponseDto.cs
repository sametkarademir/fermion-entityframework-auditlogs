using Fermion.Domain.Shared.DTOs;
using WebApi.DTOs.Categories;

namespace WebApi.DTOs.Todos;

public class TodoResponseDto : FullAuditedEntityDto<Guid>
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }

    public Guid CategoryId { get; set; }
    public CategoryResponseDto? Category { get; set; }
}