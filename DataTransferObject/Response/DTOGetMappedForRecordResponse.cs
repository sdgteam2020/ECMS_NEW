﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOGetMappedForRecordResponse
    {
        public string DomainId { get; set; } = string.Empty;
        public string ArmyNo { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int TDMId { get; set; }
    }
}
