using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalisisVentasETL.Domain.Entities.Dwh.Dimensions
{
    [Table("DimProduct", Schema = "Dimension")]
    public class DimProduct
    {
        [Key]
        public int ProductKey { get; set; }
        public int ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }
        public DateTime UploadDate { get; set; } 
    }
}
