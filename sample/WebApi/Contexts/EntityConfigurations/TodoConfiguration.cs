using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Contexts.EntityConfigurations;

public class TodoConfiguration : IEntityTypeConfiguration<Entities.Todo>
{
    public void Configure(EntityTypeBuilder<Entities.Todo> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        RelationalEntityTypeBuilderExtensions.ToTable((EntityTypeBuilder)builder, "Todos");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Title).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(500).IsRequired(false);
        builder.Property(item => item.IsCompleted).IsRequired();

        builder.HasOne(item => item.Category)
            .WithMany(item => item.Todos)
            .HasForeignKey(item => item.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}