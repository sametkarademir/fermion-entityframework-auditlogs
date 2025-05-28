using System.Reflection;
using Fermion.EntityFramework.AuditLogs.Core.Enums;
using Fermion.EntityFramework.AuditLogs.DependencyInjection;
using Fermion.Extensions.ServiceCollections;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Contexts;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"] ?? string.Empty;
builder.Services.AddDbContextFactory<ApplicationDbContext>(opt => opt.UseNpgsql(connectionString), ServiceLifetime.Scoped);

builder.Services.AddFermionAuditLogServices<ApplicationDbContext>(opt =>
{
    opt.Enabled = true;
    opt.MaskPattern = "***MASKED***";
    opt.SensitiveProperties = ["Password", "Token", "Secret", "ApiKey", "Key", "Credential", "Ssn", "Credit", "Card", "SecurityCode", "Pin", "Authorization"];
    opt.ExcludedPropertiesByEntityType = new Dictionary<Type, HashSet<string>>
    {
        //{ typeof(Todo), ["AssignedTo"] }
    };
    opt.IncludedEntityTypes = [
        //typeof(Todo) 
    ];
    opt.ExcludedEntityTypes = [
        //typeof(Todo) 
    ];
    opt.LogChangeDetails = true;
    opt.MaxValueLength = 5000;
    opt.LoggedStates = [
        States.Added,
        States.Modified,
        States.Deleted
    ];
    opt.ApiRoute = "api/audit-logs";
    opt.EnableApiEndpoints = true;
    opt.Authorization.RequireAuthentication = false;
    opt.Authorization.GlobalPolicy = null;
    opt.Authorization.EndpointPolicies = null;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddSwaggerDocumentation(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();