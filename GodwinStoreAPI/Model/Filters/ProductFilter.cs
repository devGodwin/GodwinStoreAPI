namespace GodwinStoreAPI.Model.Filters;

public class ProductFilter:BaseFilter
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal ProductPrice { get; set; }
    public string ImageUrl { get; set; }
}