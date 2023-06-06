namespace GodwinStoreAPI.Redis;

public static class RedisConstants
{
    private const string CustomerKeyByCustomerId = "GodwinStoreAPI:customer:{customerId}";

    public static string GetCustomerRedisKeyByCustomerId(string customerId) =>
        CustomerKeyByCustomerId.Replace("{customerId}", customerId);
}