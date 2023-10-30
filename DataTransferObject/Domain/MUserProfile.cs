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
    public class MUserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        public string ArmyNo { get; set; }
        public string ArmyNo_Old { get; set; }

        public int RankId { get; set; }
        public string Name { get; set; }

        [ForeignKey("MAppointment")]
        public int ApptId { get; set; }
        public MAppointment? MAppointment { get; set; }

        // public string? DomainId { get; set; }
        public int TypeOfUnit { get; set; }
        public int UnitId { get; set; }
        public MUnit? MUnit { get; set; }


        [ForeignKey("Comd")]
        public int ComdId { get; set; }

        public Comd? Comd { get; set; }

        [ForeignKey("MCorps")]
        public int CorpsId { get; set; }
        public MCorps? Corps { get; set; }

        [ForeignKey("MDiv")]

        [Required(ErrorMessage = "required!")]

        public int DivId { get; set; }
        public MDiv? Div { get; set; }

        [ForeignKey("MBde")]

        [Required(ErrorMessage = "required!")]
        public int BdeId { get; set; }
        public MBde? Bde { get; set; }

        public string? InitOffrsArmyNo { get; set; }
        public string? IOArmyNo { get; set; }
        //public int IORankId { get; set; }
        //public string? IOName { get; set; }
        //public string? IOAppointment { get; set; }
        //public string? IOUnitFormation { get; set; }
        public string? GISOfficerArmyNo { get; set; }
        //public string? GISArmyNumberPart1 { get; set; }
        //public int? GISArmyNumberPart2 { get; set; }
        //public string? GISArmyNumberPart3 { get; set; }
        //public string? GISRank { get; set; }
        //public string? GISName { get; set; }
        //public string? GISAppointment { get; set; }
        //public string? GISUnitFormation { get; set; }
        public bool IsSubmit { get; set; }
    }
}
