﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOMHierarchyRequest
    {
        public int? TableId { get; set; }
        public int? ComdId { get; set; }
                  
        public int? CorpsId { get; set; }
                  
        public int? DivId { get; set; }
                  
        public int? BdeId { get; set; }
    }
}
