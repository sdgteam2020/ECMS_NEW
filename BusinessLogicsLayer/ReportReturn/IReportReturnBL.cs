﻿using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.ReportReturn
{
    public interface IReportReturnBL
    {
        public Task<DTOReportReturnCountlst> GetMstepCount(int UserId);
    }
}