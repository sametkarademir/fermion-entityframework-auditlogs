using AutoMapper;
using WebApi.DTOs.Categories;
using WebApi.DTOs.Todos;
using WebApi.Entities;

namespace WebApi.Profiles;

public class EntityProfiles : Profile
{
    public EntityProfiles()
    {
        CreateMap<Category, CategoryResponseDto>();
        CreateMap<Entities.Todo, TodoResponseDto>();
    }
}