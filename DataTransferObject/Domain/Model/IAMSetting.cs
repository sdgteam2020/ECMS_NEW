using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class IAMSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte IAMSettingId { get; set; }
        public bool WithIAMLogin { get; set; }
        public bool DebugWithIAM { get; set; }
        public byte LocalHostActive { get; set; }
        public string? HardSAMLResonoce { get; set; }
    }
}
