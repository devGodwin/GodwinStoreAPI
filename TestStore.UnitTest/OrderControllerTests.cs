using System.Net;
using AutoFixture;
using AutoMapper;
using GodwinStoreAPI.Controllers;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.Model.OrderModel.RequestModel;
using GodwinStoreAPI.Model.OrderModel.ResponseModel;
using GodwinStoreAPI.Model.Responses;
using GodwinStoreAPI.Services.OrderServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TestStore.UnitTest.TestSetup;
using Xunit;

namespace TestStore.UnitTest;

public class OrderControllerTests:IClassFixture<TestFixture>
{
    private readonly OrderController _orderController;
    private readonly Mock<IOrderServices> _orderServicesMock = new Mock<IOrderServices>();
    private readonly Fixture _fixture = new Fixture();

    public OrderControllerTests(TestFixture fixture)
    {
        var logger = fixture.ServiceProvider.GetService<ILogger<OrderController>>();
        var mapper = fixture.ServiceProvider.GetService<IMapper>();

        _orderController = new OrderController(_orderServicesMock.Object);
    }

    [Fact]
    public async Task Place_Order_Return_Ok()
    {
        // Arrange
        var order = _fixture.Create<Order>();
        
        _orderServicesMock.Setup(repos => repos.PlaceOrderAsync(It.IsAny<OrderRequestModel>()))
            .ReturnsAsync(new BaseResponse<OrderResponseModel>()
            {
                Code = (int)HttpStatusCode.Created,
                Data = new OrderResponseModel(),
                Message = It.IsAny<string>()
            });

        // Act
        var result = await _orderController.PlaceOrder(new OrderRequestModel()
        {
            Quantity = order.Quantity,
            UnitPrice = order.UnitPrice,
            CustomerName = order.CustomerName,
            OrderNumber = order.OrderNumber,
            ShippingAddress = order.ShippingAddress
        }) as ObjectResult;

        // Assert
        Assert.Equal(201,result?.StatusCode);
    }

    [Fact]
    public async Task Get_All_Orders_Return_Ok()
    {
        // Arrange
        _orderServicesMock.Setup(repos => repos.GetAllOrdersAsync())
            .ReturnsAsync(new BaseResponse<IEnumerable<OrderResponseModel>>()
            {
                Code = (int)HttpStatusCode.OK,
                Data = new List<OrderResponseModel>(),
                Message = It.IsAny<string>()
            });

        // Act
        var result = await _orderController.GetAllOrders() as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }
    
    [Fact]
    public async Task Get_Order_ById_Return_Ok()
    {
        // Arrange
        _orderServicesMock.Setup(repos => repos.GetOrderByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new BaseResponse<OrderResponseModel>()
            {
                Code = (int)HttpStatusCode.OK,
                Data = new OrderResponseModel(),
                Message = It.IsAny<string>()
            });
        
        // Act
        var result = await _orderController.GetOrderById(It.IsAny<string>()) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }

    [Fact]
    public async Task Update_Order_If_Null_Return_Ok()
    {
        // Arrange
        var order = _fixture.Create<Order>();
        
        _orderServicesMock.Setup(repos => repos.UpdateOrderAsync(It.IsAny<string>(),It.IsAny<OrderUpdateModel>()))
            .ReturnsAsync(new BaseResponse<OrderResponseModel>()
            {
                Code = (int)HttpStatusCode.NotFound,
                Data = new OrderResponseModel(),
                Message = It.IsAny<string>()
            });

        // Act
        var result = await _orderController.UpdateOrder(It.IsAny<string>(),new OrderUpdateModel()
        {
           CustomerName = order.CustomerName,
           Quantity = order.Quantity,
           ShippingAddress = order.ShippingAddress,
           UnitPrice = order.UnitPrice
        }) as ObjectResult;

        // Assert
        Assert.Equal(404,result?.StatusCode);
    }
    
    [Fact]
    public async Task Update_Order_Return_Ok()
    {
        // Arrange
        var order = _fixture.Create<OrderResponseModel>();
        var mockResponse = CommonResponses.SuccessResponse.OkResponse(order,It.IsAny<string>());
        _orderServicesMock.Setup(repos => repos.UpdateOrderAsync(It.IsAny<string>(),It.IsAny<OrderUpdateModel>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.UpdateOrder( It.IsAny<string>(),new OrderUpdateModel()
        {
            CustomerName = order.CustomerName,
            Quantity = order.Quantity,
            ShippingAddress = order.ShippingAddress,
            UnitPrice = order.UnitPrice
        }) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }
    
    [Fact]
    public async Task Delete_Order_If_Null_Return_NotFound()
    {
        // Arrange
        var mockResponse = CommonResponses.ErrorResponse.NotFoundErrorResponse<EmptyResponse>(It.IsAny<string>());
        
        _orderServicesMock.Setup(repos => repos.DeleteOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);
        
        // Act
        var result = await _orderController.DeleteOrder(It.IsAny<string>()) as ObjectResult;

        // Assert
        Assert.Equal(404,result?.StatusCode);
    }

    [Fact]
    public async Task Delete_Order_Return_Ok()
    {
        // Arrange
        var mockResponse = CommonResponses.SuccessResponse.DeletedResponse();
        
        _orderServicesMock.Setup(repos => repos.DeleteOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);
        
        // Act
        var result = await _orderController.DeleteOrder(It.IsAny<string>()) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }
    
}
