using GodwinStoreAPI.Data.StoreData;
using Microsoft.EntityFrameworkCore;

namespace GodwinStoreAPI.Data.DbContexts;

public class CustomerContext:DbContext
{
    public CustomerContext(DbContextOptions<CustomerContext> options)
        :base(options)
    {
    }
    public DbSet<Customer> Customer { get; set; }
}