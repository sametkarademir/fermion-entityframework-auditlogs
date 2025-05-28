using Fermion.Domain.Shared.Auditing;

namespace WebApi.Entities;

public class Category : FullAuditedEntity<Guid>
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Todo> Todos { get; set; } = [];
}