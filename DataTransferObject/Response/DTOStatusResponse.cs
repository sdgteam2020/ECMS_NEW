﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOStatusResponse
    {
        public bool success { get; set; }
        public string? message { get; set; } = string.Empty;
    }
}
