using Fermion.EntityFramework.Shared.Interfaces;

namespace WebApi.Repositories;

public interface ITodoRepository : IRepository<Entities.Todo, Guid>
{
}