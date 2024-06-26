﻿using BusinessLogicsLayer.BdeCate;
using DataTransferObject.Domain.Master;
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
    public interface ITrnFwnBL : IGenericRepository<MTrnFwd>
    {
        public Task<bool> UpdateAllBYRequestId(int RequestId);
        public Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId);
        public Task<bool?> SaveInternalFwd(DTOSaveInternalFwdRequest dTO);

    }
}
