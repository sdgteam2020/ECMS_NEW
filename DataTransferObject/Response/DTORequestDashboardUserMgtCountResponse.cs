﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORequestDashboardUserMgtCountResponse
    {
        public int TotRegisterUser { get; set; }
        public int TotPostingIn { get; set; }
        public int TotPostingOut { get; set; }
    }
}
