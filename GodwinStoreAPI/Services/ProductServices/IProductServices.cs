using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Model.ProductModel.RequestModel;
using GodwinStoreAPI.Model.ProductModel.ResponseModel;
using GodwinStoreAPI.Model.Responses;

namespace GodwinStoreAPI.Services.ProductServices;

public interface IProductServices
{
    Task<BaseResponse<ProductResponseModel>> CreateProductAsync(ProductRequestModel requestModel);
    Task<BaseResponse<PaginatedResponse<ProductResponseModel>>> GetProductsAsync(ProductFilter productFilter);
    Task<BaseResponse<ProductResponseModel>> GetProductByIdAsync(string id);
    Task<BaseResponse<ProductUpdateModel>> UpdateProductAsync(string id,ProductUpdateModel updateModel);
    Task<BaseResponse<EmptyResponse>> DeleteProductAsync(string id);
}