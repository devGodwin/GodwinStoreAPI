using GodwinStoreAPI.Data.StoreData;
using Microsoft.EntityFrameworkCore;

namespace GodwinStoreAPI.Data.DbContexts;

public class ProductContext:DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options)
        :base(options)
    {
    }
    public DbSet<Product> Product { get; set; }
}
