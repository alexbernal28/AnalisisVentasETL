using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalisisVentasETL.Domain.Entities.Dwh.Dimensions
{
    [Table("DimCustomer", Schema = "Dimension")]
    public class DimCustomer
    {
        [Key]
        public int CustomerKey { get; set; }

        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string? CustomerType { get; set; }
        public string? Email { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? Region { get; set; }
        public DateTime? UploadAt { get; set; }
    }
}
