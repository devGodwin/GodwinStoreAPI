using GodwinStoreAPI.Model.OrderModel.RequestModel;
using GodwinStoreAPI.Model.OrderModel.ResponseModel;
using GodwinStoreAPI.Model.Responses;

namespace GodwinStoreAPI.Services.OrderServices;

public interface IOrderServices
{
    Task<BaseResponse<OrderResponseModel>> PlaceOrderAsync(OrderRequestModel orderResponse);
    Task<BaseResponse<IEnumerable<OrderResponseModel>>> GetAllOrdersAsync();
    Task<BaseResponse<OrderResponseModel>> GetOrderByIdAsync(string id);
    Task<BaseResponse<OrderResponseModel>> UpdateOrderAsync(string orderId, OrderUpdateModel orderUpdateModel);
    Task<BaseResponse<EmptyResponse>> DeleteOrderAsync(string orderId);
}