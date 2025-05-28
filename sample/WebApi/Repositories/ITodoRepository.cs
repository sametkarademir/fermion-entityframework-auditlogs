using Fermion.EntityFramework.Shared.Repositories.Abstractions;

namespace WebApi.Repositories;

public interface ITodoRepository : IRepository<Entities.Todo, Guid>
{
}