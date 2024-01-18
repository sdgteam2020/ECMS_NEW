using BusinessLogicsLayer.Corps;
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
        public async Task<int> TotalProfileCount()
        {
            return await _iAccountDB.TotalProfileCount();
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
        public async Task<List<DTODomainRegnResponse>?> GetAllDomainRegn(string Search, string Choice)
        {
            return await _iAccountDB.GetAllDomainRegn(Search, Choice);
        }
        public async Task<DTOUserRegnResultResponse?> SaveMapping(DTOUserRegnMappingRequest dTO)
        {
            return await _iAccountDB.SaveMapping(dTO);
        }
        public async Task<bool?> SaveDomainRegn(DTODomainRegnRequest dTO)
        {
            return await _iAccountDB.SaveDomainRegn(dTO);
        }
        public async Task<bool?> UpdateDomainFlag(DTOUserRegnUpdateDomainFlagRequest dTO)
        {
            return await _iAccountDB.UpdateDomainFlag(dTO);
        }
        public async Task<List<DTOMasterResponse>> GetAllRole()
        {
            return await _iAccountDB.GetAllRole();
        }
        public async Task<DTOTempSession?> ProfileAndMappingSaving(DTOProfileAndMappingRequest model, DTOTempSession dTOTempSession)
        {
            return await _iAccountDB.ProfileAndMappingSaving(model, dTOTempSession);
        }
        public async Task<DTOAccountCountResponse> AccountCount()
        {
            return await _iAccountDB.AccountCount();
        }
        public async Task<bool?> SaveUnitWithMapping(DTOSaveUnitWithMappingRequest dTO)
        {
            return await _iAccountDB.SaveUnitWithMapping(dTO);
        }
    }
}
