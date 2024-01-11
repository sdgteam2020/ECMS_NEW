﻿using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
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
        public Task<int> TotalProfileCount();
        public bool GetByDomainId(string DomainId, int Id);
        public Task<DTOAccountResponse?> FindDomainId(string DomainId);
        public Task<List<DTORegisterListRequest>> DomainApproveList();
        public Task<List<DTOProfileManageResponse>?> GetAllProfileManage(string Search, string Choice);
        public Task<List<DTOUserRegnResponse>?> GetAllUserRegn(string Search, string Choice);
        public Task<List<DTODomainRegnResponse>?> GetAllDomainRegn(string Search, string Choice);
        public Task<DTOUserRegnResultResponse?> SaveMapping(DTOUserRegnMappingRequest dTO, int Updatedby);
        public Task<bool?> SaveDomainRegn(DTODomainRegnRequest dTO, int Updatedby);
        public Task<bool?> UpdateDomainFlag(DTOUserRegnUpdateDomainFlagRequest dTO, int Updatedby);
        public Task<List<DTOMasterResponse>> GetAllRole();
    }
}
