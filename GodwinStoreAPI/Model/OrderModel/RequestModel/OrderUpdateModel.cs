using System.ComponentModel.DataAnnotations;

namespace GodwinStoreAPI.Model.OrderModel.RequestModel;

public class OrderUpdateModel
{
    [Required(AllowEmptyStrings = false)]
    public string CustomerName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string ShippingAddress { get; set; }
    [Required(AllowEmptyStrings = false)]
    public int Quantity { get; set; } 
    [Required(AllowEmptyStrings = false)]
    public decimal UnitPrice { get; set; }
}