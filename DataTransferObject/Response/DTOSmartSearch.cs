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
        public string Image { get; set; } = string.Empty;

    }
}
