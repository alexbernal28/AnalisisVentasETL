namespace AnalisisVentasETL.Application.Dtos
{
    public class ProductUnifiedDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
