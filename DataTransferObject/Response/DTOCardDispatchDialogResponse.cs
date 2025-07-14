using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCardDispatchDialogResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int DispatchCardMappingId { get; set; }
        public int RequestId { get; set; }
        public string NameAsPerRecord { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; } 
        public string RankName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ServiceNo { get; set; } = string.Empty;
        public string ArmedAbbreviation { get; set; } = string.Empty;
        public string? RegimentalName { get; set; }
        public string? RecordOfficeName { get; set; }
        public string ChipNo { get; set; } = string.Empty;
        public string CardSerialNo { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;

    }
}
