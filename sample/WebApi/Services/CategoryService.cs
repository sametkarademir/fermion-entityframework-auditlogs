using AutoMapper;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using WebApi.DTOs.Categories;
using WebApi.Entities;
using WebApi.Repositories;

namespace WebApi.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IMapper mapper)
    : ICategoryService
{
    public async Task<CategoryResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);

        return mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<PageableResponseDto<CategoryResponseDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetListAsync(
            predicate: null,
            include: null,
            orderBy: item => item.OrderByDescending(category => category.CreationTime),
            index: pageNumber,
            size: pageSize,
            cancellationToken: cancellationToken
            );

        var mappedCategories = mapper.Map<List<CategoryResponseDto>>(categories.Data);
        return new PageableResponseDto<CategoryResponseDto>(mappedCategories, categories.Meta);
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };
        var createdCategory = await categoryRepository.AddAsync(category);
        await categoryRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<CategoryResponseDto>(createdCategory);
    }

    public async Task<CategoryResponseDto> UpdateAsync(Guid id, UpdateCategoryRequestDto request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);

        category.Name = request.Name;
        category.Description = request.Description;

        var updatedCategory = await categoryRepository.UpdateAsync(category);
        await categoryRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<CategoryResponseDto>(updatedCategory);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);

        await categoryRepository.DeleteAsync(category);
        await categoryRepository.SaveChangesAsync(cancellationToken);
    }
}