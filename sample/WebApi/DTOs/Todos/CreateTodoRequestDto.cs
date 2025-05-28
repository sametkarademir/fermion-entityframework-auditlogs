namespace WebApi.DTOs.Todos;

public class CreateTodoRequestDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }

    public Guid CategoryId { get; set; }
}