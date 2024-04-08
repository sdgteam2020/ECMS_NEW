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
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public MasterDB(DapperContext contextDP)
        {
            _contextDP = contextDP;
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

        public async Task<List<DTOMasterResponse>> GetPostingReason()
        {
            string query = "SELECT  Id, Reason Name FROM MPostingReason";

            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOMasterResponse>(query);
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
    }
}