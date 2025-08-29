using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOLostCardGetResponse
    {
        public string NameAsPerRecord { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string? RegimentalName { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string? ArmedName { get; set; }
        public int RequestId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public int LostCardId { get; set; }
        public DateTime LostOn { get; set; }
        public string? Remark { get; set; }
        public bool IsActive { get; set; } = false;
        public string? EncryptedId { get; set; }
        public string? SupportDocName { get; set; }
        public bool IsFIRLogged { get; set; }
    }
}
