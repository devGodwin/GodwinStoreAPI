using GodwinStoreAPI.Model.OrderModel.RequestModel;
using GodwinStoreAPI.Services.OrderServices;
using Microsoft.AspNetCore.Mvc;

namespace GodwinStoreAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController:ControllerBase
{
    private readonly IOrderServices _orderServices;
    public OrderController(IOrderServices orderServices)
    {
        _orderServices = orderServices;
    }
    
    
   /// <summary>
   /// Place a new order
   /// </summary>
   /// <param name="requestModel"></param>
   /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] OrderRequestModel requestModel)
   {
       var response = await _orderServices.PlaceOrderAsync(requestModel);
       return StatusCode(response.Code, response);
   }
    
    /// <summary>
    /// Get all orders
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var response = await _orderServices.GetAllOrdersAsync();
        return StatusCode(response.Code, response);
    }
    
    /// <summary>
    /// Retrieve order
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("{orderId:required}")]
    public async Task<IActionResult> GetOrderById([FromRoute]string orderId)
    {
        var response = await _orderServices.GetOrderByIdAsync(orderId);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// Update order
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="updateModel"></param>
    /// <returns></returns>
    [HttpPut("{orderId:required}")]
    public async Task<IActionResult> UpdateOrder([FromRoute] string orderId, [FromBody] OrderUpdateModel updateModel)
    {
        var response = await _orderServices.UpdateOrderAsync(orderId, updateModel);
        return StatusCode(response.Code, response);
    }
    
    /// <summary>
    ///  Delete order
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpDelete("{orderId:required}")]
    public async Task<IActionResult> DeleteOrder([FromRoute] string orderId)
    {
        var response = await _orderServices.DeleteOrderAsync(orderId);
        return StatusCode(response.Code, response);
    }
    
}