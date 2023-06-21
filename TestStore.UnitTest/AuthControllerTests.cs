using System.Net;
using AutoFixture;
using AutoMapper;
using GodwinStoreAPI.Controllers;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Model.CustomerModel.ResponseModel;
using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Model.Responses;
using GodwinStoreAPI.Services.AuthServices;
using GodwinStoreAPI.Services.CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TestStore.UnitTest.TestSetup;
using Xunit;

namespace TestStore.UnitTest;

public class AuthControllerTests : IClassFixture<TestFixture>
{
    private readonly AuthController _authController;
    private readonly Mock<IAuthServices> _authServicesMock = new Mock<IAuthServices>();
    private readonly Fixture _fixture = new Fixture();

    public AuthControllerTests(TestFixture fixture)
    {
        var logger = fixture.ServiceProvider.GetService<ILogger<AuthController>>();
        var mapper = fixture.ServiceProvider.GetService<IMapper>();

        _authController = new AuthController(_authServicesMock.Object);
    }
    
    [Fact]
    public async Task Register_New_Customer_If_Exist_Return_Conflict()
    {
        // Arrange
        var customer = _fixture.Create<Customer>();
        _authServicesMock.Setup(repos => repos.RegisterCustomerAsync(It.IsAny<RegisterCustomerRequestModel>()))
            .ReturnsAsync(new BaseResponse<RegisterCustomerResponseModel>()
            {
                Code = (int)HttpStatusCode.Conflict,
                Data = new RegisterCustomerResponseModel(),
                Message = It.IsAny<string>()
            });

        // Act
        var result = await _authController.RegisterCustomer(new RegisterCustomerRequestModel()
        {
            CustomerName = customer.CustomerName,
            Contact = customer.Contact,
            Email = customer.Email,
        }) as ObjectResult;

        // Assert
        Assert.Equal(409,result?.StatusCode);
    }
    
    [Fact]
    public async Task Register_Customer_ReturnOk()
    {
        // Arrange
        var customer = _fixture.Create<Customer>();

        _authServicesMock.Setup(repos => repos.RegisterCustomerAsync(It.IsAny<RegisterCustomerRequestModel>()))
            .ReturnsAsync(new BaseResponse<RegisterCustomerResponseModel>()
            {
                Code = (int)HttpStatusCode.Created,
                Data = new RegisterCustomerResponseModel(),
                Message = It.IsAny<string>()

            });

        // Act
        var result = await _authController.RegisterCustomer(new RegisterCustomerRequestModel()
        {
            CustomerName = customer.CustomerName,
            Contact = customer.Contact,
            Email = customer.Email
        }) as ObjectResult;
  
        // Assert
        Assert.Equal(201,result?.StatusCode);
    }
    
    [Fact]
    public async Task Login_Customer_Return_Ok()
    {
        // Arrange
        var customer = _fixture.Create<Customer>();
        
        _authServicesMock.Setup(repos => repos.LoginCustomerAsync(It.IsAny<CustomerLoginRequestModel>()))
            .ReturnsAsync(new BaseResponse<RegisterCustomerResponseModel>()
            {
                Code = (int)HttpStatusCode.OK,
                Data = new RegisterCustomerResponseModel(),
                Message = It.IsAny<string>()
            });
        
        // Act
        var result = await _authController.LoginCustomer(new CustomerLoginRequestModel()
        {
           Email = customer.Email,
           Password = It.IsAny<string>()
        }) as ObjectResult;
  
        // Assert
        Assert.Equal(200,result?.StatusCode);
    }
}

