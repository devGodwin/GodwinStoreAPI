using System.Net;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using GodwinStoreAPI.Data.DbContexts;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.ElasticSearch;
using GodwinStoreAPI.Helper;
using GodwinStoreAPI.Model.CustomerModel;
using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Model.CustomerModel.ResponseModel;
using GodwinStoreAPI.Model.Filters;
using GodwinStoreAPI.Model.Responses;
using GodwinStoreAPI.Redis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GodwinStoreAPI.Services.CustomerServices;

public class CustomerServices : ICustomerServices
{
    private readonly CustomerContext _customerContext;
    private readonly IRedisService _redisService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerServices> _logger;
    private readonly IElasticSearchService _elasticSearchService;

    public CustomerServices(CustomerContext customerContext, IRedisService redisService, IMapper mapper,
        ILogger<CustomerServices> logger, IElasticSearchService elasticSearchService)
    {
        _customerContext = customerContext;
        _redisService = redisService;
        _mapper = mapper;
        _logger = logger;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<BaseResponse<RegisterCustomerResponseModel>> RegisterCustomerAsync(
        RegisterCustomerRequestModel registerCustomer)
    {
        try
        {
            var customerExist = await _customerContext.Customer.AnyAsync(x => x.Email.Equals(registerCustomer.Email));
            if (customerExist)
            {
                return CommonResponses.ErrorResponse.ConflictErrorResponse<RegisterCustomerResponseModel>(
                    "Customer is already registered");
            }

            Authentication.CreatePasswordHash(registerCustomer.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            var newCustomer = _mapper.Map<Customer>(registerCustomer);
            newCustomer.PasswordHash = passwordHash;
            newCustomer.PasswordSalt = passwordSalt;

            await _customerContext.Customer.AddAsync(newCustomer);
            var rows = await _customerContext.SaveChangesAsync();

            if (rows < 1)
            {
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<RegisterCustomerResponseModel>();
            }
            
            await _redisService.CacheNewCustomerAsync(_mapper.Map<CachedCustomer>(newCustomer));
            
            await _elasticSearchService.AddAsync(newCustomer);

            return CommonResponses.SuccessResponse.CreatedResponse(_mapper.Map<RegisterCustomerResponseModel>(newCustomer));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured registering customer\n{registerCustomer}",
                JsonConvert.SerializeObject(registerCustomer, Formatting.Indented));

            return CommonResponses.ErrorResponse.InternalServerErrorResponse<RegisterCustomerResponseModel>();
        }
    }

    public async Task<BaseResponse<RegisterCustomerResponseModel>> LoginCustomerAsync(CustomerLoginRequestModel requestModel)
    {
        try
        {
            var customer = await _customerContext.Customer.FirstOrDefaultAsync(x => x.Email.Equals(requestModel.Email));
            if (customer == null)
            {
                return CommonResponses.ErrorResponse.BadRequestResponse<RegisterCustomerResponseModel>("Email is incorrect");
            }

            if (!Authentication.VerifyPasswordHash(requestModel.Password, customer.PasswordHash, customer.PasswordSalt))
            {
                return CommonResponses.ErrorResponse.BadRequestResponse<RegisterCustomerResponseModel>(
                    "Password is incorrect");
            }

            return CommonResponses.SuccessResponse.OkResponse(_mapper.Map<RegisterCustomerResponseModel>(customer),
                "Login successful");
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured logging in customer \n{requestModel}",
                JsonConvert.SerializeObject(requestModel, Formatting.Indented));
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<RegisterCustomerResponseModel>();
        }
    }

    public async Task<BaseResponse<PaginatedResponse<RegisterCustomerResponseModel>>> GetCustomersAsync(
        CustomerFilter customerFilter)
    {
        try
        {

            var customerQueryable = _customerContext.Customer.AsNoTracking().AsQueryable();
               
               if (!string.IsNullOrEmpty(customerFilter.CustomerName)) 
               { 
                   customerQueryable = customerQueryable.Where(x => x.CustomerName.Equals(customerFilter.CustomerName)); 
               } 
               
               if (!string.IsNullOrEmpty(customerFilter.Contact)) 
               { 
                   customerQueryable = customerQueryable.Where(x => x.Contact.Equals(customerFilter.Contact)); 
               }
               
               if (!string.IsNullOrEmpty(customerFilter.Email)) 
               { 
                   customerQueryable = 
                       customerQueryable.Where(x => x.Email.ToLower().Equals(customerFilter.Email.ToLower())); 
               }
               
               if (!string.IsNullOrEmpty(customerFilter.CustomerId)) 
               { 
                   customerQueryable = customerQueryable.Where(x => x.CustomerId.Equals(customerFilter.CustomerId)); 
               }
               
               customerQueryable = "desc".Equals(customerFilter.OrderBy, StringComparison.OrdinalIgnoreCase) 
                   ? customerQueryable.OrderByDescending(x=>x.CreatedAt) 
                   : customerQueryable.OrderBy(x=>x.CreatedAt);
               
               var paginatedResponse = await customerQueryable.ToPagedListAsync(customerFilter.CurrentPage - 1, customerFilter.PageSize);

               return new BaseResponse<PaginatedResponse<RegisterCustomerResponseModel>>() 
               { 
                   Code = (int)HttpStatusCode.OK, 
                   Message = "Retrieved successfully", 
                   Data = new PaginatedResponse<RegisterCustomerResponseModel>() 
                   { 
                       CurrentPage = customerFilter.CurrentPage, 
                       TotalPages = paginatedResponse.TotalPages,
                       PageSize = paginatedResponse.PageSize,
                       TotalRecords = paginatedResponse.TotalCount,
                       Data = paginatedResponse.Items.Select(x => _mapper.Map<RegisterCustomerResponseModel>(x)).ToList() 
                   } 
               }; 
           
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured getting customers\n{customerFilter}",JsonConvert.SerializeObject(customerFilter,Formatting.Indented));
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<PaginatedResponse<RegisterCustomerResponseModel>>();
        }
    }

    public async Task<BaseResponse<RegisterCustomerResponseModel>> GetCustomerByIdAsync(string customerId)
    {
        var customerExist = await _customerContext.Customer.FirstOrDefaultAsync(x => x.CustomerId.Equals(customerId)); 
        if (customerExist == null) 
        { 
            return CommonResponses.ErrorResponse.NotFoundErrorResponse<RegisterCustomerResponseModel>("Customer not found"); 
        }
        
        await _redisService.GetCustomerAsync(customerId);

        await _elasticSearchService.GetByIdAsync<RegisterCustomerResponseModel>(customerId);
        
        return CommonResponses.SuccessResponse.OkResponse(_mapper.Map<RegisterCustomerResponseModel>(customerExist));
    }

    public async Task<BaseResponse<RegisterCustomerResponseModel>> UpdateCustomerAsync(string customerId, CustomerUpdateModel updateModel)
    {
        try
        {
            var customerExist = await _customerContext.Customer.FirstOrDefaultAsync(x => x.CustomerId.Equals(customerId));
            if (customerExist == null)
            {
                return CommonResponses.ErrorResponse.NotFoundErrorResponse<RegisterCustomerResponseModel>("Customer not found");
            }
        
            var updateCustomer = _mapper.Map(updateModel, customerExist);
             _customerContext.Customer.Update(updateCustomer);
             
            var rows = await _customerContext.SaveChangesAsync();
            if (rows < 1)
            {
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<RegisterCustomerResponseModel>();
            }

            await _elasticSearchService.UpdateAsync(customerExist.CustomerId, updateCustomer);

            return CommonResponses.SuccessResponse.OkResponse(_mapper.Map<RegisterCustomerResponseModel>(updateCustomer), "Updated successfully");

        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured updating customer\n{updateModel}",JsonConvert.SerializeObject(updateModel,Formatting.Indented));
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<RegisterCustomerResponseModel>();
        }
    }

    public async Task<BaseResponse<EmptyResponse>> DeleteCustomerAsync(string customerId)
    {
        try
        {
            var customerExist = await _customerContext.Customer.FirstOrDefaultAsync(x => x.CustomerId.Equals(customerId));
            if (customerExist == null)
            {
                return CommonResponses.ErrorResponse.NotFoundErrorResponse<EmptyResponse>("Customer not found");
            }

            _customerContext.Customer.Remove(customerExist);
        
            await _redisService.DeleteCustomerAsync(customerId);

            await _elasticSearchService.DeleteAsync<EmptyResponse>(customerId);
        
            var rows = await _customerContext.SaveChangesAsync(); 
            if (rows < 1 ) 
            { 
                return CommonResponses.ErrorResponse.FailedDependencyErrorResponse<EmptyResponse>(); 
            }
        
            return CommonResponses.SuccessResponse.DeletedResponse();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured deleting customer by customerId:{customerId}",customerId);
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<EmptyResponse>();
        }
    }
    
}