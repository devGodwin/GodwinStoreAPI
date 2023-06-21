using AutoMapper;
using GodwinStoreAPI.Data.DbContexts;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.ElasticSearch;
using GodwinStoreAPI.Helper;
using GodwinStoreAPI.Model.CustomerModel;
using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Model.CustomerModel.ResponseModel;
using GodwinStoreAPI.Model.Responses;
using GodwinStoreAPI.Redis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GodwinStoreAPI.Services.AuthServices;

public class AuthServices:IAuthServices
{
    private readonly CustomerContext _customerContext;
    private readonly IRedisService _redisService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerServices.CustomerServices> _logger;
    private readonly IElasticSearchService _elasticSearchService;

    public AuthServices(CustomerContext customerContext, IRedisService redisService, IMapper mapper,
        ILogger<CustomerServices.CustomerServices> logger, IElasticSearchService elasticSearchService)
    {
        _customerContext = customerContext;
        _redisService = redisService;
        _mapper = mapper;
        _logger = logger;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<BaseResponse<RegisterCustomerResponseModel>> RegisterCustomerAsync(RegisterCustomerRequestModel registerCustomer)
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
}