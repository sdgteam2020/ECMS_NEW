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
                                           select new DTOProfileManageResponse()
                                           {
                                               UserId = up.UserId,
                                               ArmyNo = up.ArmyNo,
                                               Name = up.Name,
                                               IntOffr = up.IntOffr,
                                               IsIO = up.IsIO,
                                               IsCO = up.IsCO,
                                               Id = u.Id,
                                               DomainId = u.DomainId,
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
                                           select new DTOProfileManageResponse()
                                           {
                                               UserId = up.UserId,
                                               ArmyNo = up.ArmyNo,
                                               Name = up.Name,
                                               IntOffr = up.IntOffr,
                                               IsIO = up.IsIO,
                                               IsCO = up.IsCO,
                                               Id = xu != null ? xu.Id : 0,
                                               DomainId = xu != null ? xu.DomainId : null,
                                           }).Take(200).ToListAsync();
                    return allrecord;
                }
                else if (Choice == "UserId")
                {
                    int UserId = string.IsNullOrEmpty(Search) ? 0 : Convert.ToInt32(Search);
                    var allrecord = await (from up in _context.UserProfile.Where(x=>x.UserId == UserId)
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
                                               Id = xu != null ? xu.Id : 0,
                                               DomainId = xu != null ? xu.DomainId : null,
                                           }).ToListAsync();
                    return allrecord;

                }
                else
                {
                    var allrecord = await (from up in _context.UserProfile.Take(200)
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
    }
    
}
