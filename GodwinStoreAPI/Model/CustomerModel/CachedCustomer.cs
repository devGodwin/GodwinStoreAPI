namespace GodwinStoreAPI.Model.CustomerModel;


public class CachedCustomer
{
    public string CustomerId { get; set; } = Guid.NewGuid().ToString("N");
    public string CustomerName { get; set; }
    public string Contact { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
