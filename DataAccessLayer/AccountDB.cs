using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.DataProtection;
using System.Runtime.Intrinsics.Arm;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class AccountDB : GenericRepositoryDL<ApplicationUser>, IAccountDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly ILogger<DomainMapDB> _logger;
        private readonly IDataProtector protector;
        public AccountDB(ApplicationDbContext context, ILogger<DomainMapDB> logger, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<DTOAccountResponse?> FindDomainId(string DomainId)
        {
            try
            {
                var applicationUser = await _context.Users.Where(x => x.DomainId == DomainId).Select(x => new { x.Id, x.DomainId, x.Active, x.AdminFlag }).FirstOrDefaultAsync();
                if (applicationUser != null)
                {
                    DTOAccountResponse dTOAccountResponse = new DTOAccountResponse();
                    dTOAccountResponse.Id = applicationUser.Id;
                    dTOAccountResponse.DomainId = applicationUser.DomainId;
                    dTOAccountResponse.Active = applicationUser.Active;
                    dTOAccountResponse.AdminFlag = applicationUser.AdminFlag;
                    return dTOAccountResponse;
                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->DomainApproveList");
                return null;
            }

        }
        public async Task<List<DTORegisterListRequest>?> DomainApproveList()
        {
            try
            {
                var allrecord = await (from e in _context.Users
                                       join r in _context.UserRoles on e.Id equals r.UserId
                                       join n in _context.Roles on r.RoleId equals n.Id
                                       where e.AdminFlag == false
                                       orderby e.Id
                                       select new DTORegisterListRequest()
                                       {
                                           EncryptedId = protector.Protect(e.Id.ToString()),
                                           DomainId = e.DomainId,
                                           RoleName = n.Name != null ? n.Name : "no role assign",
                                       }).ToListAsync();
                return allrecord;
            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "AccountDB->DomainApproveList");
                return null;
            }

        }
        public async Task<List<DTOProfileManageResponse>?> GetAllProfileManage(string Search,string Choice)
        {
            try
            {
                List<DTOProfileManageResponse> dTOProfileManageResponse = new List<DTOProfileManageResponse>();
                if (Choice== "DomainId")
                {
                    Search = string.IsNullOrEmpty(Search) ? "" : Search.ToLower();
                    var allrecord = await (from u in _context.Users.Where(P => Search == "" || P.DomainId.ToLower().Contains(Search))
                                           join ur in _context.UserRoles on u.Id equals ur.UserId
                                           join r in _context.Roles on ur.RoleId equals r.Id
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                           from xup in xtdmup_jointable.DefaultIfEmpty()
                                           select new DTOProfileManageResponse()
                                           {
                                               Id=u.Id,
                                               DomainId = u.DomainId,
                                               AdminFlag=u.AdminFlag,
                                               Active=u.Active,
                                               UpdatedOn=u.UpdatedOn,
                                               Mapped = xtdm != null?true:false,
                                               ArmyNo = xup!=null? xup.ArmyNo:null,
                                               RoleName = r!=null? (r.Name != null ? r.Name : "Role name is blank."): "no role assign",
                                           }).Take(200).ToListAsync();
                    return allrecord;
                }
                else if (Choice == "ICNo")
                {
                    Search = string.IsNullOrEmpty(Search) ? "" : Search.ToLower();
                    var allrecord = await (from up in _context.UserProfile.Where(P => Search == "" || P.ArmyNo.ToLower().Contains(Search))
                                           join tdm in _context.TrnDomainMapping on up.UserId equals tdm.UserId into uptdm_jointable
                                           from xtdm in uptdm_jointable.DefaultIfEmpty()
                                           join u in _context.Users on xtdm.AspNetUsersId equals u.Id into xtdmu_jointable
                                           from xu in xtdmu_jointable.DefaultIfEmpty()
                                           join ur in _context.UserRoles on xu.Id equals ur.UserId into xuur_jointable
                                           from xur in xuur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id
                                           select new DTOProfileManageResponse()
                                           {
                                               Id = xu!=null? xu.Id:null,
                                               DomainId = xu != null ? xu.DomainId:null,
                                               AdminFlag = xu != null ? xu.AdminFlag:null,
                                               Active = xu != null ? xu.Active:null,
                                               UpdatedOn = xu != null ? xu.UpdatedOn:null,
                                               Mapped = xtdm != null ? true : null,
                                               ArmyNo = up.ArmyNo,
                                               RoleName = r != null ? (r.Name != null ? r.Name : "Role name is blank.") : null,
                                           }).Take(200).ToListAsync();
                    return allrecord;
                }
                else if (Choice == "Id")
                {
                    int Id = string.IsNullOrEmpty(Search) ? 0 : Convert.ToInt32(Search);
                    var allrecord = await (from e in _context.Users.Where(P =>P.Id== Id)
                                           join r in _context.UserRoles on e.Id equals r.UserId
                                           join n in _context.Roles on r.RoleId equals n.Id
                                           join tdm in _context.TrnDomainMapping on e.Id equals tdm.AspNetUsersId into etdm_jointable
                                           from xtdm in etdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into tdmup_jointable
                                           from xup in tdmup_jointable.DefaultIfEmpty()
                                           select new DTOProfileManageResponse()
                                           {
                                               Id = e.Id,
                                               DomainId = e.DomainId,
                                               AdminFlag = e.AdminFlag,
                                               Active = e.Active,
                                               UpdatedOn = e.UpdatedOn,
                                               Mapped = xtdm != null ? true : false,
                                               ArmyNo = xup != null ? xup.ArmyNo : null,
                                               RoleName = n != null ? (n.Name != null ? n.Name : "Role name is blank.") : "no role assign",
                                           }).ToListAsync();
                    return allrecord;

                }
                else
                {
                    var allrecord = await (from e in _context.Users.Take(5)
                                           join r in _context.UserRoles on e.Id equals r.UserId
                                           join n in _context.Roles on r.RoleId equals n.Id
                                           join tdm in _context.TrnDomainMapping on e.Id equals tdm.AspNetUsersId into etdm_jointable
                                           from xtdm in etdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into tdmup_jointable
                                           from xup in tdmup_jointable.DefaultIfEmpty()
                                           select new DTOProfileManageResponse()
                                           {
                                               Id = e.Id,
                                               DomainId = e.DomainId,
                                               AdminFlag = e.AdminFlag,
                                               Active = e.Active,
                                               UpdatedOn = e.UpdatedOn,
                                               Mapped = xtdm != null ? true : false,
                                               ArmyNo = xup != null ? xup.ArmyNo : null,
                                               RoleName = n != null ? (n.Name != null ? n.Name : "Role name is blank.") : "no role assign",
                                           }).ToListAsync();
                    return allrecord;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->ProfileManage");
                return null;
            }
        }
    }
}
