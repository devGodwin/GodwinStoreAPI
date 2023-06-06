using System.ComponentModel.DataAnnotations;

namespace GodwinStoreAPI.Data.StoreData;


public class Customer
{ 
    [Key]
    public string CustomerId { get; set; } = Guid.NewGuid().ToString("N");
    public string CustomerName { get; set; }
    public string Contact { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    
}
