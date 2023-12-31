﻿using BusinessLogicsLayer.Corps;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Response;
using DataTransferObject.Requests;

namespace BusinessLogicsLayer.Account
{
    public class AccountBL : GenericRepositoryDL<ApplicationUser>, IAccountBL
    {
        private readonly IAccountDB _iAccountDB;

        public AccountBL(ApplicationDbContext context, IAccountDB accountDB) : base(context)
        {
            _iAccountDB = accountDB;
        }
        public bool GetByDomainId(string DomainId, int Id)
        {
            return _iAccountDB.GetByDomainId(DomainId, Id);
        }
        public async Task<DTOAccountResponse?> FindDomainId(string DomainId)
        {
            return await _iAccountDB.FindDomainId(DomainId);
        }
        public async Task<List<DTORegisterListRequest>> DomainApproveList()
        {
            return await _iAccountDB.DomainApproveList();
        }
        public async Task<List<DTOProfileManageResponse>?> GetAllProfileManage(string Search, string Choice)
        {
            return await _iAccountDB.GetAllProfileManage(Search, Choice);
        }
        public async Task<List<DTOUserRegnResponse>?> GetAllUserRegn(string Search, string Choice)
        {
            return await _iAccountDB.GetAllUserRegn(Search, Choice);
        }
        public async Task<DTOUserRegnResultResponse?> SaveDomainWithAll(DTOUserRegnRequest dTO, int Updatedby)
        {
            return await _iAccountDB.SaveDomainWithAll(dTO, Updatedby);
        }
    }
}
