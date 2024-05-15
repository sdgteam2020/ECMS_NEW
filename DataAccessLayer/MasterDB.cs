using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using DataTransferObject.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class MasterDB : IMasterDB
    {
        private readonly DapperContext _contextDP;
        protected readonly ApplicationDbContext _context;
        private readonly ILogger<MasterDB> _logger;
        public MasterDB(DapperContext contextDP, ApplicationDbContext context, ILogger<MasterDB> logger)
        {
            _contextDP = contextDP;
            _context = context;
            _logger = logger;
        }
      
        private readonly IConfiguration configuration;

        public async Task<List<DTORemarksResponse>> GetRemarksByTypeId(DTORemarksRequest Data)
        {
            string query = "Select rem.RemarksId,rem.Remarks,remtype.RemarksType,remtype.RemarkTypeId from MRemarks rem" +
                            " inner join MRemarkType remtype on rem.RemarkTypeId=remtype.RemarkTypeId"+
                            " where rem.RemarkTypeId in @RemarkTypeId";
            int[] RemarkTypeId=Data.RemarkTypeId;
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<DTORemarksResponse>(query, new { RemarkTypeId });



                return ret.ToList();
            }
        }
        public async Task<List<DTOArmsListResponse>> GetArmsList()
        {
            string query = "Select marm.ArmedId,marm.ArmedName from MArmedType marm order by ArmedName";

            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOArmsListResponse>(query);
                return ret.ToList();
            }
        }

        public async Task<List<DTOMasterResponse>> GetMFmnBranches()
        {
            string query = "SELECT FmnBranchID Id,BranchName Name FROM MFmnBranches where IsActive=1 and FmnBranchID !=1";
          
            using (var connection = _contextDP.CreateConnection())
            { 
                var ret = await connection.QueryAsync<DTOMasterResponse>(query);
                return ret.ToList();
            }
        }

        public async Task<List<DTOMasterResponse>> GetMPSO()
        {
            string query = "SELECT PsoId Id,PSOName Name FROM MPso where IsActive=1 and PsoId !=1";

            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOMasterResponse>(query);
                return ret.ToList();
            }
        }

        public async Task<List<DTOMasterResponse>> GetMSubDte()
        {
            string query = "SELECT SubDteId Id,SubDteName Name FROM MSubDte where IsActive=1 and SubDteId !=1";

            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOMasterResponse>(query);
                return ret.ToList();
            }
        }

        public async Task<List<DTOMasterResponse>> GetPostingReason(int TypeId)
        {
            string query = "SELECT  Id, Reason Name FROM MPostingReason where Type=@TypeId";

            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOMasterResponse>(query,new { TypeId });
                return ret.ToList();
            }
        }
        public async Task<DTODashboardMasterCountResponse> GetDashboardMasterCount()
        {
            string query = " declare @TotComd int=0 declare @TotCorps int=0 declare @TotDiv int=0 declare @TotBde int=0  declare @TotMapUnit int=0 declare @TotRecordOffice int=0 " +
                            " declare @TotUnit int=0 declare @TotRank int=0 declare @TotAppointment int=0 declare @TotArms int=0  declare @TotRegtCentre int=0 " +
                            " declare @TotDomainRegn int=0 declare @TotUserProfile int=0 " +
                            " select @TotComd=COUNT(ComdId) from MComd " +
                            " select @TotCorps=COUNT(CorpsId) from MCorps " +
                            " select @TotDiv=COUNT(DivId) from MDiv " +
                            " select @TotBde=COUNT(BdeId) from MBde " +
                            " select @TotMapUnit=COUNT(UnitMapId) from MapUnit " +
                            " select @TotUnit=COUNT(UnitId) from MUnit " +
                            " select @TotRank=COUNT(RankId) from MRank " +
                            " select @TotAppointment=COUNT(ApptId) from MAppointment " +
                            " select @TotArms=COUNT(ArmedId) from MArmedType " +
                            " select @TotRegtCentre=COUNT(RegId) from MRegimental " +
                            " select @TotRecordOffice=COUNT(RecordOfficeId) from MRecordOffice " +

                            " select @TotDomainRegn=COUNT(u.Id) from AspNetUsers u " +
                            " inner join TrnDomainMapping trn on u.Id = trn.AspNetUsersId  " +
                            " select @TotUserProfile=COUNT(UserId) from UserProfile " +

                            " select @TotComd TotComd,@TotCorps TotCorps,@TotDiv TotDiv,@TotBde TotBde,@TotMapUnit TotMapUnit,@TotUnit TotUnit,@TotRank TotRank,@TotAppointment TotAppointment,@TotArms TotArms,@TotRegtCentre TotRegtCentre,@TotRecordOffice TotRecordOffice,@TotDomainRegn TotDomainRegn,@TotUserProfile TotUserProfile ";

            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTODashboardMasterCountResponse>(query);
                return ret.FirstOrDefault();
            }
        }
        public async Task<List<DTOGetMappedForRecordResponse>?> GetMappedForRecord(int TypeId, string SearchName)
        {
            #region using linq
            //try
            //{
            //    if (TypeId == 1)
            //    {
            //        SearchName = string.IsNullOrEmpty(SearchName) ? "" : SearchName.ToLower();
            //        var query = await (from u in _context.Users.Where(P => SearchName == "" || P.DomainId.ToLower().Contains(SearchName)).OrderByDescending(x => x.Id)
            //                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
            //                           from xtdm in utdm_jointable.DefaultIfEmpty()
            //                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
            //                           from xup in xtdmup_jointable.DefaultIfEmpty()
            //                           join rk in _context.MRank on xup.RankId equals rk.RankId
            //                           select new DTOGetMappedForRecordResponse()
            //                           {
            //                               DomainId = u.DomainId,
            //                               TDMId = xtdm != null ? xtdm.Id : 0,
            //                               ArmyNo = xup != null ? xup.ArmyNo : "No Army No",
            //                               Name = xup != null ? xup.Name : "No Name",
            //                               RankAbbreviation = xup != null ? rk.RankAbbreviation : "No RK",
            //                           }).Take(5).ToListAsync();
            //        return query;

            //    }
            //    else if (TypeId == 2)
            //    {
            //        SearchName = string.IsNullOrEmpty(SearchName) ? "" : SearchName.ToLower();
            //        var allrecord = await (from up in _context.UserProfile.Where(P => SearchName == "" || P.ArmyNo.ToLower().Contains(SearchName)).OrderByDescending(x => x.UserId)
            //                               join rk in _context.MRank on up.RankId equals rk.RankId
            //                               join tdm in _context.TrnDomainMapping on up.UserId equals tdm.UserId
            //                               join u in _context.Users on tdm.AspNetUsersId equals u.Id
            //                               select new DTOGetMappedForRecordResponse()
            //                               {
            //                                   ArmyNo = up.ArmyNo,
            //                                   Name = up.Name,
            //                                   RankAbbreviation = rk.RankAbbreviation,
            //                                   TDMId = tdm.Id,
            //                                   DomainId = u.DomainId,
            //                               }).Take(5).ToListAsync();
            //        return allrecord;
            //    }
            //    else
            //    {
            //        SearchName = string.IsNullOrEmpty(SearchName) ? "" : SearchName.ToLower();
            //        var query = await (from u in _context.Users.Where(P => SearchName == "" || P.DomainId.ToLower().Contains(SearchName)).OrderByDescending(x => x.Id)
            //                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
            //                           from xtdm in utdm_jointable.DefaultIfEmpty()
            //                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
            //                           from xup in xtdmup_jointable.DefaultIfEmpty()
            //                           join rk in _context.MRank on xup.RankId equals rk.RankId
            //                           select new DTOGetMappedForRecordResponse()
            //                           {
            //                               DomainId = u.DomainId,
            //                               TDMId = xtdm != null ? xtdm.Id : 0,
            //                               ArmyNo = xup != null ? xup.ArmyNo : "No Army No",
            //                               Name = xup != null ? xup.Name : "No Name",
            //                               RankAbbreviation = xup != null ? rk.RankAbbreviation : "No RK",
            //                           }).Take(5).ToListAsync();
            //        return query;

            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(1001, ex, "AccountDB->ProfileManage");
            //    return null;
            //}
            #endregion
            try { 
                string query = "";
                if (TypeId == 1)
                {
                    SearchName = "%" + SearchName.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    query = "Select TOP 5 users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name,trndomain.Id as TDMId from AspNetUsers users" +
                            " inner join TrnDomainMapping trndomain on users.Id=trndomain.AspNetUsersId" +
                            " inner join UserProfile usep on usep.UserId=trndomain.UserId" +
                            " inner join MRank ra on ra.RankId=usep.RankId " +
                            " where users.DomainId like @SearchName and usep.IsRO=1";

                }
                else if (TypeId == 2)
                {
                    SearchName = "%" + SearchName.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    query = "Select TOP 5 users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name,trndomain.Id as TDMId from UserProfile usep" +
                            " inner join MRank ra on ra.RankId=usep.RankId " +
                            " inner join TrnDomainMapping trndomain on trndomain.UserId=usep.UserId" +
                            " inner join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                            " where usep.ArmyNo like @SearchName and usep.IsRO=1";

                }
                else
                {
                    SearchName = "%" + SearchName.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    query = "Select TOP 5 users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name,trndomain.Id as TDMId from AspNetUsers users" +
                            " inner join TrnDomainMapping trndomain on users.Id=trndomain.AspNetUsersId" +
                            " inner join UserProfile usep on usep.UserId=trndomain.UserId" +
                            " inner join MRank ra on ra.RankId=usep.RankId " +
                            " where users.DomainId like @SearchName and usep.IsRO=1";

                }
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetMappedForRecordResponse>(query, new { TypeId, SearchName });
                    return allrecord.ToList();

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MasterDB->GetMappedForRecord");
                return null;
            }

        }
        public async Task<DTOGetDomainIdByTDMIdResponse?> GetDomainIdByTDMId(int TDMId)
        {
            try
            {
                string query = "";
                query = "Select users.DomainId,ra.RankAbbreviation,usep.Name,usep.ArmyNo from TrnDomainMapping trndomain" +
                        " inner join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " left join MRank ra on ra.RankId=usep.RankId " +
                        " where trndomain.Id=@TDMId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetDomainIdByTDMIdResponse>(query, new { TDMId });
                    return allrecord.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MasterDB->GetDomainIdByTDMId");
                return null;
            }

        }
    }
}