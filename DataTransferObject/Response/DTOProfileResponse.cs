﻿using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOProfileResponse
    {
        public int UserId { get; set; }
        public string ArmyNo { get; set; } = string.Empty;
        public bool? IsRO { get; set; }
        public int RankId { get; set; }
        public string RankName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string? DialingCode { get; set; } 
        public string? Extension { get; set; } 
        public bool Mapping { get; set; }
        public string? DomainId { get; set; }
        public int AspNetUsersId { get; set; }
        public byte FormationId { get; set; }
        public string FormationName { get; set; } = string.Empty;
        public short ApptId { get; set; }
        public string AppointmentName { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }

    }
}
