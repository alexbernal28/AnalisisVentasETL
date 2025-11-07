using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Persistence.Destination.Repositories;
using AnalisisVentasETL.Persistence.Sources.API.Repositories;
using AnalisisVentasETL.Persistence.Sources.CSV.Repositories;
using AnalisisVentasETL.Persistence.Sources.Db.Context;
using AnalisisVentasETL.Persistence.Sources.Db.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AnalisisVentasETL.WRKVentas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddDbContext<TransactionalDBContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(builder.Configuration.GetConnectionString("TransactionalDB") ?? string.Empty);
            });

            builder.Services.AddScoped<IDatabaseExtractor, DatabaseExtractor>();

            builder.Services.AddHttpClient<IAPIExtractor, APIExtractor>();

            builder.Services.AddDbContext<DwhDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataWarehouse") ?? string.Empty);
            });

            builder.Services.AddScoped(typeof(IDataLoader<>), typeof(DataLoader<>));

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IDatabaseExtractor, DatabaseExtractor>();
            builder.Services.AddHttpClient<IAPIExtractor, APIExtractor>();
            builder.Services.AddScoped<IDimProductRepository, DimProductRepository>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}