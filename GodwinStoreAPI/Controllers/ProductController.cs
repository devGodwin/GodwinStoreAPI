using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Model.ProductModel.RequestModel;
using GodwinStoreAPI.Services.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace GodwinStoreAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController:ControllerBase
{
    private readonly IProductServices _productServices;

    public ProductController(IProductServices productServices)
    {
        _productServices = productServices;
    }
    
    
   /// <summary>
   /// Create a new product
   /// </summary>
   /// <param name="requestModel"></param>
   /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequestModel requestModel)
   {
       var response = await _productServices.CreateProductAsync(requestModel);
       return StatusCode(response.Code, response);
   }

   /// <summary>
   /// Filter products
   /// </summary>
   /// <param name="productFilter"></param>
   /// <returns></returns>
   [HttpGet()]
   public async Task<IActionResult> GetProducts([FromQuery]ProductFilter productFilter)
    {
        var response = await _productServices.GetProductsAsync(productFilter);
        return StatusCode(response.Code, response);
    }
    
    /// <summary>
    /// Retrieve a product
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpGet("{productId:required}")]
    public async Task<IActionResult> GetProductById([FromRoute]string productId)
    {
        var response = await _productServices.GetProductByIdAsync(productId);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// Update a product
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpPut("{productId:required}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] string productId,[FromBody] ProductUpdateModel updateModel)
    {
        var response =  await _productServices.UpdateProductAsync(productId,updateModel);
          return StatusCode(response.Code, response);
    }
    
    /// <summary>
    ///  Delete a product
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpDelete("{productId:required}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] string productId) 
    { 
        var response = await _productServices.DeleteProductAsync(productId); 
        return StatusCode(response.Code, response);
    }
    
}