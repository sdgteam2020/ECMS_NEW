﻿using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ITrnMappingUnMappingLogDB
    {
        public Task<bool> Add(TrnMappingUnMapping_Log data);
    }
}
