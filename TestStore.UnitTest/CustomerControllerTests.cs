using System.Net;
using AutoFixture;
using AutoMapper;
using GodwinStoreAPI.Controllers;
using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Model.CustomerModel.ResponseModel;
using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Model.Responses;
using GodwinStoreAPI.Services.CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TestStore.UnitTest.TestSetup;
using Xunit;

namespace TestStore.UnitTest;

public class CustomerControllerTests : IClassFixture<TestFixture>
{
    private readonly CustomerController _customerController;
    private readonly Mock<ICustomerServices> _customerServicesMock = new Mock<ICustomerServices>();
    private readonly Fixture _fixture = new Fixture();

    public CustomerControllerTests(TestFixture fixture)
    {
        var logger = fixture.ServiceProvider.GetService<ILogger<CustomerController>>();
        var mapper = fixture.ServiceProvider.GetService<IMapper>();

        _customerController = new CustomerController(_customerServicesMock.Object);
    }

    [Fact]
    public async Task Filter_Customers_Return_Ok()
    {
        // Arrange
        var customers = _fixture.Create<PaginatedResponse<RegisterCustomerResponseModel>>();
        _customerServicesMock.Setup(repos => repos.GetCustomersAsync(It.IsAny<CustomerFilter>()))
            .ReturnsAsync(new BaseResponse<PaginatedResponse<RegisterCustomerResponseModel>>()
            {
              Code  = (int)HttpStatusCode.OK,
              Message = It.IsAny<string>(),
              Data = new PaginatedResponse<RegisterCustomerResponseModel>()
              {
                  CurrentPage = customers.CurrentPage,
                  PageSize = customers.PageSize,
                  TotalRecords = customers.TotalRecords,
                  TotalPages = customers.TotalPages,
                  Data = new List<RegisterCustomerResponseModel>()
              }
            });

        // Act
        var result = await _customerController.GetCustomers(It.IsAny<CustomerFilter>()) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }

    [Fact]
    public async Task Get_Customer_ById_Return_Ok()
    {
        // Arrange
        _customerServicesMock.Setup(repos => repos.GetCustomerByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new BaseResponse<RegisterCustomerResponseModel>()
            {
                Code = (int)HttpStatusCode.OK,
                Data = new RegisterCustomerResponseModel(),
            });

        // Act
        var result = await _customerController.GetCustomerById(It.IsAny<string>()) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }

    [Fact]
    public async Task Update_Customer_If_Null_Return_NotFound()
    {
        // Arrange
        var mockResponse = CommonResponses.ErrorResponse.NotFoundErrorResponse<RegisterCustomerResponseModel>(It.IsAny<string>());
        
        _customerServicesMock.Setup(repos =>
                repos.UpdateCustomerAsync(It.IsAny<string>(), It.IsAny<CustomerUpdateModel>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _customerController.UpdateCustomer(It.IsAny<string>(),new CustomerUpdateModel()) as ObjectResult;

        // Assert
        Assert.Equal(404,result?.StatusCode);
    }

    [Fact]
    public async Task Update_Customer_Return_Ok()
    {
        // Arrange
        var customer = _fixture.Create<RegisterCustomerResponseModel>();
        var mockResponse = CommonResponses.SuccessResponse.OkResponse(customer);
        
        _customerServicesMock.Setup(repos => repos.UpdateCustomerAsync(It.IsAny<string>(), It.IsAny<CustomerUpdateModel>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _customerController.UpdateCustomer( It.IsAny<string>(), new CustomerUpdateModel
        {
            Contact = customer.Contact,
            Email = customer.Email,
            CustomerName = customer.CustomerName
        }) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }

    [Fact]
    public async Task Delete_Customer_If_Null_Return_NotFound()
    {
        // Arrange
        var mockResponse = CommonResponses.ErrorResponse.NotFoundErrorResponse<EmptyResponse>(It.IsAny<string>());
        
        _customerServicesMock.Setup(repos => repos.DeleteCustomerAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _customerController.DeleteCustomer(It.IsAny<string>()) as ObjectResult;

        // Assert
        Assert.Equal(404,result?.StatusCode);
    }
    
    [Fact]
    public async Task Delete_Customer_Return_Ok()
    {
        // Arrange
        var mockResponse = CommonResponses.SuccessResponse.DeletedResponse();
        
        _customerServicesMock.Setup(repos => repos.DeleteCustomerAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = (ObjectResult) await _customerController.DeleteCustomer(It.IsAny<string>());

        // Assert
        Assert.Equal(200,result.StatusCode);
    }
    
}

