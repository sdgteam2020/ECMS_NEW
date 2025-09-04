using DataTransferObject.Domain.Master;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class AfsacCellMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "AfsacCellMappingId is number.")]
        public short AfsacCellMappingId { get; set; }

        [ForeignKey("TrnDomainMapping"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TDMId is number.")]
        public int? TDMId { get; set; }
        public TrnDomainMapping? TrnDomainMapping { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UnitId { get; set; }
        public MapUnit? MapUnit { get; set; }
    }
}
