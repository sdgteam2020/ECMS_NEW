﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOIMLoginRequest
    {
        public string DomainId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
