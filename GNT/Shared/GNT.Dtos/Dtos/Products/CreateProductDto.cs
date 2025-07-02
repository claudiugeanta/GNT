using GNT.Shared.Enums;

namespace GNT.Shared.Dtos.Products
{
    public class CreateProductDto
    {
        public CreateProductDto()
        {

        }

        public CreateProductDto(string name,string code, decimal price, ProductType type, bool isInStock, DateTime datetIn, DateTime dateOut)
        {
            Code = code;
            Name = name;
            Price = price;
            Type = type;
            IsInStock = isInStock;
            DatetIn = datetIn;
            DateOut = dateOut;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ProductType Type { get; set; }
        public bool IsInStock { get; set; }
        public DateTime DatetIn { get; set; }
        public DateTime DateOut { get; set; }
    }
}
