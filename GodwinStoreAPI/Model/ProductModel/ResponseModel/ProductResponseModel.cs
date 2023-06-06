namespace GodwinStoreAPI.Model.ProductModel.ResponseModel;

public class ProductResponseModel
{
    public string ProductId { get; set; } 
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal ProductPrice { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } 
}