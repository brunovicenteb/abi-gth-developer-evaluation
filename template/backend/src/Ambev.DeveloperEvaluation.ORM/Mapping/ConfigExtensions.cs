using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public static class ConfigExtensions
{
    public const short DEFAULT_NAME_FIELD_SIZE = 150;
    private const short DEFAULT_DECIMAL_PRECISON = 12;
    private const short DEFAULT_DECIMAL_SCALA = 2;

    public static PropertyBuilder<TProperty> AddDefaultName<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
        => propertyBuilder.AddDefaultString(DEFAULT_NAME_FIELD_SIZE, true);

    public static PropertyBuilder<TProperty> AddDefaultDateTime<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, bool isRequired = true)
        => propertyBuilder.HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired(isRequired);

    public static PropertyBuilder<TProperty> AddDefaultString<TProperty>(this PropertyBuilder<TProperty> propertyBuilder,
        int size = DEFAULT_NAME_FIELD_SIZE, bool isRequired = true)
    {
        return propertyBuilder
            .HasMaxLength(size)
            .IsRequired(isRequired);
    }

    public static PropertyBuilder<TProperty> AddDefaultDecimal<TProperty>(this PropertyBuilder<TProperty> propertyBuilder,
        bool isRequired = true, decimal? defaultValue = 0m)
    {
        var result = propertyBuilder
             .IsRequired(isRequired)
             .HasPrecision(DEFAULT_DECIMAL_PRECISON, DEFAULT_DECIMAL_SCALA);
        if (defaultValue.HasValue)
            result = result.HasDefaultValue(defaultValue);
        return result;
    }

    public static PropertyBuilder<TProperty> AddDefaultIntMinValueConstraint<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, EntityTypeBuilder builder, int minValue = 0)
    {
        var fieldName = propertyBuilder.Metadata.Name;
        var constraintName = $"CHK_{fieldName}_Min_Value";
        var constraintImplementation = $"(\"{fieldName}\" >= {minValue})";
        builder.ToTable(o => o.HasCheckConstraint(constraintName, constraintImplementation));
        return propertyBuilder;
    }

    public static PropertyBuilder<TProperty> AddDefaultDecimalMinValueConstraint<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, EntityTypeBuilder builder, decimal minValue = 0m)
    {
        var fieldName = propertyBuilder.Metadata.Name;
        var constraintName = $"CHK_{fieldName}_Min_Value";
        var constraintImplementation = $"(\"{fieldName}\" >= {minValue})";
        builder.ToTable(o => o.HasCheckConstraint(constraintName, constraintImplementation));
        return propertyBuilder;
    }
}