using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Model.CustomerModel.ResponseModel;
using GodwinStoreAPI.Model.Responses;

namespace GodwinStoreAPI.Services.AuthServices;

public interface IAuthServices
{
    Task<BaseResponse<RegisterCustomerResponseModel>> RegisterCustomerAsync(RegisterCustomerRequestModel registerCustomer);
    Task<BaseResponse<RegisterCustomerResponseModel>> LoginCustomerAsync(CustomerLoginRequestModel requestModel);
}