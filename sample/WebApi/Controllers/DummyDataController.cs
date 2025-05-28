using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Contexts;
using WebApi.Entities;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DummyDataController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DummyDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateDummyData()
    {
        try
        {
            // Clear existing data
            _context.Todos.RemoveRange(await _context.Todos.ToListAsync());
            _context.Categories.RemoveRange(await _context.Categories.ToListAsync());
            await _context.SaveChangesAsync();

            // Generate Categories
            var categories = new List<Category>
            {
                new Category { Name = "Work" },
                new Category { Name = "Personal" },
                new Category { Name = "Shopping" },
                new Category { Name = "Health" },
                new Category { Name = "Education" },
                new Category { Name = "Finance" },
                new Category { Name = "Travel" },
                new Category { Name = "Home" },
                new Category { Name = "Entertainment" },
                new Category { Name = "Social" }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Generate Todos
            var todos = new List<Todo>
            {
                new Todo { Title = "Complete Project", Description = "Finish the current project", CategoryId = categories[0].Id },
                new Todo { Title = "Buy Groceries", Description = "Get milk, eggs, and bread", CategoryId = categories[2].Id },
                new Todo { Title = "Exercise", Description = "30 minutes workout", CategoryId = categories[3].Id },
                new Todo { Title = "Read Book", Description = "Read 2 chapters", CategoryId = categories[4].Id },
                new Todo { Title = "Call Family", Description = "Weekly catch-up", CategoryId = categories[1].Id },
                new Todo { Title = "Pay Bills", Description = "Pay monthly bills", CategoryId = categories[5].Id },
                new Todo { Title = "Plan Vacation", Description = "Research travel destinations", CategoryId = categories[6].Id },
                new Todo { Title = "Clean House", Description = "Deep clean the house", CategoryId = categories[7].Id },
                new Todo { Title = "Watch Movie", Description = "Watch new release", CategoryId = categories[8].Id },
                new Todo { Title = "Attend Party", Description = "Prepare for friend's party", CategoryId = categories[9].Id },
                new Todo { Title = "Update Resume", Description = "Update professional experience", CategoryId = categories[0].Id },
                new Todo { Title = "Meditate", Description = "Daily meditation practice", CategoryId = categories[3].Id },
                new Todo { Title = "Study Spanish", Description = "Practice vocabulary", CategoryId = categories[4].Id },
                new Todo { Title = "Buy Birthday Gift", Description = "Get gift for mom", CategoryId = categories[2].Id },
                new Todo { Title = "Schedule Dentist", Description = "Book annual checkup", CategoryId = categories[3].Id }
            };

            await _context.Todos.AddRangeAsync(todos);
            await _context.SaveChangesAsync();

            // Perform multiple updates
            var updates = new List<(Todo todo, string newTitle, string newDescription)>
            {
                (todos[0], "Complete Project - Updated", "Finish the current project with new requirements"),
                (todos[1], "Buy Groceries - Updated", "Get milk, eggs, bread, and vegetables"),
                (todos[2], "Exercise - Updated", "45 minutes workout with cardio"),
                (todos[3], "Read Book - Updated", "Read 3 chapters and take notes"),
                (todos[4], "Call Family - Updated", "Weekly catch-up and plan weekend"),
                (todos[5], "Pay Bills - Updated", "Pay monthly bills and update budget"),
                (todos[6], "Plan Vacation - Updated", "Book flights and hotel"),
                (todos[7], "Clean House - Updated", "Deep clean the house and organize"),
                (todos[8], "Watch Movie - Updated", "Watch new release with friends"),
                (todos[9], "Attend Party - Updated", "Prepare for friend's birthday party")
            };

            foreach (var update in updates)
            {
                update.todo.Title = update.newTitle;
                update.todo.Description = update.newDescription;
                _context.Todos.Update(update.todo);
            }

            // Update categories
            var categoryUpdates = new List<(Category category, string newName)>
            {
                (categories[0], "Work - Updated"),
                (categories[1], "Personal - Updated"),
                (categories[2], "Shopping - Updated"),
                (categories[3], "Health - Updated"),
                (categories[4], "Education - Updated"),
                (categories[5], "Finance - Updated"),
                (categories[6], "Travel - Updated"),
                (categories[7], "Home - Updated"),
                (categories[8], "Entertainment - Updated"),
                (categories[9], "Social - Updated")
            };

            foreach (var update in categoryUpdates)
            {
                update.category.Name = update.newName;
                _context.Categories.Update(update.category);
            }

            await _context.SaveChangesAsync();

            // Perform some deletes
            var todosToDelete = todos.Skip(10).Take(3).ToList();
            _context.Todos.RemoveRange(todosToDelete);

            var categoriesToDelete = categories.Skip(7).Take(2).ToList();
            _context.Categories.RemoveRange(categoriesToDelete);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dummy data generated successfully",
                initialCategoriesCount = categories.Count,
                initialTodosCount = todos.Count,
                updatedTodosCount = updates.Count,
                updatedCategoriesCount = categoryUpdates.Count,
                deletedTodosCount = todosToDelete.Count,
                deletedCategoriesCount = categoriesToDelete.Count,
                updatedTodoIds = updates.Select(u => u.todo.Id).ToList(),
                updatedCategoryIds = categoryUpdates.Select(c => c.category.Id).ToList()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error generating dummy data", error = ex.Message });
        }
    }
}