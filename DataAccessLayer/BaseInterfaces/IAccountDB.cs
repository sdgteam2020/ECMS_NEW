﻿using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IAccountDB : IGenericRepositoryDL<ApplicationUser>
    {
        public Task<DTOAccountResponse?> FindDomainId(string DomainId);
    }
}