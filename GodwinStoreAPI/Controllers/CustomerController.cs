using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Services.CustomerServices;
using Microsoft.AspNetCore.Mvc;

namespace GodwinStoreAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController:ControllerBase
{
    private readonly ICustomerServices _customerServices;

    public CustomerController(ICustomerServices customerServices)
    {
        _customerServices = customerServices;
    }
    

    /// <summary>
   /// Register a new customer
   /// </summary>
   /// <param name="requestModel"></param>
   /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequestModel requestModel)
    {
        var response = await _customerServices.RegisterCustomerAsync(requestModel);
        return StatusCode(response.Code,response);
    }
    
    /// <summary>
    /// Login Customer
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> LoginCustomer([FromBody] CustomerLoginRequestModel requestModel)
    {
        var response = await _customerServices.LoginCustomerAsync(requestModel);
        return StatusCode(response.Code,response);
    }
    
    /// <summary>
    /// Filter customers
    /// </summary>
    /// <param name="customerFilter"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<IActionResult> GetCustomers([FromQuery]CustomerFilter customerFilter)
    {
        var response= await _customerServices.GetCustomersAsync(customerFilter);
        return StatusCode(response.Code, response);
    }
    
    /// <summary>
    /// Retrieve a product
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet("{customerId:required}")]
    public async Task<IActionResult> GetCustomerById([FromRoute]string customerId)
    {
        var response = await _customerServices.GetCustomerByIdAsync(customerId);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// Update a customer
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="updateModel"></param>
    /// <returns></returns>
    [HttpPut("{customerId:required}")]
    public async Task<IActionResult> UpdateCustomer([FromRoute] string customerId, [FromBody] CustomerUpdateModel updateModel)
    {
        var response = await _customerServices.UpdateCustomerAsync(customerId, updateModel);
        return StatusCode(response.Code, response);
    }
    
    /// <summary>
    ///  Delete a customer
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpDelete("{customerId:required}")]
    public async Task<IActionResult> DeleteCustomer([FromRoute] string customerId)
    {
        var response = await _customerServices.DeleteCustomerAsync(customerId);
        return StatusCode(response.Code, response);

    }
    
}