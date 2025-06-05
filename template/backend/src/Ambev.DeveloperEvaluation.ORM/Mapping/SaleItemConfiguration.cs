using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SalesItems");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.ProductId).HasColumnType("uuid").IsRequired();
        builder.Property(u => u.SaleId).HasColumnType("uuid").IsRequired();
        builder.Property(u => u.ProductName).AddDefaultString();
        builder.Property(u => u.Quantity)
            .AddDefaultIntMinValueConstraint(builder, 1);
        builder.Property(u => u.UnitPrice)
            .AddDefaultDecimal()
            .AddDefaultDecimalMinValueConstraint(builder);
        builder.Property(u => u.Discount)
            .AddDefaultDecimal()
            .AddDefaultDecimalMinValueConstraint(builder);
        builder.Property(u => u.Total)
            .AddDefaultDecimal()
            .AddDefaultDecimalMinValueConstraint(builder);
    }
}