using GodwinStoreAPI.Model.CustomerModel;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace GodwinStoreAPI.Redis;

public class RedisService:IRedisService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisService> _logger;
    private readonly RedisConfig _redisConfig;

    public RedisService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisService>logger,IOptions<RedisConfig> redisConfig)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
        _redisConfig = redisConfig.Value;
    }
    
    public async Task<CachedCustomer> GetCustomerAsync(string customerId)
    {
        try
        {
            string customerKey = RedisConstants.GetCustomerRedisKeyByCustomerId(customerId);
            bool cacheExist = await _connectionMultiplexer.GetDatabase().KeyExistsAsync(customerKey);
            if (cacheExist)
            {
                var customerValue = await _connectionMultiplexer.GetDatabase().StringGetAsync(customerKey);

                return JsonSerializer.Deserialize<CachedCustomer>(customerValue);
            }

            return null;

        }
        catch (Exception e)
        {
           _logger.LogError(e, "An error occured getting customer by customerKey:{customerKey}",customerId);
            return null;
        }
    }

    public async Task<bool> CacheNewCustomerAsync(CachedCustomer cachedCustomer)
    {
        try
        {
            string customerKey = RedisConstants.GetCustomerRedisKeyByCustomerId(cachedCustomer.CustomerId);
            bool cachedSuccessfully = await _connectionMultiplexer.GetDatabase().StringSetAsync(
                key: customerKey,
                value: JsonConvert.SerializeObject(cachedCustomer),
                TimeSpan.FromDays(_redisConfig.DataExpiryDays)
                );
            
            return cachedSuccessfully;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured caching customer by customerKey \n{customerKey}",
                JsonConvert.SerializeObject(cachedCustomer,Formatting.Indented));

            return false;
        }
    }

    public async Task<bool> DeleteCustomerAsync(string customerId)
    {
        try
        {
            string customerKey = RedisConstants.GetCustomerRedisKeyByCustomerId(customerId);
            bool cacheExist = await _connectionMultiplexer.GetDatabase().KeyExistsAsync(customerKey);
            if (cacheExist)
            {
                var deletedSuccessfully = await _connectionMultiplexer.GetDatabase().KeyDeleteAsync(
                    key: customerKey
                );

                return deletedSuccessfully;
            }

            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured deleting customer by customerKey:{customerKey}",customerId);
            return false;
        }
    }
    
}