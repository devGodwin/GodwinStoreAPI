using System.ComponentModel.DataAnnotations;

namespace GodwinStoreAPI.Model.CustomerModel.RequestModel;

public class CustomerUpdateModel
{
    [Required(AllowEmptyStrings = false)]
    public string CustomerName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Contact { get; set; }
    [Required,EmailAddress]
    public string Email { get; set; }
    
    
}