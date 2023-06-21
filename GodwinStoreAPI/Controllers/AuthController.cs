using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Services.AuthServices;
using Microsoft.AspNetCore.Mvc;

namespace GodwinStoreAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController:ControllerBase
{
    private readonly IAuthServices _authServices;

    public AuthController(IAuthServices authServices)
    {
        _authServices = authServices;
    }
    
    /// <summary>
    /// Register a new customer
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequestModel requestModel)
    {
        var response = await _authServices.RegisterCustomerAsync(requestModel);
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
        var response = await _authServices.LoginCustomerAsync(requestModel);
        return StatusCode(response.Code,response);
    }
}