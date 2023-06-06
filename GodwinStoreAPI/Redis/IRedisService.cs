using GodwinStoreAPI.Model.CustomerModel;
using GodwinStoreAPI.Model.CustomerModel.ResponseModel;

namespace GodwinStoreAPI.Redis;

public interface IRedisService
{
    Task<CachedCustomer> GetCustomerAsync(string customerId);
    Task<bool> CacheNewCustomerAsync(CachedCustomer cachedCustomer);
    Task<bool> DeleteCustomerAsync(string customerId);
}