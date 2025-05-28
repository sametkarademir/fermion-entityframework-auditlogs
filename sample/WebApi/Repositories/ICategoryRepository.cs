using Fermion.EntityFramework.Shared.Repositories.Abstractions;
using WebApi.Entities;

namespace WebApi.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
}