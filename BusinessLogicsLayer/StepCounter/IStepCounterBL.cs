﻿using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Bde
{
    public interface IStepCounterBL : IGenericRepository<MStepCounter>
    {

        public Task<MStepCounter> UpdateStepCounter(MStepCounter Data);
    }
}
