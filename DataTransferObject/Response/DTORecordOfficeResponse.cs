﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORecordOfficeResponse
    {
        public byte RecordOfficeId { get; set; }
        public string RecordOfficeName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public byte ArmedId { get; set; }
        public string ArmedName { get; set; } = string.Empty;
        public int TDMId { get; set; }
        public string? Message { get; set; }
        public string DomainId { get; set; } = string.Empty;
        public string ArmyNo { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
