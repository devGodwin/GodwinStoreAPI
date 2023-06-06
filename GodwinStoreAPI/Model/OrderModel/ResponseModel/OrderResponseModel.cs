namespace GodwinStoreAPI.Model.OrderModel.ResponseModel;

public class OrderResponseModel
{

    public string OrderId { get; set; } 
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; }
    public string ShippingAddress { get; set; }
    public int Quantity { get; set; } 
    public decimal UnitPrice { get; set; } 
    public decimal TotalPrice { get; set; } 
    public DateTime CreatedAt { get; set; } 
    public string TrackingStatus { get; set; }
    
}