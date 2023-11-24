using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class TrnFwd : Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrnFwdId { get; set; }
        public int BasicDetailId { get; set; }
        public BasicDetail? BasicDetail { get; set; }
        public int FromProfileId { get; set; }
        public bool FlagSUS { get; set; } = false;
        public int SusNo { get; set; }
        public int? ToProfileId { get; set; }
        public string? ActionRemark { get; set; } = string.Empty;
        public bool FlagAccepted { get; set; } = false;
        public byte StatusLevel { get; set; }
        public DateTime? CancelDate { get; set; }
    }
}
