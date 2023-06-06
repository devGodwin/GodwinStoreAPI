using System.ComponentModel.DataAnnotations;

namespace GodwinStoreAPI.Data.StoreData;

public class Order
{

    [Key]
    public string OrderId { get; set; } = Guid.NewGuid().ToString("N");
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; } 
    public string ShippingAddress { get; set; }
    public int Quantity { get; set; } 
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string TrackingStatus { get; set; }
    public bool IsShipped { get; set; }
    public bool IsDelivered { get; set; }
}
