using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Persistence.Sources.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnalisisVentasETL.Persistence.Sources.Db.Repositories
{
    public class DatabaseExtractor : IDatabaseExtractor
    {
        private readonly TransactionalDBContext _context;
        private readonly ILogger<DatabaseExtractor> _logger;
        public DatabaseExtractor(TransactionalDBContext context, ILogger<DatabaseExtractor> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            _logger.LogInformation("Extracting data from database for entity {Entity}", typeof(T).Name);
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }
    }
}
