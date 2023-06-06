namespace GodwinStoreAPI.Model.Filters;

public class CustomerFilter:BaseFilter
{
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Contact { get; set; }
    public string Email { get; set; }
}