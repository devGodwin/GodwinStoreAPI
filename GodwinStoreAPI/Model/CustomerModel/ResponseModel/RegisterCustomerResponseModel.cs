namespace GodwinStoreAPI.Model.CustomerModel.ResponseModel;

public class RegisterCustomerResponseModel
{
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Contact { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } 
}