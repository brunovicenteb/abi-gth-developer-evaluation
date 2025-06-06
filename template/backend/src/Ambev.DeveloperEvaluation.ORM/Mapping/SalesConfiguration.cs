using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalesConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.CustomerId).HasColumnType("uuid").IsRequired();
        builder.Property(u => u.CreatedAt).AddDefaultDateTime();
        builder.Property(u => u.SaleNumber).AddDefaultString();
        builder.Property(u => u.Branch).AddDefaultString();
        builder.Property(u => u.CustomerName).AddDefaultString();
        builder.Property(u => u.TotalAmount)
            .AddDefaultDecimal()
            .AddDefaultDecimalMinValueConstraint(builder);
        builder.Property(u => u.IsCancelled).IsRequired();
        builder.HasMany(u => u.Items)
            .WithOne()
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}