using AutoMapper;
using GodwinStoreAPI.Services.AuthServices;
using GodwinStoreAPI.Services.CustomerServices;
using GodwinStoreAPI.Services.OrderServices;
using GodwinStoreAPI.Services.ProductServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestStore.UnitTest.TestSetup;

public class TestFixture
{
    public ServiceProvider ServiceProvider { get; }
    
    public TestFixture()
    {
        var services = new ServiceCollection();
        ConfigurationManager.SetupConfiguration();

        services.AddSingleton(sp => ConfigurationManager.Configuration);

        services.AddLogging(x => x.AddConsole());
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<IProductServices, ProductServices>();
        services.AddScoped<ICustomerServices, CustomerServices>();
        services.AddScoped<IOrderServices, OrderServices>();
        services.AddScoped<IAuthServices, AuthServices>();
        
        ServiceProvider = services.BuildServiceProvider();
    }
}