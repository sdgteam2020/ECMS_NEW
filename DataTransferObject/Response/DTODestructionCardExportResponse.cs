using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODestructionCardExportResponse
    {
        //public int RequestId { get; set; }
        public int DestructedCardId { get; set; }
        public string ArmyNo { get; set; }
        public string RankAbbreviation { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string RankAndName => $"{RankAbbreviation} {FName} {(string.IsNullOrEmpty(LName) ? string.Empty : LName)}";
        public string Unit { get; set; }
        public DateTime DestructedOn { get; set; }
        public DateTime DateAndTime { get; set; }
        public bool IsActiveBool { get; set; }
        public string IsActive => IsActiveBool ? "Yes" : "No";
        public string Remark { get; set; }
        public string Reasons { get; set; }
        public string CardSerialNo { get; set; }
        public string ChipNo { get; set; }
    }
}
