﻿using DataAccessLayer.BaseInterfaces;
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

namespace DataAccessLayer
{
    public class AccountDB : GenericRepositoryDL<ApplicationUser>, IAccountDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly ILogger<DomainMapDB> _logger;
        private readonly IDataProtector protector;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserProfileDB userProfileDB;

        public AccountDB(ApplicationDbContext context, ILogger<DomainMapDB> logger, UserManager<ApplicationUser> userManager, IUserProfileDB userProfileDB, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _logger = logger;
            this.userManager = userManager;
            this.userProfileDB = userProfileDB;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
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
                                               Id = u.Id,
                                               DomainId = u.DomainId,
                                           }).Take(200).ToListAsync();
                    return allrecord;
                }
                else if (Choice == "ICNo")
                {
                    Search = string.IsNullOrEmpty(Search) ? "" : Search.ToLower();
                    var allrecord = await (from up in _context.UserProfile.Where(P => Search == "" || P.ArmyNo.ToLower().Contains(Search))
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
                                               Id = xu != null ? xu.Id : 0,
                                               DomainId = xu != null ? xu.DomainId : null,
                                           }).ToListAsync();
                    return allrecord;

                }
                else
                {
                    var allrecord = await (from up in _context.UserProfile.Take(200)
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
        public async Task<DTOUserRegnResultResponse?> SaveDomainWithAll(DTOUserRegnRequest dTO, int Updatedby)
        {
            int Status = 0;
            DTOUserRegnResultResponse dTOUserRegnResultResponse = new DTOUserRegnResultResponse();
            try
            {
                if (dTO.Id > 0)
                {
                    dTOUserRegnResultResponse.Result = true;
                    dTOUserRegnResultResponse.Message = "Save";
                    return dTOUserRegnResultResponse;
                }
                else
                {
                    TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                    if (dTO.UserId > 0)
                    {
                        DTOProfileResponse? dTOProfileResponse = await userProfileDB.GetProfileByUserId(dTO.UserId);
                        if(dTOProfileResponse!=null && dTOProfileResponse.Mapping==false)
                        {
                            trnDomainMapping.UserId = dTO.UserId;
                        }
                        else if (dTOProfileResponse != null && dTOProfileResponse.Mapping == true && dTOProfileResponse.DomainId!=null)
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
                    }
                        
                    var user = new ApplicationUser
                    {
                        DomainId = dTO.DomainId,
                        Active = dTO.Active,
                        AdminFlag = dTO.AdminFlag,
                        Updatedby = Updatedby,
                        UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                        UserName = dTO.DomainId.ToLower(),
                        Email = dTO.DomainId.ToLower() + "@army.mil",
                    };
                    var result = await userManager.CreateAsync(user, "Admin123#");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, dTO.RoleName);
                    }
                    foreach (var error in result.Errors)
                    {
                        dTOUserRegnResultResponse.Result = false;
                        dTOUserRegnResultResponse.Message += error.Description;
                        return dTOUserRegnResultResponse;
                    }

                    
                    trnDomainMapping.AspNetUsersId = user.Id;
                    trnDomainMapping.UnitId = dTO.UnitMappId;
                    trnDomainMapping.ApptId = dTO.ApptId;
                    await _context.TrnDomainMapping.AddAsync(trnDomainMapping);
                    await _context.SaveChangesAsync();

                    dTOUserRegnResultResponse.Result = true;
                    dTOUserRegnResultResponse.Message = "Domain Id has been Updated";
                    return dTOUserRegnResultResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->SaveDomainWithAll");
                return null;
            }

        }
    }
    
}
