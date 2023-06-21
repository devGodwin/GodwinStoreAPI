using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Model.CustomerModel.ResponseModel;
using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Model.Responses;

namespace GodwinStoreAPI.Services.CustomerServices;

public interface ICustomerServices
{
    Task<BaseResponse<PaginatedResponse<RegisterCustomerResponseModel>>> GetCustomersAsync(CustomerFilter customerFilter);
    Task<BaseResponse<RegisterCustomerResponseModel>> GetCustomerByIdAsync(string id);
    Task<BaseResponse<RegisterCustomerResponseModel>> UpdateCustomerAsync(string customerId, CustomerUpdateModel updateModel);
    Task<BaseResponse<EmptyResponse>> DeleteCustomerAsync(string customerId);
}