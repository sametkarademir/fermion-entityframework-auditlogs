using System.Reflection;
using Fermion.EntityFramework.AuditLogs.Core.Options;
using Fermion.EntityFramework.AuditLogs.Extensions;
using Fermion.EntityFramework.AuditLogs.Infrastructure.EntityConfigurations;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApi.Entities;

namespace WebApi.Contexts;

public class ApplicationDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Todo> Todos { get; set; }

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<AuditLogOptions> _auditLogOptions;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor, IOptions<AuditLogOptions> auditLogOptions) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _auditLogOptions = auditLogOptions;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyConfigurationsFromAssembly(typeof(AuditLogConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(EntityPropertyChangeConfiguration).Assembly);
    }

    public override int SaveChanges()
    {
        this.SetCreationTimestamps(_httpContextAccessor);
        this.SetModificationTimestamps(_httpContextAccessor);
        this.SetSoftDelete(_httpContextAccessor);
        this.SetAuditLogAsync(_httpContextAccessor, _auditLogOptions.Value).GetAwaiter().GetResult();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.SetCreationTimestamps(_httpContextAccessor);
        this.SetModificationTimestamps(_httpContextAccessor);
        this.SetSoftDelete(_httpContextAccessor);
        await this.SetAuditLogAsync(_httpContextAccessor, _auditLogOptions.Value);
        return await base.SaveChangesAsync(cancellationToken);
    }
}