using GodwinStoreAPI.Data.StoreData;
using Microsoft.EntityFrameworkCore;

namespace GodwinStoreAPI.Data.DbContexts;

public class OrderContext:DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options)
        :base(options)
    {
    }
    public DbSet<Order> Order { get; set; }
}