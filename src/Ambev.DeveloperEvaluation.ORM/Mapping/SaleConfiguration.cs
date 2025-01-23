using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(
        EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
               .HasColumnType("uuid")
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Customer)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.Branch)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.Number)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Date)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.TotalAmount)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.IsCancelled)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired()
               .HasColumnType("timestamp with time zone")
               .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasMany(s => s.ListSaleItems)
               .WithOne()
               .HasForeignKey(si => si.SaleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}