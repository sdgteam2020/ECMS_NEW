﻿using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ITrnLoginLogDB : IGenericRepositoryDL<TrnLogin_Log>
    {
        public Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId);
        public Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId);
    }
}
