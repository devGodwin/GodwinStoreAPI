using Elasticsearch.Net;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.ElasticSearch;
using GodwinStoreAPI.Redis;
using Nest;
using StackExchange.Redis;
using Order = GodwinStoreAPI.Data.StoreData.Order;

namespace GodwinStoreAPI.Helper;

public static class ServiceExtensions
{
    public static void AddRedisCache(this IServiceCollection services, Action<RedisConfig> redisConfig)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        services.Configure(redisConfig);

        var redisConfiguration = new RedisConfig();
        redisConfig.Invoke(redisConfiguration);
        
        var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            EndPoints = { redisConfiguration.BaseUrl },
            AllowAdmin = true,
            AbortOnConnectFail = false,
            ReconnectRetryPolicy = new LinearRetry(500),
            DefaultDatabase = redisConfiguration.Database
        });

        services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
        services.AddSingleton<IRedisService, RedisService>();
    }

    public static void AddElasticSearch(this IServiceCollection services,
        Action<ElasticSearchConfig> elasticSearchConfig)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.Configure(elasticSearchConfig);

        var elasticSearchConfiguration = new ElasticSearchConfig();
        
        elasticSearchConfig.Invoke(elasticSearchConfiguration);
        
        var pool = new SingleNodeConnectionPool(new Uri(elasticSearchConfiguration.BaseUrl));
        var connectionSettings = new ConnectionSettings(pool);//.DefaultIndex(elasticSearchConfiguration.CustomerIndex);
        
        connectionSettings.DefaultMappingFor<Customer>(m => m.IndexName(elasticSearchConfiguration.CustomerIndex));
        connectionSettings.DefaultMappingFor<Order>(m => m.IndexName(elasticSearchConfiguration.OrderIndex));
        connectionSettings.DefaultMappingFor<Product>(m => m.IndexName(elasticSearchConfiguration.ProductIndex));
            
        connectionSettings.PrettyJson();
        connectionSettings.DisableDirectStreaming();

        var elasticClient = new ElasticClient(connectionSettings);
        var elasticSearchLowLevelClient = new ElasticLowLevelClient(connectionSettings);

        services.AddSingleton<IElasticClient>(elasticClient);
        services.AddSingleton<IElasticLowLevelClient>(elasticSearchLowLevelClient);
        services.AddSingleton<IElasticSearchService, ElasticSearchService>();

    }
}