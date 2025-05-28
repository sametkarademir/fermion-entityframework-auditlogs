using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Todos;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoControllers : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoControllers(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var todo = await _todoService.GetByIdAsync(id, cancellationToken);
        return Ok(todo);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var todos = await _todoService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(todos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTodoRequestDto request, CancellationToken cancellationToken)
    {
        var todo = await _todoService.CreateAsync(request, cancellationToken);
        return Ok(todo);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTodoRequestDto request, CancellationToken cancellationToken)
    {
        var todo = await _todoService.UpdateAsync(id, request, cancellationToken);
        return Ok(todo);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _todoService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}