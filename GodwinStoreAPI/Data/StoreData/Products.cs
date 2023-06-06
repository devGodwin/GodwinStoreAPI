using System.ComponentModel.DataAnnotations;

namespace GodwinStoreAPI.Data.StoreData;

public class Product
{
    [Key]
    public string ProductId { get; set; } = Guid.NewGuid().ToString("N");
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal ProductPrice { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}