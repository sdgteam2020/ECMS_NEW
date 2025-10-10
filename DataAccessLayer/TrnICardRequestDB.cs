using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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


        /// <summary>
        /// Retrieves a TrnICardRequest record based on the provided BasicDetailId.
        /// </summary>
        /// <param name="BasicDetailId">The ID of the BasicDetail record.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the TrnICardRequest record, or null if no matching record is found.</returns>
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


        /// <summary>
        /// Checks whether there is any pending TrnICardRequest for the given BasicDetailId.
        /// </summary>
        /// <param name="BasicDetailId">The ID of the BasicDetail record.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains true if there is a pending request, otherwise false.</returns>
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


        /// <summary>
        /// Checks whether there are any pending TrnICardRequests for a list of BasicDetailIds.
        /// </summary>
        /// <param name="BasicDetailId">An array of BasicDetailIds.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains true if there is at least one pending request, otherwise false.</returns>
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


        /// <summary>
        /// Retrieves the UserId associated with a given RequestId, based on the active status.
        /// </summary>
        /// <param name="RequestId">The ID of the request.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the UserId associated with the given RequestId, or a default value if not found.</returns>
        public async Task<int> GetUserIdByRequestId(int RequestId)
        {
           string query = @"Select AspNetUsersId from TrnICardRequest icard
                            inner join TrnDomainMapping map on icard.TrnDomainMappingId=map.Id
                            where RequestId=@RequestId and [StatusId]=1";
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryFirstAsync<int>(query, new { RequestId });
                return Convert.ToInt32(ret);
            }
        }

        
        /// <summary>
        /// Updates the status of a TrnICardRequest to a completed status (StatusId = 3) based on the provided RequestId.
        /// </summary>
        /// <param name="RequestId">The ID of the request to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is always true.</returns>
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
