using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public class DimProductRepository : IDimProductRepository
    {
        private readonly DwhDBContext _context;
        private readonly ILogger<DimProductRepository> _logger;

        public DimProductRepository(DwhDBContext context, ILogger<DimProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task BulkInsertAsync(IEnumerable<DimProduct> products)
        {
            _context.DimProducts.RemoveRange(_context.DimProducts);

            await _context.DimProducts.AddRangeAsync(products);

            // Aumentar timeout
            _context.Database.SetCommandTimeout(300);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(Expression<Func<DimProduct, bool>> filter)
        {
            return await _context.DimProducts.AnyAsync(filter);
        }

        public async Task<List<DimProduct>> GetAll()
        {
            var products = await _context.DimProducts.AsNoTracking().ToListAsync();
            return products;
        }

        public async Task RemoveAll(DimProduct[] entities)
        {
            _context.DimProducts.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            try
            {
                _context.Database.SetCommandTimeout(120);
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[DimProduct]");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar DimProduct");
                throw;
            }
        }

        public async Task SaveAll(DimProduct[] entities)
        {
            try
            {
                // LOTES MÁS PEQUEÑOS: 20 en lugar de 100
                const int batchSize = 20;
                int totalBatches = (int)Math.Ceiling(entities.Length / (double)batchSize);

                _logger.LogInformation("Total de lotes a procesar: {TotalBatches}", totalBatches);

                // Aumentar timeout
                _context.Database.SetCommandTimeout(180);

                for (int i = 0; i < entities.Length; i += batchSize)
                {
                    var batch = entities.Skip(i).Take(batchSize).ToArray();

                    _logger.LogInformation("[Lote {CurrentBatch}/{TotalBatches}] Insertando {Count} productos (registro {From} a {To})...",
                        (i / batchSize) + 1,
                        totalBatches,
                        batch.Length,
                        i + 1,
                        Math.Min(i + batchSize, entities.Length));

                    await _context.DimProducts.AddRangeAsync(batch);

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Lote guardado exitosamente");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al guardar lote. Intentando con lotes más pequeños...");

                        // Si falla, intentar de uno en uno
                        _context.ChangeTracker.Clear();
                        foreach (var product in batch)
                        {
                            try
                            {
                                await _context.DimProducts.AddAsync(product);
                                await _context.SaveChangesAsync();
                                _context.ChangeTracker.Clear();
                            }
                            catch (Exception innerEx)
                            {
                                _logger.LogError(innerEx, "Error al insertar producto {ProductId}: {Name}",
                                    product.ProductID, product.Name);
                            }
                        }
                    }

                    // Limpiar el tracking para liberar memoria
                    _context.ChangeTracker.Clear();

                    // Pequeña pausa para no saturar la base de datos
                    await Task.Delay(50);
                }

                _logger.LogInformation("✓✓✓ SaveAll completado: {Count} productos guardados", entities.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error crítico en SaveAll");
                throw;
            }
        }
    }
}
