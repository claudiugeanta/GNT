using GNT.Shared.Enums;

namespace GNT.Shared.Dtos.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ProductType Type { get; set; }
    public bool IsInStock { get; set; }
    public DateTime DatetIn { get; set; }
    public DateTime DateOut { get; set; }
}
