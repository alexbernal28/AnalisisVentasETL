using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalisisVentasETL.Domain.Entities.Dwh.Dimensions
{
    [Table("DimDataSource", Schema = "Dimension")]
    public class DimDataSource
    {
        [Key]
        public int SourceID { get; set; }

        [StringLength(30)]
        public string SourceType { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UploadDate { get; set; }
    }
}
