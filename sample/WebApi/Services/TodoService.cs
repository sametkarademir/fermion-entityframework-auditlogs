using AutoMapper;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using WebApi.DTOs.Todos;
using WebApi.Repositories;

namespace WebApi.Services;

public class TodoService(
    ITodoRepository todoRepository,
    IMapper mapper
)
    : ITodoService
{
    public async Task<TodoResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);

        return mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<PageableResponseDto<TodoResponseDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var todos = await todoRepository.GetListAsync(
            predicate: null,
            include: null,
            orderBy: item => item.OrderByDescending(todo => todo.CreationTime),
            index: pageNumber,
            size: pageSize,
            cancellationToken: cancellationToken
        );

        var mappedTodos = mapper.Map<List<TodoResponseDto>>(todos.Data);
        return new PageableResponseDto<TodoResponseDto>(mappedTodos, todos.Meta);
    }

    public async Task<TodoResponseDto> CreateAsync(CreateTodoRequestDto request, CancellationToken cancellationToken)
    {
        var todo = new Entities.Todo
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            CategoryId = request.CategoryId
        };

        var createdTodo = await todoRepository.AddAsync(todo);
        await todoRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<TodoResponseDto>(createdTodo);
    }

    public async Task<TodoResponseDto> UpdateAsync(Guid id, UpdateTodoRequestDto request, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.IsCompleted = request.IsCompleted;

        var updatedTodo = await todoRepository.UpdateAsync(todo);
        await todoRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<TodoResponseDto>(updatedTodo);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);

        await todoRepository.DeleteAsync(todo);
        await todoRepository.SaveChangesAsync(cancellationToken);
    }
}