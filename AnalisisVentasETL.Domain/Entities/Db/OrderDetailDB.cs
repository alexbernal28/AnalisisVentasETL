namespace AnalisisVentasETL.Domain.Entities.Db
{
    public class OrderDetailDB
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
