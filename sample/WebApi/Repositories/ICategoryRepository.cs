using Fermion.EntityFramework.Shared.Interfaces;
using WebApi.Entities;

namespace WebApi.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
}