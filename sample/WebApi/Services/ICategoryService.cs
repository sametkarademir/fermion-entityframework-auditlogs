using Fermion.EntityFramework.Shared.DTOs.Pagination;
using WebApi.DTOs.Categories;

namespace WebApi.Services;

public interface ICategoryService
{
    Task<CategoryResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PageableResponseDto<CategoryResponseDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto request, CancellationToken cancellationToken);
    Task<CategoryResponseDto> UpdateAsync(Guid id, UpdateCategoryRequestDto request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

