using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOHotlistCardGetResponse
    {
        public int TotalFilteredRecords { get; set; }
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public int RequestId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public int HotlistCardId { get; set; }
        public string RemarksNameList { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }
}
