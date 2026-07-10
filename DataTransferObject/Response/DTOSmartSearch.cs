using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOSmartSearch
    {
        public int BasicDetailId { get; set; }
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public int RequestId { get; set; }
        public string Image { get; set; } = string.Empty;
        public int? MaxTrnFwdId { get; set; }
        public string? ChipNo { get; set; }
        public string? CardSerialNo { get; set; }

        public string? FName_1 { get; set; }
        public string? LName_1 { get; set; }

        public string? FName_2 { get; set; }
        public string? LName_2 { get; set; }

    }
}
