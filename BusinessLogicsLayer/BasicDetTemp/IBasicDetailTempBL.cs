﻿using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDetTemp
{
    public interface IBasicDetailTempBL : IGenericRepository<BasicDetailTemp>
    {
        public Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId);
    }
}
