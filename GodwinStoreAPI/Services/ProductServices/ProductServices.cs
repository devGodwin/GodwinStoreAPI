using System.Net;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using GodwinStoreAPI.Data.DbContexts;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.ElasticSearch;
using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Model.ProductModel.RequestModel;
using GodwinStoreAPI.Model.ProductModel.ResponseModel;
using GodwinStoreAPI.Model.Responses;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GodwinStoreAPI.Services.ProductServices;

public class ProductServices:IProductServices
{
    private readonly ProductContext _productContext;
    private readonly ILogger<ProductServices> _logger;
    private readonly IMapper _mapper;
    private readonly IElasticSearchService _elasticSearchService;

    public ProductServices(ProductContext productContext, ILogger<ProductServices> logger,IMapper mapper,
        IElasticSearchService elasticSearchService)
    {
        _productContext = productContext;
        _logger = logger;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<BaseResponse<ProductResponseModel>> CreateProductAsync(ProductRequestModel requestModel)
    {
        try
        { 
            var productExist = await _productContext.Product.AnyAsync(x => x.ProductName.Equals(requestModel.ProductName)); 
            if (productExist) 
            { 
                return CommonResponses.ErrorResponse.ConflictErrorResponse<ProductResponseModel>("Product is already added"); 
            }
            
            var newProduct = _mapper.Map<Product>(requestModel); 
            await _productContext.Product.AddAsync(newProduct); 
            
            var rows = await _productContext.SaveChangesAsync();
            
            if (rows < 1) 
            { 
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<ProductResponseModel>(); 
            }
            
            await _elasticSearchService.AddAsync(newProduct);
            
            return CommonResponses.SuccessResponse.CreatedResponse(_mapper.Map<ProductResponseModel>(newProduct));
            
        }
        catch (Exception e) 
        { 
            _logger.LogError(e,"An error occured creating product\n{requestModel}",JsonConvert.SerializeObject(requestModel,Formatting.Indented)); 
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<ProductResponseModel>(); 
        }
    }

    public async Task<BaseResponse<PaginatedResponse<ProductResponseModel>>> GetProductsAsync(
        ProductFilter productFilter)
    {
        try
        {
            var productQueryable = _productContext.Product.AsNoTracking().AsQueryable();
        
            if (!string.IsNullOrEmpty(productFilter.ProductId)) 
            { 
                productQueryable = productQueryable.Where(x => x.ProductId.Equals(productFilter.ProductId)); 
            } 
               
            if (!string.IsNullOrEmpty(productFilter.ProductName)) 
            { 
                productQueryable = productQueryable.Where(x => x.ProductName.Equals(productFilter.ProductName)); 
            }
               
            if (!string.IsNullOrEmpty(productFilter.Description)) 
            { 
                productQueryable = productQueryable.Where(x => x.Description.Equals(productFilter.Description)); 
            }

            productQueryable = "desc".Equals(productFilter.OrderBy, StringComparison.OrdinalIgnoreCase) 
                ? productQueryable.OrderByDescending(x=>x.CreatedAt) 
                : productQueryable.OrderBy(x=>x.CreatedAt);
        
            var paginatedResponse = await productQueryable.ToPagedListAsync(productFilter.CurrentPage - 1, productFilter.PageSize);
        
            return new BaseResponse<PaginatedResponse<ProductResponseModel>>() 
            { 
                Code = (int)HttpStatusCode.OK, 
                Message = "Retrieved successfully", 
                Data = new PaginatedResponse<ProductResponseModel>() 
                { 
                    CurrentPage = productFilter.CurrentPage, 
                    TotalPages = paginatedResponse.TotalPages,
                    PageSize = paginatedResponse.PageSize,
                    TotalRecords = paginatedResponse.TotalCount,
                    Data = paginatedResponse.Items.Select(x => _mapper.Map<ProductResponseModel>(x)).ToList() 
                } 
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured getting products\n{productFilter}",JsonConvert.SerializeObject(productFilter,Formatting.Indented));
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<PaginatedResponse<ProductResponseModel>>();
        } 
    }

    public async Task<BaseResponse<ProductResponseModel>> GetProductByIdAsync(string productId)
    { 
        var product = await _productContext.Product.FirstOrDefaultAsync(x => x.ProductId.Equals(productId)); 
        if (product == null) 
        { 
            return CommonResponses.ErrorResponse.NotFoundErrorResponse<ProductResponseModel>("Product not found"); 
        }
        
        await _elasticSearchService.GetByIdAsync<ProductResponseModel>(productId);

        
        return CommonResponses.SuccessResponse.OkResponse(_mapper.Map<ProductResponseModel>(product)); 
    }

    public async Task<BaseResponse<ProductUpdateModel>> UpdateProductAsync(string productId,ProductUpdateModel updateModel)
    { 
        try 
        { 
            var product = await _productContext.Product.FirstOrDefaultAsync(x => x.ProductId.Equals(productId));
            if (product == null)
            {
                return CommonResponses.ErrorResponse.NotFoundErrorResponse<ProductUpdateModel>("Product not found");
            }
            
            var updateProduct = _mapper.Map(updateModel,product); 
            _productContext.Product.Update(updateProduct); 
            
            var rows = await _productContext.SaveChangesAsync(); 
            if (rows < 1) 
            { 
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<ProductUpdateModel>(); 
            }
            
            await _elasticSearchService.UpdateAsync(updateProduct.ProductId, updateProduct);

            
            return CommonResponses.SuccessResponse.CreatedResponse(_mapper.Map<ProductUpdateModel>(product));
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured updating product\n{id}",JsonConvert.SerializeObject(productId,Formatting.Indented));
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<ProductUpdateModel>();
        }
    }

    public async Task<BaseResponse<EmptyResponse>> DeleteProductAsync(string productId)
    {
        try
        {
            var productExist = await _productContext.Product.FirstOrDefaultAsync(x => x.ProductId.Equals(productId)); 
            if (productExist == null)
            {
                return CommonResponses.ErrorResponse.NotFoundErrorResponse<EmptyResponse>("Product not found");
            }
            
            _productContext.Product.Remove(productExist);
            
            var rows = await _productContext.SaveChangesAsync();
            if (rows < 1 )
            {
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<EmptyResponse>();
            }
            
            await _elasticSearchService.DeleteAsync<EmptyResponse>(productId);

            return CommonResponses.SuccessResponse.DeletedResponse();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured deleting product\n{id}",JsonConvert.SerializeObject(productId,Formatting.Indented));
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<EmptyResponse>();
        }
    }
}