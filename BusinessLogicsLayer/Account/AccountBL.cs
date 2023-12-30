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
        public async Task<DTOAccountResponse?> FindDomainId(string DomainId)
        {
            return await _iAccountDB.FindDomainId(DomainId);
        }
        public async Task<List<DTORegisterListRequest>> DomainApproveList()
        {
            return await _iAccountDB.DomainApproveList();
        }
        public async Task<List<DTOProfileManageResponse>> GetAllProfileManage(string Search, string Choice)
        {
            return await _iAccountDB.GetAllProfileManage(Search, Choice);
        }
    }
}
