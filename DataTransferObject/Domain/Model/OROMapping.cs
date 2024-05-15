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
    public class OROMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "OROMappingId is number.")]
        public short OROMappingId { get; set; }

        [Column(TypeName = "varchar(100)")]
        [RegularExpression(@"^[\d\,]+$", ErrorMessage = "ArmedId is number.")]
        public string ArmedIdList { get; set; } = string.Empty;

        [ForeignKey("MRecordOffice"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RecordOfficeId is number.")]
        public byte RecordOfficeId { get; set; }

        [ForeignKey("TrnDomainMapping"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TDMId is number.")]
        public int? TDMId { get; set; }
        public TrnDomainMapping? TrnDomainMapping { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UnitId { get; set; }
        public MapUnit? MapUnit { get; set; }

    }
}
