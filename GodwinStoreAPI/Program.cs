using System.Reflection;
using AutoMapper;
using GodwinStoreAPI.Data.DbContexts;
using GodwinStoreAPI.ElasticSearch;
using GodwinStoreAPI.Helper;
using GodwinStoreAPI.Redis;
using GodwinStoreAPI.Services.AuthServices;
using GodwinStoreAPI.Services.CustomerServices;
using GodwinStoreAPI.Services.OrderServices;
using GodwinStoreAPI.Services.ProductServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionDb")));
builder.Services.AddScoped<IProductServices,ProductServices>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<CustomerContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionDb")));
builder.Services.AddScoped<ICustomerServices, CustomerServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();

builder.Services.AddDbContext<OrderContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionDb")));
builder.Services.AddScoped<IOrderServices, OrderServices>();

builder.Services.AddRedisCache(c => builder.Configuration.GetSection(nameof(RedisConfig)).Bind(c));

builder.Services.AddElasticSearch(c=>builder.Configuration.GetSection(nameof(ElasticSearchConfig)).Bind(c));

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1",new OpenApiInfo()
    {
        Title = "Godwin Store Api",
        Version = "v1",
        Description = "Godwin Store Api",
        Contact = new OpenApiContact
        {
            Name = "Godwin Mensah",
            Email = "godwinmensah945@gmail.com"
        }
    });
    x.ResolveConflictingActions(resolver=>resolver.First());
    x.EnableAnnotations();

    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    x.IncludeXmlComments(xmlPath);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    app.UseSwaggerUI(x=>x.SwaggerEndpoint("/swagger/v1/swagger.json","Godwin Store Api"));
}

app.UseAuthorization();
 
app.UseHttpsRedirection();

app.MapControllers();

app.Run();