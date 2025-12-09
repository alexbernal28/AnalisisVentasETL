using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalisisVentasETL.Domain.Entities.Dwh.Facts
{
    [Table("FactSales", Schema = "Fact")]
    public class FactSales
    {
        [Key]
        public int SaleKey { get; set; }
        public int TimeKey { get; set; }
        public int CustomerKey { get; set; }
        public int ProductKey { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string? SalesChannel { get; set; }
        public int SourceID { get; set; }
    }
}
