using System.Net;
using AutoFixture;
using GodwinStoreAPI.Controllers;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Model.ProductModel.RequestModel;
using GodwinStoreAPI.Model.ProductModel.ResponseModel;
using GodwinStoreAPI.Model.Responses;
using GodwinStoreAPI.Services.ProductServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestStore.UnitTest.TestSetup;
using Xunit;

namespace TestStore.UnitTest;

public class ProductControllerTests:IClassFixture<TestFixture>
{
    private readonly ProductController _productController;
    private readonly Mock<IProductServices> _productServicesMock = new Mock<IProductServices>();
    private readonly Fixture _fixture = new Fixture();

    public ProductControllerTests(TestFixture fixture)
    {
        _productController = new ProductController(_productServicesMock.Object);
    }
    
    [Fact]
    public async Task Create_Product_If_Exist_Return_Conflict()
    {
        // Arrange
        var product = _fixture.Create<Product>();

        _productServicesMock.Setup(repos => repos.CreateProductAsync(It.IsAny<ProductRequestModel>()))
            .ReturnsAsync(new BaseResponse<ProductResponseModel>()
            {
                Code = (int)HttpStatusCode.Conflict,
                Data = new ProductResponseModel(),
                Message = It.IsAny<string>()
            });

        // Act
        var result = await _productController.CreateProduct(new ProductRequestModel()
        {
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductName = product.ProductName,
            ProductPrice = product.ProductPrice
        })as ObjectResult;

        // Assert
        Assert.Equal(409,result?.StatusCode);
    }
    
    [Fact]
    public async Task Create_Product_Return_Ok()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        
        _productServicesMock.Setup(repos => repos.CreateProductAsync(It.IsAny<ProductRequestModel>()))
            .ReturnsAsync(new BaseResponse<ProductResponseModel>()
            {
                Code = (int)HttpStatusCode.Created,
                Data = new ProductResponseModel(),
                Message = It.IsAny<string>()
            });

        // Act
        var result = await _productController.CreateProduct(new ProductRequestModel()
        {
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductName = product.ProductName,
            ProductPrice = product.ProductPrice
        }) as ObjectResult;

        // Assert
        Assert.Equal(201,result?.StatusCode);
    }

    [Fact]
    public async Task Filter_Products_Return_Ok()
    {
        // Arrange
        var product = _fixture.Create<PaginatedResponse<ProductResponseModel>>();
        _productServicesMock.Setup(repos => repos.GetProductsAsync(It.IsAny<ProductFilter>()))
            .ReturnsAsync(new BaseResponse <PaginatedResponse<ProductResponseModel>>()
            {
                Code = (int)HttpStatusCode.OK,
                Message = It.IsAny<string>(),
                Data = new PaginatedResponse<ProductResponseModel>()
                {
                    CurrentPage = product.CurrentPage,
                    PageSize = product.PageSize,
                    TotalRecords = product.TotalRecords,
                    TotalPages = product.TotalPages,
                    Data = new List<ProductResponseModel>() 
                }
            });

        // Act
        var result = await _productController.GetProducts(It.IsAny<ProductFilter>()) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }
    
    [Fact]
    public async Task Get_Product_ById_Return_Ok()
    {
        // Arrange
        _productServicesMock.Setup(repos => repos.GetProductByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new BaseResponse<ProductResponseModel>()
            {
                Code = (int)HttpStatusCode.OK,
                Data = new ProductResponseModel(),
                Message = It.IsAny<string>()
                    
            });
        
        // Act
        var result = await _productController.GetProductById(It.IsAny<string>()) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }

    [Fact]
    public async Task Update_Product_If_Null_Return_NotFound()
    {
        // Arrange
        _productServicesMock.Setup(repos => repos.UpdateProductAsync(It.IsAny<string>(),It.IsAny<ProductUpdateModel>()))
            .ReturnsAsync(new BaseResponse<ProductUpdateModel>()
            {
                Code = (int)HttpStatusCode.NotFound,
                Data = new ProductUpdateModel(),
                Message = It.IsAny<string>()
            });

        // Act
        var result = await _productController.UpdateProduct(It.IsAny<string>(),new ProductUpdateModel()) as ObjectResult;

        // Assert
        Assert.Equal(404,result?.StatusCode);
    }
    
    [Fact]
    public async Task Update_Product_Return_Ok()
    {
        // Arrange
        var product = _fixture.Create<Product>();

         _productServicesMock.Setup(repos => repos.UpdateProductAsync(It.IsAny<string>(),It.IsAny<ProductUpdateModel>()))
             .ReturnsAsync(new BaseResponse<ProductUpdateModel>()
             {
                 Code = (int)HttpStatusCode.OK,
                 Data = new ProductUpdateModel(),
                 Message = It.IsAny<string>()
             });

        // Act
        var result = await _productController.UpdateProduct( It.IsAny<string>(),new ProductUpdateModel()
        {
            ProductName = product.ProductName,
            Description = product.Description,
            ProductPrice = product.ProductPrice,
            ImageUrl = product.ImageUrl
        }) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }
    
    [Fact]
    public async Task Delete_Product_If_Null_Return_NotFound()
    {
        // Arrange
        _productServicesMock.Setup(repos => repos.DeleteProductAsync(It.IsAny<string>()))
            .ReturnsAsync(new BaseResponse<EmptyResponse>());

        // Act
        var result = await _productController.DeleteProduct(It.IsAny<string>()) as ObjectResult;

        // Assert
        Assert.Equal(404,result?.StatusCode);
    }
    
    [Fact]
    public async Task Delete_Product_Return_Ok()
    {
        // Arrange
        _productServicesMock.Setup(repos => repos.DeleteProductAsync(It.IsAny<string>()))
            .ReturnsAsync(new BaseResponse<EmptyResponse>()
            {
                Code = (int)HttpStatusCode.OK,
                Data = new EmptyResponse(),
                Message = It.IsAny<string>()
            });
        
        // Act
        var result = await _productController.DeleteProduct(It.IsAny<string>()) as ObjectResult;

        // Assert
        Assert.Equal(200,result?.StatusCode);
    }
    
}