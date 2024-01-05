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

        
    }
}