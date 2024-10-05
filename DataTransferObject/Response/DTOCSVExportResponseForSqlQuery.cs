using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCSVExportResponseForSqlQuery
    {
        public string ServiceNo { get; set; } = string.Empty;
        public string NameAsPerRecord { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public DateTime DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string TrackingId { get; set; } = string.Empty;
        public string ApplyFor { get; set; } = string.Empty;
        public string ICardType { get; set; } = string.Empty;
        public string? State { get; set; }
        public string? District { get; set; }
        public string? PS { get; set; }
        public string? PO { get; set; }
        public string? Tehsil { get; set; }
        public string? Village { get; set; }
        public int PinCode { get; set; }
    }
}
