using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
    }
}