using Fermion.EntityFramework.AuditLogs.Domain.Entities;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.AuditLogs.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the EntityPropertyChange entity.
/// </summary>
public class EntityPropertyChangeConfiguration : IEntityTypeConfiguration<EntityPropertyChange>
{
    /// <summary>
    /// Configures the entity type and its properties.
    /// </summary>
    /// <param name="builder">The builder being used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<EntityPropertyChange> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        builder.ToTable("EntityPropertyChanges");

        builder.Property(item => item.PropertyName).HasMaxLength(500).IsRequired();
        builder.Property(item => item.PropertyTypeFullName).HasMaxLength(1000).IsRequired();
        builder.Property(item => item.NewValue).IsRequired(false);
        builder.Property(item => item.OriginalValue).IsRequired(false);

        builder.HasOne(item => item.AuditLog)
            .WithMany(a => a.EntityPropertyChanges)
            .HasForeignKey(item => item.AuditLogId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
