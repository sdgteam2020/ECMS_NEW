using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain
{
    public class MUserProfile:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string ArmyNo { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey("MRank")]
        public short RankId { get; set; }
        public MRank? MRank { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = string.Empty;

        
        [ForeignKey("MAppointment"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public byte ApptId { get; set; }
        public MAppointment? MAppointment { get; set; }
        // [Required]
        //public int UnitId { get; set; }
        public Boolean IntOffr { get; set; }
        //public MUnit? MUnit { get; set; }
       
    }
}
