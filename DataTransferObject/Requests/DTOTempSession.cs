﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOTempSession
    {
        public string DomainId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string ICNO { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int TrnDomainMappingId { get; set; }
        public int AspNetUsersId { get; set; }
        public int Status { get; set; }
    }
}