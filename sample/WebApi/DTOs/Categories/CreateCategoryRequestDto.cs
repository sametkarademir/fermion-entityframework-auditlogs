namespace WebApi.DTOs.Categories;

public class CreateCategoryRequestDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}