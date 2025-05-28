namespace WebApi.DTOs.Categories;

public class UpdateCategoryRequestDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}