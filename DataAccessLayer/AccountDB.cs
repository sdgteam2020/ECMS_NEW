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
using DataTransferObject.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;
using DataTransferObject.Domain.Model;
using DataTransferObject.Domain.Error;
using DataTransferObject.Response.User;

namespace DataAccessLayer
{
    public class AccountDB : GenericRepositoryDL<ApplicationUser>, IAccountDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly ILogger<DomainMapDB> _logger;
        private readonly IDataProtector protector;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserProfileDB userProfileDB;
        private readonly IDomainMapDB domainMapDB;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public AccountDB(ApplicationDbContext context, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<DomainMapDB> logger, UserManager<ApplicationUser> userManager, IUserProfileDB userProfileDB, IDomainMapDB domainMapDB, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
            this.userManager = userManager;
            this.userProfileDB = userProfileDB;
            this.domainMapDB = domainMapDB;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<int> TotalProfileCount()
        {
            int ret = await _context.UserProfile.CountAsync();
            return ret;
        }
        public bool GetByDomainId(string DomainId, int Id)
        {
            var ret = _context.Users.Any(x => x.DomainId.ToUpper() == DomainId.ToUpper() && x.Id != Id);
            return ret;
        }
        public async Task<DTOAccountResponse?> FindDomainId(string DomainId)
        {
            try
            {
                ApplicationUser? applicationUser = await _context.Users.Where(x => x.DomainId == DomainId).FirstOrDefaultAsync();

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
        public async Task<List<DTODomainRegnResponse>?> GetAllDomainRegn(string Search, string Choice)
        {
            try
            {
                if (Choice == "DomainId")
                {
                    Search = string.IsNullOrEmpty(Search) ? "" : Search.ToLower();
                    var allrecord = await (from u in _context.Users.Where(P => Search == "" || P.DomainId.ToLower().Contains(Search)).OrderByDescending(x=>x.Id)
                                           join ur in _context.UserRoles on u.Id equals ur.UserId into uur_jointable
                                           from xur in uur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                           from xup in xtdmup_jointable.DefaultIfEmpty()
                                           select new DTODomainRegnResponse()
                                           {
                                               Id = u.Id,
                                               DomainId = u.DomainId,
                                               AdminFlag = u.AdminFlag,
                                               Active = u.Active,
                                               UpdatedOn = u.UpdatedOn,
                                               Mapped = xtdm != null ? true : false,
                                               TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                               TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                               TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                               ArmyNo = xup != null ? xup.ArmyNo : null,
                                               UserId = xup != null ? xup.UserId : 0,
                                               RoleName = xr != null ? (xr.Name != null ? xr.Name : "Role name is blank.") : "no role assign",
                                           }).Take(200).ToListAsync();
                    return allrecord;
                }
                else if (Choice == "Id")
                {
                    int Id = string.IsNullOrEmpty(Search) ? 0 : Convert.ToInt32(Search);
                    var allrecord = await (from u in _context.Users.Where(P => P.Id == Id)
                                           join ur in _context.UserRoles on u.Id equals ur.UserId into uur_jointable
                                           from xur in uur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into tdmup_jointable
                                           from xup in tdmup_jointable.DefaultIfEmpty()
                                           select new DTODomainRegnResponse()
                                           {
                                               Id = u.Id,
                                               DomainId = u.DomainId,
                                               AdminFlag = u.AdminFlag,
                                               Active = u.Active,
                                               UpdatedOn = u.UpdatedOn,
                                               Mapped = xtdm != null ? true : false,
                                               TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                               TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                               TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                               ArmyNo = xup != null ? xup.ArmyNo : null,
                                               UserId = xup != null ? xup.UserId : 0,
                                               RoleName = xr != null ? (xr.Name != null ? xr.Name : "Role name is blank.") : null,
                                           }).ToListAsync();
                    return allrecord;

                }
                else
                {
                    var allrecord = await (from u in _context.Users.OrderByDescending(x=>x.Id).Take(200)
                                           join ur in _context.UserRoles on u.Id equals ur.UserId into uur_jointable
                                           from xur in uur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                           from xup in xtdmup_jointable.DefaultIfEmpty()
                                           select new DTODomainRegnResponse()
                                           {
                                               Id = u.Id,
                                               DomainId = u.DomainId,
                                               AdminFlag = u.AdminFlag,
                                               Active = u.Active,
                                               UpdatedOn = u.UpdatedOn,
                                               Mapped = xtdm != null ? true : false,
                                               TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                               TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                               TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                               ArmyNo = xup != null ? xup.ArmyNo : null,
                                               UserId = xup != null ? xup.UserId : 0,
                                               RoleName = xr != null ? (xr.Name != null ? xr.Name : "Role name is blank.") : null,
                                           }).ToListAsync();
                    return allrecord;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->UserRegn");
                return null;
            }
        }
        public async Task<List<DTOUserRegnResponse>?> GetAllUserRegn(string Search,string Choice)
        {
            try
            {
                if (Choice== "DomainId")
                {
                    Search = string.IsNullOrEmpty(Search) ? "" : Search.ToLower();
                    var allrecord = await (from u in _context.Users.Where(P => Search == "" || P.DomainId.ToLower().Contains(Search))
                                           join ur in _context.UserRoles on u.Id equals ur.UserId into uur_jointable
                                           from xur in uur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                           from xup in xtdmup_jointable.DefaultIfEmpty()
                                           select new DTOUserRegnResponse()
                                           {
                                               Id=u.Id,
                                               DomainId = u.DomainId,
                                               AdminFlag=u.AdminFlag,
                                               Active=u.Active,
                                               UpdatedOn=u.UpdatedOn,
                                               Mapped = xtdm != null?true:false,
                                               TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                               TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                               TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                               ArmyNo = xup!=null? xup.ArmyNo:null,
                                               UserId = xup != null ? xup.UserId : 0,
                                               RoleName = xr!=null? (xr.Name != null ? xr.Name : "Role name is blank."): "no role assign",
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
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           select new DTOUserRegnResponse()
                                           {
                                               Id = xu!=null? xu.Id:null,
                                               DomainId = xu != null ? xu.DomainId:null,
                                               AdminFlag = xu != null ? xu.AdminFlag:null,
                                               Active = xu != null ? xu.Active:null,
                                               UpdatedOn = xu != null ? xu.UpdatedOn:null,
                                               Mapped = xtdm != null ? true : null,
                                               TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                               TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                               TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                               ArmyNo = up.ArmyNo,
                                               UserId = up != null ? up.UserId : 0,
                                               RoleName = xr != null ? (xr.Name != null ? xr.Name : "Role name is blank.") : null,
                                           }).Take(200).ToListAsync();
                    return allrecord;
                }
                else if (Choice == "Id")
                {
                    int Id = string.IsNullOrEmpty(Search) ? 0 : Convert.ToInt32(Search);
                    var allrecord = await (from u in _context.Users.Where(P =>P.Id== Id)
                                           join ur in _context.UserRoles on u.Id equals ur.UserId into uur_jointable
                                           from xur in uur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into tdmup_jointable
                                           from xup in tdmup_jointable.DefaultIfEmpty()
                                           select new DTOUserRegnResponse()
                                           {
                                               Id = u.Id,
                                               DomainId = u.DomainId,
                                               AdminFlag = u.AdminFlag,
                                               Active = u.Active,
                                               UpdatedOn = u.UpdatedOn,
                                               Mapped = xtdm != null ? true : false,
                                               TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                               TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                               TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                               ArmyNo = xup != null ? xup.ArmyNo : null,
                                               UserId = xup != null ? xup.UserId : 0,
                                               RoleName = xr != null ? (xr.Name != null ? xr.Name : "Role name is blank.") : null,
                                           }).ToListAsync();
                    return allrecord;

                }
                else
                {
                    var allrecord = await (from u in _context.Users.Take(200)
                                           join ur in _context.UserRoles on u.Id equals ur.UserId into uur_jointable
                                           from xur in uur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                           from xup in xtdmup_jointable.DefaultIfEmpty()
                                           select new DTOUserRegnResponse()
                                           {
                                               Id = u.Id,
                                               DomainId = u.DomainId,
                                               AdminFlag = u.AdminFlag,
                                               Active = u.Active,
                                               UpdatedOn = u.UpdatedOn,
                                               Mapped = xtdm != null ? true : false,
                                               TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                               TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                               TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                               ArmyNo = xup != null ? xup.ArmyNo : null,
                                               UserId = xup != null ? xup.UserId : 0,
                                               RoleName = xr != null ? (xr.Name != null ? xr.Name : "Role name is blank.") : null,
                                           }).ToListAsync();
                    return allrecord;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->UserRegn");
                return null;
            }
        }
        public async Task<List<DTOProfileManageResponse>?> GetAllProfileManage(string Search, string Choice)
        {
            try
            {
                if (Choice == "DomainId")
                {
                    Search = string.IsNullOrEmpty(Search) ? "" : Search.ToLower();
                    var allrecord = await (from u in _context.Users.Where(P => Search == "" || P.DomainId.ToLower().Contains(Search))
                                           join ur in _context.UserRoles on u.Id equals ur.UserId into uur_jointable
                                           from xur in uur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId
                                           join rk in _context.MRank on up.RankId equals rk.RankId
                                           select new DTOProfileManageResponse()
                                           {
                                               UserId = up.UserId,
                                               ArmyNo = up.ArmyNo,
                                               Name = up.Name,
                                               IntOffr = up.IntOffr,
                                               IsIO = up.IsIO,
                                               IsCO = up.IsCO,
                                               RankId=rk.RankId,
                                               RankName=rk.RankName,
                                               RankAbbreviation=rk.RankAbbreviation,
                                               Id = u.Id,
                                               DomainId = u.DomainId,
                                           }).Take(200).ToListAsync();
                    return allrecord;
                }
                else if (Choice == "ICNo")
                {
                    Search = string.IsNullOrEmpty(Search) ? "" : Search.ToLower();
                    var allrecord = await (from up in _context.UserProfile.Where(P => Search == "" || P.ArmyNo.ToLower().Contains(Search)).OrderByDescending(x => x.UserId)
                                           join rk in _context.MRank on up.RankId equals rk.RankId
                                           join tdm in _context.TrnDomainMapping on up.UserId equals tdm.UserId into uptdm_jointable
                                           from xtdm in uptdm_jointable.DefaultIfEmpty()
                                           join u in _context.Users on xtdm.AspNetUsersId equals u.Id into xtdmu_jointable
                                           from xu in xtdmu_jointable.DefaultIfEmpty()
                                           join ur in _context.UserRoles on xu.Id equals ur.UserId into xuur_jointable
                                           from xur in xuur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           select new DTOProfileManageResponse()
                                           {
                                               UserId = up.UserId,
                                               ArmyNo = up.ArmyNo,
                                               Name = up.Name,
                                               IntOffr = up.IntOffr,
                                               IsIO = up.IsIO,
                                               IsCO = up.IsCO,
                                               RankId = rk.RankId,
                                               RankName = rk.RankName,
                                               RankAbbreviation = rk.RankAbbreviation,
                                               Id = xu != null ? xu.Id : 0,
                                               DomainId = xu != null ? xu.DomainId : null,
                                           }).Take(200).ToListAsync();
                    return allrecord;
                }
                else if (Choice == "UserId")
                {
                    int UserId = string.IsNullOrEmpty(Search) ? 0 : Convert.ToInt32(Search);
                    var allrecord = await (from up in _context.UserProfile.Where(x=>x.UserId == UserId)
                                           join rk in _context.MRank on up.RankId equals rk.RankId
                                           join tdm in _context.TrnDomainMapping on up.UserId equals tdm.UserId into uptdm_jointable
                                           from xtdm in uptdm_jointable.DefaultIfEmpty()
                                           join u in _context.Users on xtdm.AspNetUsersId equals u.Id into xtdmu_jointable
                                           from xu in xtdmu_jointable.DefaultIfEmpty()
                                           join ur in _context.UserRoles on xu.Id equals ur.UserId into xuur_jointable
                                           from xur in xuur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           select new DTOProfileManageResponse()
                                           {
                                               UserId = up.UserId,
                                               ArmyNo = up.ArmyNo,
                                               Name = up.Name,
                                               IntOffr = up.IntOffr,
                                               IsIO = up.IsIO,
                                               IsCO = up.IsCO,
                                               RankId = rk.RankId,
                                               RankName = rk.RankName,
                                               RankAbbreviation = rk.RankAbbreviation,
                                               Id = xu != null ? xu.Id : 0,
                                               DomainId = xu != null ? xu.DomainId : null,
                                           }).ToListAsync();
                    return allrecord;

                }
                else
                {
                    var allrecord = await (from up in _context.UserProfile.OrderByDescending(x=>x.UserId).Take(200)
                                           join rk in _context.MRank on up.RankId equals rk.RankId
                                           join tdm in _context.TrnDomainMapping on up.UserId equals tdm.UserId into uptdm_jointable
                                           from xtdm in uptdm_jointable.DefaultIfEmpty()
                                           join u in _context.Users on xtdm.AspNetUsersId equals u.Id into xtdmu_jointable
                                           from xu in xtdmu_jointable.DefaultIfEmpty()
                                           join ur in _context.UserRoles on xu.Id equals ur.UserId into xuur_jointable
                                           from xur in xuur_jointable.DefaultIfEmpty()
                                           join r in _context.Roles on xur.RoleId equals r.Id into xurr_jointable
                                           from xr in xurr_jointable.DefaultIfEmpty()
                                           select new DTOProfileManageResponse()
                                           {
                                               UserId = up.UserId,
                                               ArmyNo = up.ArmyNo,
                                               Name = up.Name,
                                               IntOffr = up.IntOffr,
                                               IsIO = up.IsIO,
                                               IsCO = up.IsCO,
                                               RankId = rk.RankId,
                                               RankName = rk.RankName,
                                               RankAbbreviation = rk.RankAbbreviation,
                                               Id = xu != null ?xu.Id : 0,
                                               DomainId = xu != null ? xu.DomainId : null,
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
        public async Task<DTOUserRegnResultResponse?> SaveMapping(DTOUserRegnMappingRequest dTO, int Updatedby)
        {
            DTOUserRegnResultResponse dTOUserRegnResultResponse = new DTOUserRegnResultResponse();
            
            try
            {
                TrnDomainMapping trnDomainMapping = await domainMapDB.Get(dTO.TDMId);
                if (dTO.UserId > 0)
                {
                    DTOProfileResponse? dTOProfileResponse = await userProfileDB.GetProfileByUserId(dTO.UserId);
                    if (dTOProfileResponse != null && dTOProfileResponse.Mapping == false)
                    {
                        trnDomainMapping.UserId = dTO.UserId;
                        trnDomainMapping.MappedBy = Updatedby;
                        trnDomainMapping.MappedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        await domainMapDB.Update(trnDomainMapping);
                        dTOUserRegnResultResponse.Result = true;
                        dTOUserRegnResultResponse.Message = "Profile mapped.";
                        return dTOUserRegnResultResponse;
                    }
                    else if (dTOProfileResponse != null && dTOProfileResponse.Mapping == true && dTOProfileResponse.DomainId != null && dTOProfileResponse.AspNetUsersId == dTO.Id)
                    {
                        dTOUserRegnResultResponse.Result = false;
                        dTOUserRegnResultResponse.Message = "Profile Id -" + dTOProfileResponse.UserId + " is alredy mapped to Domain Id - " + dTOProfileResponse.DomainId + " in Sys.<br/>Action not required.";
                        return dTOUserRegnResultResponse;
                    }
                    else if (dTOProfileResponse != null && dTOProfileResponse.Mapping == true && dTOProfileResponse.DomainId != null && dTOProfileResponse.AspNetUsersId != dTO.Id)
                    {
                        dTOUserRegnResultResponse.Result = false;
                        dTOUserRegnResultResponse.Message = "Profile Id -" + dTOProfileResponse.UserId + " is mapped to Domain Id - " + dTOProfileResponse.DomainId + " in Sys.<br/>Pl relieved first and try again.";
                        return dTOUserRegnResultResponse;
                    }
                    else
                    {
                        dTOUserRegnResultResponse.Result = false;
                        dTOUserRegnResultResponse.Message = "Army number not valid.";
                        return dTOUserRegnResultResponse;
                    }
                }
                else
                {
                    trnDomainMapping.UserId = null;
                    trnDomainMapping.MappedBy = null;
                    trnDomainMapping.MappedDate = null;
                    await domainMapDB.Update(trnDomainMapping);
                    dTOUserRegnResultResponse.Result = true;
                    dTOUserRegnResultResponse.Message = "Profile Unmapped.";
                    return dTOUserRegnResultResponse;
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->SaveMapping");
                return null;
            }
        }
        public async Task<bool?> SaveDomainRegn(DTODomainRegnRequest dTO, int Updatedby)
        {
            try
            {
                if (dTO.Id > 0)
                {
                    var userUpdate = await userManager.FindByIdAsync(dTO.Id.ToString());

                    if (userUpdate == null)
                    {
                        return false;
                    }
                    else
                    {
                        userUpdate.DomainId = dTO.DomainId;
                        userUpdate.Active = dTO.Active;
                        userUpdate.Updatedby = Updatedby;
                        userUpdate.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        userUpdate.UserName = dTO.DomainId.ToLower();
                        userUpdate.Email = dTO.DomainId.ToLower() + "@army.mil";
                        if (dTO.AdminFlag == true)
                        {
                            userUpdate.AdminFlag = dTO.AdminFlag;
                            userUpdate.AdminFlagDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        }
                        else
                        {
                            userUpdate.AdminFlag = dTO.AdminFlag;
                            userUpdate.AdminFlagDate = null;
                        }
                        var result = await userManager.UpdateAsync(userUpdate);

                        if (!result.Succeeded)
                        {
                            return false;
                        }

                        foreach (var error in result.Errors)
                        {
                            return false;
                        }
                        TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                        if (dTO.TDMId>0)
                        {
                            trnDomainMapping = await domainMapDB.Get(dTO.TDMId);
                            trnDomainMapping.AspNetUsersId = userUpdate.Id;
                            trnDomainMapping.UnitId = dTO.UnitMappId;
                            trnDomainMapping.ApptId = dTO.ApptId;
                            await domainMapDB.Update(trnDomainMapping);
                        }
                        else
                        {
                            trnDomainMapping.AspNetUsersId = userUpdate.Id;
                            trnDomainMapping.UnitId = dTO.UnitMappId;
                            trnDomainMapping.ApptId = dTO.ApptId;
                            await domainMapDB.Add(trnDomainMapping);
                        }

                        return true;
                    }
                }
                else
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            var userAdd = new ApplicationUser
                            {
                                DomainId = dTO.DomainId,
                                Active = dTO.Active,
                                AdminFlag = dTO.AdminFlag,
                                AdminFlagDate = dTO.AdminFlag == true ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")) : null,
                                Updatedby = Updatedby,
                                UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                                UserName = dTO.DomainId.ToLower(),
                                NormalizedUserName = dTO.DomainId.ToUpper(),
                                Email = dTO.DomainId.ToLower() + "@army.mil",
                                NormalizedEmail = dTO.DomainId.ToUpper() + "@ARMY.MIL"
                            };

                            userAdd.PasswordHash = _passwordHasher.HashPassword(userAdd, "Admin123#");
                            await _context.Users.AddAsync(userAdd);
                            await _context.SaveChangesAsync();
                            int Id = userAdd.Id;
                            // Assign new roles to the user
                            foreach (var roleId in dTO.RoleIds)
                            {
                                await _context.UserRoles.AddAsync(new IdentityUserRole<int> { RoleId = roleId, UserId = Id });
                                await _context.SaveChangesAsync();
                            }

                            var trnmapAdd = new TrnDomainMapping
                            {
                                AspNetUsersId = Id,
                                UnitId = dTO.UnitMappId,
                                ApptId = dTO.ApptId,
                            };
                            await _context.TrnDomainMapping.AddAsync(trnmapAdd);
                            await _context.SaveChangesAsync(true);
                            transaction.Commit();
                            await userManager.UpdateSecurityStampAsync(userAdd);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.LogError(1001, ex, "AccountDB->SaveDomainRegn");
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->SaveDomainRegn");
                return null;
            }

        }
        public async Task<bool?> UpdateDomainFlag(DTOUserRegnUpdateDomainFlagRequest dTO, int Updatedby)
        {
            try
            {
                var userUpdate = await userManager.FindByIdAsync(dTO.Id.ToString());

                if (userUpdate == null)
                {
                    return false;
                }
                else
                {
                    userUpdate.Active = dTO.Active;
                    userUpdate.Updatedby = Updatedby;
                    userUpdate.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    if (dTO.AdminFlag == true)
                    {
                        userUpdate.AdminFlag = dTO.AdminFlag;
                        userUpdate.AdminFlagDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    }
                    else
                    {
                        userUpdate.AdminFlag = dTO.AdminFlag;
                        userUpdate.AdminFlagDate = null;
                    }
                    var result = await userManager.UpdateAsync(userUpdate);

                    if (result.Succeeded)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->SaveDomainRegn");
                return null;
            }

        }
        public async Task<List<DTOMasterResponse>>GetAllRole()
        {
            List<DTOMasterResponse> lst = new List<DTOMasterResponse>();
            var Ret = await _context.Roles.OrderBy(x=>x.Id).ToListAsync();
            foreach (var r in Ret)
            {
                DTOMasterResponse db = new DTOMasterResponse()
                {
                    Id = r.Id,
                    Name = r.Name != null ? r.Name : "Role Name Blank",
                };
                lst.Add(db);
            }
            return lst;
        }
    }
    
}
