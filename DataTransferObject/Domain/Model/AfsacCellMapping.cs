using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;

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
