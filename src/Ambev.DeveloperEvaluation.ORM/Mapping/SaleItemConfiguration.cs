using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(
        EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
               .HasColumnType("uuid")
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Product)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.Quantity)
               .IsRequired();

        builder.Property(x => x.UnitPrice)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.Discount)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.IsCancelled)
            .IsRequired();

        builder.HasOne<Sale>()
               .WithMany(s => s.ListSaleItems)
               .HasForeignKey(x => x.SaleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}