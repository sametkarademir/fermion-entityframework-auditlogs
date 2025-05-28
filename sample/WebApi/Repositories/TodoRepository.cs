using Fermion.EntityFramework.Shared.Repositories;
using WebApi.Contexts;

namespace WebApi.Repositories;

public class TodoRepository : EfRepositoryBase<Entities.Todo, Guid, ApplicationDbContext>, ITodoRepository
{
    public TodoRepository(ApplicationDbContext context) : base(context)
    {
    }
}