using Fermion.EntityFramework.AuditLogs.Core.Entities;
using Fermion.EntityFramework.AuditLogs.Core.Options;
using Fermion.EntityFramework.AuditLogs.Extensions;
using Fermion.EntityFramework.AuditLogs.Infrastructure.EntityConfigurations;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fermion.EntityFramework.AuditLogs.Infrastructure.Contexts;

/// <summary>
/// Database context for managing audit logs and entity property changes.
/// </summary>
public class AuditLogDbContext : DbContext
{
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<EntityPropertyChange> EntityPropertyChanges { get; set; }

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<AuditLogOptions> _auditLogOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for accessing current request information.</param>
    /// <param name="auditLogOptions">The audit log configuration options.</param>
    protected AuditLogDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor, IOptions<AuditLogOptions> auditLogOptions) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _auditLogOptions = auditLogOptions;
    }

    /// <summary>
    /// Configures the database model and its relationships.
    /// </summary>
    /// <param name="builder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AuditLogConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(EntityPropertyChangeConfiguration).Assembly);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        this.SetCreationTimestamps(_httpContextAccessor);
        this.SetModificationTimestamps(_httpContextAccessor);
        this.SetSoftDelete(_httpContextAccessor);
        this.SetAuditLogAsync(_httpContextAccessor, _auditLogOptions.Value).GetAwaiter().GetResult();
        return base.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.SetCreationTimestamps(_httpContextAccessor);
        this.SetModificationTimestamps(_httpContextAccessor);
        this.SetSoftDelete(_httpContextAccessor);
        await this.SetAuditLogAsync(_httpContextAccessor, _auditLogOptions.Value);
        return await base.SaveChangesAsync(cancellationToken);
    }
}