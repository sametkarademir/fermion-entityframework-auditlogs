using Fermion.EntityFramework.Shared.Repositories;
using WebApi.Contexts;
using WebApi.Entities;

namespace WebApi.Repositories;

public class CategoryRepository : EfRepositoryBase<Category, Guid, ApplicationDbContext>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}