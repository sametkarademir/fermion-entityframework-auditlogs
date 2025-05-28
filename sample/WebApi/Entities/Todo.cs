using Fermion.Domain.Shared.Auditing;

namespace WebApi.Entities;

public class Todo : FullAuditedEntity<Guid>
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}