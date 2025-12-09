using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Db;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using System.Globalization;

namespace AnalisisVentasETL.WRKVentas.Loaders
{
    public class DimTimeLoader
    {
        private readonly IOrderRepository _csvRepository;
        private readonly IDatabaseExtractor _dbRepository;
        private readonly IDimTimeRepository _dimTimeRepository;
        private readonly ILogger<DimTimeLoader> _logger;

        public DimTimeLoader(
            IOrderRepository csvRepository,
            IDatabaseExtractor dbRepository,
            IDimTimeRepository dimTimeRepository,
            ILogger<DimTimeLoader> logger)
        {
            _csvRepository = csvRepository;
            _dbRepository = dbRepository;
            _dimTimeRepository = dimTimeRepository;
            _logger = logger;
        }

        public async Task LoadAsync()
        {
            _logger.LogInformation("Cargando DimTime...");

            await _dimTimeRepository.DeleteAll();

            var csvOrders = await _csvRepository.GetAll();
            var dbOrders = await _dbRepository.GetAllAsync<OrderDB>();

            var allDates = csvOrders.Select(o => o.OrderDate)
                .Concat(dbOrders.Select(o => o.OrderDate))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            var dimTimes = allDates.Select(date => new DimTime
            {
                Date = date,
                Day = date.Day,
                Month = date.Month,
                Year = date.Year,
                Quarter = (date.Month - 1) / 3 + 1,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month),
                TimeKey = date.Year * 10000 + date.Month * 100 + date.Day
            }).ToArray();

            await _dimTimeRepository.SaveAll(dimTimes);

            _logger.LogInformation("DimTime cargado correctamente ({Count} fechas).", dimTimes.Length);
        }
    }
}
