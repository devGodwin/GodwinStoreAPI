using AutoMapper;
using GodwinStoreAPI.Data.DbContexts;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.ElasticSearch;
using GodwinStoreAPI.Model.OrderModel.RequestModel;
using GodwinStoreAPI.Model.OrderModel.ResponseModel;
using GodwinStoreAPI.Model.Responses;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GodwinStoreAPI.Services.OrderServices;

public class OrderServices : IOrderServices
{
    private readonly OrderContext _orderContext;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderServices> _logger;
    private readonly IElasticSearchService _elasticSearchService;

    public OrderServices(OrderContext orderContext, IMapper mapper, ILogger<OrderServices>logger,IElasticSearchService elasticSearchService)
    {
        _orderContext = orderContext;
        _mapper = mapper;
        _logger = logger;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<BaseResponse<OrderResponseModel>> PlaceOrderAsync(OrderRequestModel orderRequestModel)
    {
        try
        {
            var orderNumberExist = await _orderContext.Order.AnyAsync(x => x.OrderNumber.Equals(orderRequestModel.OrderNumber));
            if (orderNumberExist)
            {
                return CommonResponses.ErrorResponse.ConflictErrorResponse<OrderResponseModel>("Order is already placed");
            }
            
            var newOrder =  _mapper.Map<Order>(orderRequestModel); 
            newOrder.TotalPrice = newOrder.UnitPrice * newOrder.Quantity;
            
            newOrder.TrackingStatus = "Processing";
            
            await _orderContext.Order.AddAsync(newOrder); 
            
            var rows = await _orderContext.SaveChangesAsync();
            if (rows < 1) 
            { 
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<OrderResponseModel>(); 
            }
            
            await _elasticSearchService.AddAsync(newOrder);
            
            return CommonResponses.SuccessResponse.CreatedResponse(_mapper.Map<OrderResponseModel>(newOrder));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured placing order\n{orderResponse}",JsonConvert.SerializeObject(orderRequestModel, Formatting.Indented));
            
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<OrderResponseModel>();
        }
    }

    public async Task<BaseResponse<IEnumerable<OrderResponseModel>>> GetAllOrdersAsync()
    {
        var orders = await _orderContext.Order.AsNoTracking().ToListAsync(); 
        
        return CommonResponses.SuccessResponse.OkResponse(_mapper.Map<IEnumerable<OrderResponseModel>>(orders));
    }

    public async Task<BaseResponse<OrderResponseModel>> GetOrderByIdAsync(string orderId)
    {
        var orderExist = await _orderContext.Order.FirstOrDefaultAsync(x => x.OrderId.Equals(orderId));
        
        if (orderExist == null)
        {
            return CommonResponses.ErrorResponse.NotFoundErrorResponse<OrderResponseModel>("Order not found");
        }

        if (orderExist.IsShipped)
        {
            orderExist.TrackingStatus = "Your order has been shipped";
        }
        else if (orderExist.IsDelivered)
        {
            orderExist.TrackingStatus = "Your order has been delivered";
        }
        else
        {
            orderExist.TrackingStatus = "Your order is being processing";
        }
        
        await _elasticSearchService.GetByIdAsync<OrderResponseModel>(orderId);

        return CommonResponses.SuccessResponse.OkResponse(_mapper.Map<OrderResponseModel>(orderExist));
    }

    public async Task<BaseResponse<OrderResponseModel>> UpdateOrderAsync(string orderId, OrderUpdateModel orderUpdateModel)
    {
        try
        {
            var orderExist = await _orderContext.Order.FirstOrDefaultAsync(x => x.OrderId.Equals(orderId));
            if (orderExist == null)
            {
                return CommonResponses.ErrorResponse.NotFoundErrorResponse<OrderResponseModel>("Order not found");
            }
        
            var updateOrder = _mapper.Map(orderUpdateModel,orderExist); 
            _orderContext.Order.Update(updateOrder);

            updateOrder.TotalPrice = updateOrder.UnitPrice * updateOrder.Quantity;
            
            var rows = await _orderContext.SaveChangesAsync();
            if (rows < 1)
            {
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<OrderResponseModel>();
            }

            await _elasticSearchService.UpdateAsync(updateOrder.OrderId, updateOrder);
            
            return CommonResponses.SuccessResponse.CreatedResponse(_mapper.Map<OrderResponseModel>(updateOrder));
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured updating order\n{orderUpdateModel}",JsonConvert.SerializeObject(orderUpdateModel,Formatting.Indented));
            
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<OrderResponseModel>();
        }
    }

    public async Task<BaseResponse<EmptyResponse>> DeleteOrderAsync(string orderId)
    {
        try
        {
            var orderExist = await _orderContext.Order.FirstOrDefaultAsync(x => x.OrderId.Equals(orderId));
            if (orderExist == null)
            {
                return CommonResponses.ErrorResponse.NotFoundErrorResponse<EmptyResponse>("Order not found");
            }
            
            _orderContext.Order.Remove(orderExist); 
            
            var rows = await _orderContext.SaveChangesAsync();
            if (rows < 1)
            {
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<EmptyResponse>();
            }

            await _elasticSearchService.DeleteAsync<EmptyResponse>(orderId);
            
            return CommonResponses.SuccessResponse.DeletedResponse();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured deleting order\n{orderId}",JsonConvert.SerializeObject(orderId,Formatting.Indented));
            
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<EmptyResponse>();
        }
    }
}