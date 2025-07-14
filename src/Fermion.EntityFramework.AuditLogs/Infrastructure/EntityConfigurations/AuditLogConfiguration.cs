using Fermion.EntityFramework.AuditLogs.Domain.Entities;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.AuditLogs.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the AuditLog entity.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <summary>
    /// Configures the entity type and its properties.
    /// </summary>
    /// <param name="builder">The builder being used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        builder.ToTable("AuditLogs");
        builder.HasIndex(item => item.State);
        builder.HasIndex(item => item.EntityId);
        builder.HasIndex(item => item.EntityName);

        builder.Property(item => item.EntityId).HasMaxLength(100).IsRequired();
        builder.Property(item => item.EntityName).HasMaxLength(500).IsRequired();
        builder.Property(item => item.State).IsRequired();
    }
}