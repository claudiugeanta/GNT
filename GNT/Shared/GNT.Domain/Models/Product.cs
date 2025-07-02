using GNT.Domain.BaseModels;
using GNT.Shared.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GNT.Shared.Dtos.Products;

namespace GNT.Domain.Models;

public class Product : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ProductType Type { get; set; }
    public bool IsInStock { get; set; }
    public DateTime DatetIn {  get; set; }
    public DateTime DateOut { get; set; }

}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.ConfigureBase();
    }
}

public static class ProductMapping
{
    public static Expression<Func<Product, ProductDto>> DtoProjection
    {
        get
        {
            return d => new ProductDto
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                Price = d.Price,
                Type = d.Type,
                IsInStock = d.IsInStock,
                DatetIn = d.DatetIn,
                DateOut = d.DateOut
            };
        }
    }

    public static Product CreateEntity(this CreateProductDto d)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = d.Name,
            Code = d.Code,
            Price = d.Price,
            Type = d.Type,
            IsInStock = d.IsInStock,
            DatetIn = d.DatetIn,
            DateOut = d.DateOut
        };
    }
}
