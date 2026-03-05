using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOFaultyCardListResponse
    {
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public int UnitMapId { get; set; }
        public string UnitAbbreviation { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public int RequestId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public int TrnFaultyCardId { get; set; }
        public string RemarksIds { get; set; } = string.Empty;
        public string RemarksNameList { get; set; } = string.Empty;
        public string? FromRemark { get; set; }
        public string? ToRemark { get; set; }
        public string FaultyStage { get; set; } = string.Empty;
        public byte CategoryId { get; set; }
        public bool IsEditAction { get; set; } = false;
        public string? EncryptedId { get; set; }

    }
}
