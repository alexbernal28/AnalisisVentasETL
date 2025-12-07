using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Db;
using AnalisisVentasETL.Domain.Entities.Dwh.Facts;

namespace AnalisisVentasETL.WRKVentas.Loaders
{
    public class FactSalesLoader
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IDatabaseExtractor _databaseExtractor;
        private readonly IFactSalesRepository _factSalesRepository;
        private readonly IDimCustomerRepository _dimCustomerRepository;
        private readonly IDimProductRepository _dimProductRepository;
        private readonly IDimTimeRepository _dimTimeRepository;
        private readonly IDimDataSourceRepository _dimDataSourceRepository;
        private readonly ILogger<FactSalesLoader> _logger;

        public FactSalesLoader(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IDatabaseExtractor databaseExtractor,
            IFactSalesRepository factSalesRepository,
            IDimCustomerRepository dimCustomerRepository,
            IDimProductRepository dimProductRepository,
            IDimTimeRepository dimTimeRepository,
            IDimDataSourceRepository dimDataSourceRepository,
            ILogger<FactSalesLoader> logger)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _databaseExtractor = databaseExtractor;
            _factSalesRepository = factSalesRepository;
            _dimCustomerRepository = dimCustomerRepository;
            _dimProductRepository = dimProductRepository;
            _dimTimeRepository = dimTimeRepository;
            _dimDataSourceRepository = dimDataSourceRepository;
            _logger = logger;
        }

        public async Task LoadAsync()
        {
            _logger.LogInformation("Iniciando carga de FactSales...");

            // 1. LIMPIAR TABLA ANTES DE CARGAR
            await _factSalesRepository.DeleteAll();

            // 2. CARGAR DIMENSIONES EN MEMORIA PARA LOOKUPS
            _logger.LogInformation("Cargando dimensiones para lookups...");
            var dimCustomers = await _dimCustomerRepository.GetAll();
            var dimProducts = await _dimProductRepository.GetAll();
            var dimTimes = await _dimTimeRepository.GetAll();
            var dimDataSources = await _dimDataSourceRepository.GetAll();

            // Crear diccionarios para búsquedas rápidas
            var customerLookup = dimCustomers.ToDictionary(c => c.CustomerId, c => c.CustomerKey);
            var productLookup = dimProducts.ToDictionary(p => p.ProductID, p => p.ProductKey);
            var timeLookup = dimTimes.ToDictionary(
                t => new DateTime(t.Year, t.Month, t.Day).Date,
                t => t.TimeKey
            );
            var sourceLookup = dimDataSources.ToDictionary(s => s.SourceType, s => s.SourceID);

            _logger.LogInformation("Dimensiones cargadas: {Customers} clientes, {Products} productos, {Times} fechas, {Sources} fuentes",
                customerLookup.Count, productLookup.Count, timeLookup.Count, sourceLookup.Count);

            // 3. EXTRAER DATOS DE CSV
            _logger.LogInformation("Extrayendo datos de CSV...");
            var csvOrders = await _orderRepository.GetAll();
            var csvOrderDetails = await _orderDetailRepository.GetAll();

            var csvSales = await ProcessSalesFromSource(
                csvOrders.Select(o => (o.OrderId, o.CustomerId, o.OrderDate)).ToList(),
                csvOrderDetails.Select(od => (od.OrderID, od.ProductID, od.Quantity, od.TotalPrice)).ToList(),
                customerLookup,
                productLookup,
                timeLookup,
                sourceLookup["CSV"],
                "CSV"
            );

            // 4. EXTRAER DATOS DE BASE DE DATOS
            _logger.LogInformation("Extrayendo datos de base de datos transaccional...");
            var dbOrders = await _databaseExtractor.GetAllAsync<OrderDB>();
            var dbOrderDetails = await _databaseExtractor.GetAllAsync<OrderDetailDB>();

            var dbSales = await ProcessSalesFromSource(
                dbOrders.Select(o => (o.OrderId, o.CustomerId, o.OrderDate)).ToList(),
                dbOrderDetails.Select(od => (od.OrderID, od.ProductID, od.Quantity, od.TotalPrice)).ToList(),
                customerLookup,
                productLookup,
                timeLookup,
                sourceLookup["Database"],
                "Database"
            );

            // 5. CONSOLIDAR Y CARGAR
            var allSales = csvSales.Concat(dbSales).ToList();

            _logger.LogInformation("Total de registros a cargar: {Count}", allSales.Count);
            _logger.LogInformation("- Desde CSV: {CsvCount}", csvSales.Count);
            _logger.LogInformation("- Desde DB: {DbCount}", dbSales.Count);

            if (allSales.Any())
            {
                await _factSalesRepository.BulkInsertAsync(allSales);
                _logger.LogInformation("FactSales cargado exitosamente con {Count} registros.", allSales.Count);
            }
            else
            {
                _logger.LogWarning("No se encontraron registros para cargar en FactSales.");
            }

            _logger.LogInformation("========================================");
        }

        private async Task<List<FactSales>> ProcessSalesFromSource(
            List<(int OrderId, int CustomerId, DateTime OrderDate)> orders,
            List<(int OrderID, int ProductID, int Quantity, decimal TotalPrice)> orderDetails,
            Dictionary<int, int> customerLookup,
            Dictionary<int, int> productLookup,
            Dictionary<DateTime, int> timeLookup,
            int sourceId,
            string sourceName)
        {
            var sales = new List<FactSales>();
            int skippedRecords = 0;

            // Unir órdenes con detalles
            var salesData = from order in orders
                            join detail in orderDetails on order.OrderId equals detail.OrderID
                            select new
                            {
                                order.OrderId,
                                order.CustomerId,
                                order.OrderDate,
                                detail.ProductID,
                                detail.Quantity,
                                detail.TotalPrice
                            };

            foreach (var sale in salesData)
            {
                // Validar que existan las claves en las dimensiones
                if (!customerLookup.TryGetValue(sale.CustomerId, out var customerKey))
                {
                    _logger.LogWarning("Cliente {CustomerId} no encontrado en DimCustomer. Registro omitido.", sale.CustomerId);
                    skippedRecords++;
                    continue;
                }

                if (!productLookup.TryGetValue(sale.ProductID, out var productKey))
                {
                    _logger.LogWarning("Producto {ProductId} no encontrado en DimProduct. Registro omitido.", sale.ProductID);
                    skippedRecords++;
                    continue;
                }

                var orderDateOnly = sale.OrderDate.Date;
                if (!timeLookup.TryGetValue(orderDateOnly, out var timeKey))
                {
                    _logger.LogWarning("Fecha {Date} no encontrada en DimTime. Registro omitido.", orderDateOnly);
                    skippedRecords++;
                    continue;
                }

                // Calcular precio unitario
                decimal unitPrice = sale.Quantity > 0 ? sale.TotalPrice / sale.Quantity : 0;

                sales.Add(new FactSales
                {
                    TimeKey = timeKey,
                    CustomerKey = customerKey,
                    ProductKey = productKey,
                    SourceID = sourceId,
                    QuantitySold = sale.Quantity,
                    UnitPrice = unitPrice,
                    Total = sale.TotalPrice,
                    SalesChannel = DetermineSalesChannel(sourceName, sale.OrderId)
                });
            }

            if (skippedRecords > 0)
            {
                _logger.LogWarning("{Count} registros omitidos de {Source} por falta de claves foráneas.",
                    skippedRecords, sourceName);
            }

            _logger.LogInformation("Procesados {Count} registros desde {Source}.", sales.Count, sourceName);

            return sales;
        }

        private string DetermineSalesChannel(string source, int orderId)
        {
            if (source == "CSV")
            {
                return orderId % 3 == 0 ? "Online" : "In-Store";
            }
            else if (source == "Database")
            {
                return orderId % 2 == 0 ? "In-Store" : "Online";
            }
            else
            {
                return "Online";
            }
        }
    }
}
