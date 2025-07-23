using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class TrnICardRequestDB : GenericRepositoryDL<MTrnICardRequest>, ITrnICardRequestDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<TrnICardRequestDB> _logger;
        public TrnICardRequestDB(ApplicationDbContext context, DapperContext contextDP, ILogger<TrnICardRequestDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        public async Task<MTrnICardRequest?> GetRequestByBasicDetailId(int BasicDetailId)
        {
            string query = @"Select * from TrnICardRequest where BasicDetailId = @BasicDetailId";
            MTrnICardRequest? trnICardRequest = new MTrnICardRequest();
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    trnICardRequest = await connection.QueryFirstOrDefaultAsync<MTrnICardRequest>(query, new { BasicDetailId });
                    if (trnICardRequest != null)
                    {
                        return trnICardRequest;
                    }
                    else
                    {
                        return trnICardRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnICardRequestDB->GetRequestByBasicDetailId");
                return null;
            }
        }
        public async Task<MTrnICardRequest> GetByAspNetUserBy(int AspnetuserId)
        {
            return null;// await _context.TrnICardRequest.Where(P => P.TrnDomainMappingId == AspnetuserId).ToListAsync();
        }
        public async Task<bool> GetRequestPending(int BasicDetailId)
        {
            string query = "Select count(*) from BasicDetails bd " +
                            "LEFT JOIN TrnICardRequest tr ON bd.BasicDetailId = tr.BasicDetailId WHERE bd.BasicDetailId = @BasicDetailId and tr.StatusId = 1 ";
            using (var connection = _contextDP.CreateConnection())
            {
                int PendingRequest = await connection.QueryFirstAsync<int>(query, new { BasicDetailId });
                if (PendingRequest > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<bool> GetRequestPendingUsingBasicDetailIds(int[] BasicDetailId)
        {
            string query = @"Select count(*) from TrnICardRequest tr where tr.BasicDetailId in @BasicDetailId and tr.StatusId =1";
            using (var connection = _contextDP.CreateConnection())
            {
                int PendingRequest = await connection.ExecuteScalarAsync<int>(query, new { BasicDetailId });
                if (PendingRequest > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<int> GetUserIdByRequestId(int RequestId)
        {
           string query = "Select AspNetUsersId from TrnICardRequest icard"+
                        " inner join TrnDomainMapping map on icard.TrnDomainMappingId=map.Id"+
                        " where RequestId=@RequestId and [StatusId]=1";
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
              
                var ret = await connection.QueryFirstAsync<int>(query, new { RequestId });



                return Convert.ToInt32(ret);
            }
        }

        public async Task<bool> UpdateStatus(int RequestId)
        {

            string query = "";
            using (var connection = _contextDP.CreateConnection())
            {
                connection.Execute("UPDATE TrnICardRequest set StatusId=3 where RequestId=@RequestId", new { RequestId });

                return true;

            }
        }
    }
}
