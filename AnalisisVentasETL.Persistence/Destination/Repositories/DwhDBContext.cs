using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using AnalisisVentasETL.Domain.Entities.Dwh.Facts;
using Microsoft.EntityFrameworkCore;

namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public class DwhDBContext : DbContext
    {
        public DwhDBContext(DbContextOptions<DwhDBContext> options) : base(options)
        {
        }

        public DbSet<DimProduct> DimProducts { get; set; }
        public DbSet<DimCustomer> DimCustomers { get; set; }
        public DbSet<DimTime> DimTimes { get; set; }
        public DbSet<DimDataSource> DimDataSources { get; set; }
        public DbSet<FactSales> FactSales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DimProduct>()
                .Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<FactSales>()
                .Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<FactSales>()
                .Property(x => x.Total)
                .HasColumnType("decimal(18,2)");
        }

    }
}
