using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAppClosedListResponse
    {
        public int Sno { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int Updatedby { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string ApplyFor { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int Id { get; set; }
        public int BasicDetailId { get; set; }
        public byte ReasonId { get; set; }
        public int RequestId { get; set; }
    }
}
