using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    /// <summary>
    /// Repository class for handling operations related to the MTrnICardHold entity.
    /// Inherits from the GenericRepositoryDL for common CRUD operations.
    /// Implements the IICardHoldDB interface for custom database operations specific to ICardHold.
    /// </summary>
    public class ICardHoldDB : GenericRepositoryDL<MTrnICardHold>, IICardHoldDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<MTrnICardHold> _logger;

        /// <summary>
        /// Constructor for ICardHoldDB class.
        /// Initializes the context and logger for database operations and logging.
        /// </summary>
        /// <param name="context">The ApplicationDbContext for database access.</param>
        /// <param name="contextDP">The DapperContext for Dapper queries.</param>
        /// <param name="logger">The logger for logging operations.</param>
        public ICardHoldDB(ApplicationDbContext context, DapperContext contextDP, ILogger<MTrnICardHold> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }

        /// <summary>
        /// Checks if an ICardHold record exists based on the given RequestId.
        /// If the ICardHoldId is not provided (i.e., it's 0), it checks if any record with the RequestId exists.
        /// If the ICardHoldId is provided, it checks if any record with the RequestId exists but excludes the current record's ICardHoldId.
        /// </summary>
        /// <param name="dTO">The DTO containing the RequestId and optional ICardHoldId.</param>
        /// <returns>True if a matching record exists; otherwise, false.</returns>
        public async Task<bool> GetByRequestId(MTrnICardHold dTO)
        {
            if(dTO.ICardHoldId == 0)
            {
                var ret = await _context.MTrnICardHold.AnyAsync(x => x.RequestId == dTO.RequestId);
                return ret;
            }
            else
            {
                var ret = await _context.MTrnICardHold.AnyAsync(x => x.RequestId == dTO.RequestId && x.ICardHoldId != dTO.ICardHoldId);
                return ret;
            }

        }
        public async Task<DTOBeforeSaveICardRequestHoldResponse> CheckBeforeICardRequestHold(MTrnICardHold hold)
        {
            var response = new DTOBeforeSaveICardRequestHoldResponse();
            try
            {
                string query = "";
                if (hold.ICardHoldId > 0)
                {
                    query = @"SELECT currentReq.RequestId,hold.HoldReason, 1 as Result, 'Valid' as Message FROM MTrnICardHold hold
                                LEFT JOIN TrnICardRequest currentReq on hold.RequestId=currentReq.RequestId
                                WHERE hold.ICardHoldId = @ICardHoldId";
                }
                else
                {
                    query = @"SELECT currentReq.RequestId,
                                CASE
                                    WHEN currentReq.StatusId IN (2,3) THEN 0
                                    WHEN hold.RequestId = @RequestId THEN 0
                                    ELSE 1
                                END AS Result,
		                        case
                                    WHEN currentReq.StatusId IN (2,3) THEN 'The application is no longer active.'
                                    WHEN hold.RequestId = @RequestId THEN 'The application already exists in the ICardHold table.'
		                            ELSE 'Valid'
		                        END as Message
                            FROM TrnICardRequest currentReq
                            INNER JOIN TrnStepCounter stepcount on currentReq.RequestId=stepcount.RequestId
                            LEFT JOIN MTrnICardHold hold on currentReq.RequestId=hold.RequestId
                            WHERE currentReq.RequestId = @RequestId";
                }

                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ICardHoldId", hold.ICardHoldId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@RequestId", hold.RequestId, DbType.Int32, ParameterDirection.Input);

                    var result = await connection.QueryFirstOrDefaultAsync<DTOBeforeSaveICardRequestHoldResponse>(query, parameters);

                    if (result != null)
                    {
                        response.RequestId = result.RequestId;
                        response.HoldReason = hold.ICardHoldId > 0 ? result.HoldReason : hold.HoldReason;
                        response.Result = result.Result;
                        response.Message = result.Message;
                    }
                    else
                    {
                        response.Result = false;
                        response.Message = hold.ICardHoldId > 0 ? "Id not found." : "Invalid Application Id";
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "ICardHoldDB->CheckBeforeICardRequestHold");
                response.Result = false;
                response.Message = "Something went wrong";
            }
            return response;
        }
    }
}
