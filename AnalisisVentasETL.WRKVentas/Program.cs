using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Persistence.Destination.Repositories;
using AnalisisVentasETL.Persistence.Sources.API.Repositories;
using AnalisisVentasETL.Persistence.Sources.CSV.Repositories;
using AnalisisVentasETL.Persistence.Sources.Db.Context;
using AnalisisVentasETL.Persistence.Sources.Db.Repositories;
using AnalisisVentasETL.WRKVentas.Loaders;
using Microsoft.EntityFrameworkCore;

namespace AnalisisVentasETL.WRKVentas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var transactionalConn = builder.Configuration.GetConnectionString("TransactionalDB")
                ?? throw new ArgumentNullException("TransactionalDB connection string not configured.");

            var warehouseConn = builder.Configuration.GetConnectionString("DataWarehouse")
                ?? throw new ArgumentNullException("DataWarehouse connection string not configured.");

            // Databases Contexts

            builder.Services.AddDbContext<TransactionalDBContext>(options =>
                options.UseSqlServer(transactionalConn));

            builder.Services.AddDbContext<DwhDBContext>(options =>
                options.UseSqlServer(warehouseConn));

            // Sources Extractors

            builder.Services.AddScoped<IDatabaseExtractor, DatabaseExtractor>();
            builder.Services.AddHttpClient<IAPIExtractor, APIExtractor>();

            // Repositories CSV
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            // Repositories DWH
            builder.Services.AddScoped<IDimProductRepository, DimProductRepository>();
            builder.Services.AddScoped<IDimCustomer, DimCustomerRepository>();
            builder.Services.AddScoped<IDimTime, DimTimeRepository>();
            builder.Services.AddScoped<IDimDataSource, DimDataSourceRepository>();

            // Generic Data Loader
            builder.Services.AddScoped(typeof(IDataLoader<>), typeof(DataLoader<>));

            builder.Services.AddScoped<DimDataSourceLoader>();
            builder.Services.AddScoped<DimProductLoader>();
            builder.Services.AddScoped<DimCustomerLoader>();
            builder.Services.AddScoped<DimTimeLoader>();

            // Worker Service
            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}