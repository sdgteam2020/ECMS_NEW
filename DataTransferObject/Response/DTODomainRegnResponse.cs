﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODomainRegnResponse
    {
        public int Id { get; set; }
        public string DomainId { get; set; } = string.Empty;
        public List<int>? RoleIds { get; set; }
        public List<string>? RoleNames { get; set; }
        public string? ArmyNo { get; set; }
        public string DialingCode { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public DateTime UpdatedOn { get; set; }
        public bool AdminFlag { get; set; }
        public bool Active { get; set; }
        public bool IsIO { get; set; }
        public bool IsCO { get; set; }
        public bool IsRO { get; set; }
        public bool IsORO { get; set; }
        public bool Mapped { get; set; }
        public int TrnDomainMappingId { get; set; }
        public int TrnDomainMappingUnitId { get; set; }
        public short TrnDomainMappingApptId { get; set; }
        public int UserId { get; set; }
    }
}
