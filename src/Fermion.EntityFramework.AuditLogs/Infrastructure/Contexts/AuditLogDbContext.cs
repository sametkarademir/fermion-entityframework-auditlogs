using Fermion.EntityFramework.AuditLogs.Domain.Entities;
using Fermion.EntityFramework.AuditLogs.Domain.Options;
using Fermion.EntityFramework.AuditLogs.Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fermion.EntityFramework.AuditLogs.Infrastructure.Contexts;

/// <summary>
/// Database context for managing audit logs and entity property changes.
/// </summary>
public class AuditLogDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<AuditLogOptions> _auditLogOptions;

    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<EntityPropertyChange> EntityPropertyChanges { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for accessing current request information.</param>
    /// <param name="auditLogOptions">The audit log configuration options.</param>
    public AuditLogDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor, IOptions<AuditLogOptions> auditLogOptions)
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
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AuditLogConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(EntityPropertyChangeConfiguration).Assembly);
    }
}