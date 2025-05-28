using Fermion.EntityFramework.Shared.DTOs.Pagination;
using WebApi.DTOs.Todos;

namespace WebApi.Services;

public interface ITodoService
{
    Task<TodoResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PageableResponseDto<TodoResponseDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<TodoResponseDto> CreateAsync(CreateTodoRequestDto request, CancellationToken cancellationToken);
    Task<TodoResponseDto> UpdateAsync(Guid id, UpdateTodoRequestDto request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}