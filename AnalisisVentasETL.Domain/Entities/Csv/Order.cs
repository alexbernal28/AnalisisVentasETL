using CsvHelper.Configuration.Attributes;

namespace AnalisisVentasETL.Domain.Entities.Csv
{
    public class Order
    {
        [Name("OrderID")]
        public int OrderId { get; set; }

        [Name("CustomerID")]
        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
