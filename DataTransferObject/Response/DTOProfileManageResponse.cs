﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOProfileManageResponse
    {
        public int UserId { get; set; }
        public string ArmyNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsToken { get; set; }
        public bool IsIO { get; set; }
        public bool IsCO { get; set; }
        public bool IntOffr { get; set; }
        public short RankId { get; set; }
        public string RankName { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public int Id { get; set; }
        public string? DomainId { get; set; }
    }
}
