using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class ProfileData : Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProfileDataId { get; set; }
        public string? ArmyNumber { get; set; }
        public string? ArmyNumberPart1 { get; set; }
        public int? ArmyNumberPart2 { get; set; }
        public string? ArmyNumberPart3 { get; set; }
        public string? Rank { get; set; }
        public string? Name { get; set; }
        public string? Appointment { get; set; }
        public string? DomainId { get; set; }
        public string? UnitSusNo { get; set; }
        public int? UnitSusNoPart1 { get; set; }
        public string? UnitSusNoPart2 { get; set; }
        public string? UnitName { get; set; }
        public string? TypeOfUnit { get; set; }
        public string? Comd { get; set; }
        public string? Corps { get; set; }
        public string? Div { get; set; }
        public string? Bde { get; set; }
        public string? InitiatingOfficerArmyNumber { get; set; }
        public string? IOArmyNumberPart1 { get; set; }
        public int? IOArmyNumberPart2 { get; set; }
        public string? IOArmyNumberPart3 { get; set; }
        public string? IORank { get; set; }
        public string? IOName { get; set; }
        public string? IOAppointment { get; set; }
        public string? IOUnitFormation { get; set; }
        public string? GISOfficerArmyNumber { get; set; }
        public string? GISArmyNumberPart1 { get; set; }
        public int? GISArmyNumberPart2 { get; set; }
        public string? GISArmyNumberPart3 { get; set; }
        public string? GISRank { get; set; }
        public string? GISName { get; set; }
        public string? GISAppointment { get; set; }
        public string? GISUnitFormation { get; set; }
        public bool IsSubmit { get; set; }
    }
}
