using AnalisisVentasETL.Domain.Entities.Db;
using Microsoft.EntityFrameworkCore;

namespace AnalisisVentasETL.Persistence.Sources.Db.Context
{
    public class TransactionalDBContext : DbContext
    {
        public TransactionalDBContext(DbContextOptions<TransactionalDBContext> options) : base(options)
        {
        }
        public DbSet<ProductDB> Products { get; set; } = null!;
        public DbSet<CustomerDB> Customers { get; set; } = null!;
        public DbSet<OrderDB> Orders { get; set; } = null!;
        public DbSet<OrderDetailDB> OrderDetails { get; set; } = null!;
    }
}
