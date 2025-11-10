using System.ComponentModel.DataAnnotations.Schema;

namespace AnalisisVentasETL.Domain.Entities.Db
{
    [Table("Order_Details")]
    public class OrderDetailDB
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
