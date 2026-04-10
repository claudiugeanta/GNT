using GNT.Domain.BaseModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GNT.Enums;

namespace GNT.Domain.Models;

public class BusinessProduct : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public BusinessProductType Type { get; set; }
    public bool IsInStock { get; set; }
    public DateTime DatetIn {  get; set; }
    public DateTime DateOut { get; set; }

}

public class BusinessProductConfiguration : IEntityTypeConfiguration<BusinessProduct>
{
    public void Configure(EntityTypeBuilder<BusinessProduct> entity)
    {
        entity.ConfigureBase();

        entity.Property(e => e.Price)
            .HasPrecision(18, 2);
    }
}


