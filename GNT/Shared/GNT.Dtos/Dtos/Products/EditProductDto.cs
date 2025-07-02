using GNT.Shared.Enums;

namespace GNT.Shared.Dtos.Products
{
    public class EditProductDto
    {
        public EditProductDto()
        {

        }

        public EditProductDto(string name, string code, decimal? price, ProductType? type, bool? isInStock, DateTime? datetIn, DateTime? dateOut)
        {
            Code = code;
            Name = name;
            Price = price;
            Type = type;
            IsInStock = isInStock;
            DatetIn = datetIn;
            DateOut = dateOut;
        }

        public EditProductDto(ProductDto model)
        {
            Code = model.Code;
            Name = model.Name;
            Price = model.Price;
            Type = model.Type;
            IsInStock = model.IsInStock;
            DatetIn = model.DatetIn;
            DateOut = model.DateOut;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public ProductType? Type { get; set; }
        public bool? IsInStock { get; set; }
        public DateTime? DatetIn { get; set; }
        public DateTime? DateOut { get; set; }
    }
}
