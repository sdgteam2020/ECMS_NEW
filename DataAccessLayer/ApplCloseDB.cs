using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    /// <summary>
    /// Data Access Layer for ApplClose entity, providing database operations.
    /// And implements the IApplCloseDB interface.
    /// And inherits from GenericRepositoryDL for basic CRUD operations.
    /// </summary>
    public class ApplCloseDB : GenericRepositoryDL<TrnApplClose>, IApplCloseDB
    {
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly ILogger<TrnApplClose> _logger;// For logging

        /// <summary>
        /// Constructor to initialize the ApplCloseDB with necessary contexts and logger.
        /// And calls the base class constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextDP"></param>
        /// <param name="logger"></param>
        public ApplCloseDB(ApplicationDbContext context, DapperContext contextDP, ILogger<TrnApplClose> logger) : base(context)
        {
            _contextDP = contextDP;
            _logger = logger;
        }

        public async Task<DTOApplicationCloseResponse> RequestIdExists(DTOApplicationCloseRequest DTo)
        {
            DTOApplicationCloseResponse closeResponse = new DTOApplicationCloseResponse();

            string query = @"Select basi.BasicDetailId,req.StatusId,basi.UnitId,appclose.Id as ApplCloseId from BasicDetails basi
                            inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId
                            left join TrnApplClose appclose on appclose.RequestId = req.RequestId
                            where req.RequestId=@RequestId";

            using (var connection = _contextDP.CreateConnection())
            {
                closeResponse = await connection.QueryFirstAsync<DTOApplicationCloseResponse>(query, new { DTo.RequestId, DTo.UnitId });

                if (closeResponse != null)
                {
                    if (closeResponse.StatusId == 1 && closeResponse.UnitId == DTo.UnitId && closeResponse.ApplCloseId == null)
                    {
                        closeResponse.Result = true;
                        closeResponse.Message = "Ok";
                        return closeResponse;
                    }
                    else
                    {
                        if (closeResponse.StatusId != 1)
                        {
                            closeResponse.Message = "Appl Allready Complete / Closed!";
                        }
                        else if (closeResponse.UnitId != DTo.UnitId)
                        {
                            closeResponse.Message = "You are not authorized to closed this request.";
                        }
                        else if (closeResponse.ApplCloseId != null)
                        {
                            closeResponse.Message = "Appl Allready Closed!";
                        }
                        closeResponse.Result = false;
                        return closeResponse;
                    }
                }
                else
                {
                    closeResponse.Result = false;
                    closeResponse.Message = "Invalid Input";
                    return closeResponse;
                }
            }
        }

        /// <summary>
        /// Asynchronously inserts a record into the TrnApplClose table and updates the status of the corresponding 
        /// RequestId in the TrnICardRequest table within a transaction. If any operation fails, the transaction is rolled back.
        /// </summary>
        /// <param name="Data">The TrnApplClose object containing the data to be inserted and used for updating the status.</param>
        /// <returns>
        /// Returns true if the insert and update operations succeed, otherwise returns false.
        /// </returns>
        public async Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data)
        {
            // Creating a connection and transaction for database operations.
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();

            try
            {
                // SQL query to insert a new record into the TrnApplClose table.
                var insertSql = "INSERT INTO TrnApplClose (ReasonId, Authority, Remarks, RequestId, IsActive, UpdatedOn, Updatedby, UserId) " +
                                "VALUES (@ReasonId, @Authority, @Remarks, @RequestId, @IsActive, @UpdatedOn, @Updatedby, @UserId);";

                // Creating dynamic parameters for the insert query.
                var parameters = new DynamicParameters();
                parameters.Add("@ReasonId", Data.ReasonId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@Authority", Data.Authority, DbType.String, ParameterDirection.Input, 50);
                parameters.Add("@Remarks", Data.Remarks, DbType.String, ParameterDirection.Input, 50);
                parameters.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UserId", Data.UserId, DbType.Int32, ParameterDirection.Input);

                // Execute the insert query asynchronously with transaction.
                await db.ExecuteAsync(insertSql, parameters, transaction: transaction);

                // SQL query to update the status of the RequestId in the TrnICardRequest table.
                string query1 = "UPDATE TrnICardRequest SET StatusId = 3 WHERE RequestId = @RequestId";
                
                var query1_parameters = new DynamicParameters();
                query1_parameters.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);

                // Execute the update query asynchronously with transaction.
                await db.ExecuteAsync(query1, query1_parameters, transaction: transaction);

                // SQL query to update the status of the RequestId in the TrnICardRequest table.
                string query2 = "UPDATE TrnNotification SET [Read] = 1 WHERE RequestId = @RequestId  AND [Read] = 0";

                var query2_parameters = new DynamicParameters();
                query2_parameters.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);

                // Execute the update query asynchronously with transaction.
                await db.ExecuteAsync(query2, query2_parameters, transaction: transaction);

                // Commit the transaction if both operations succeed.
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails.
                transaction.Rollback();
                _logger.LogError(1001, ex, "ApplCloseDB->ApplCloseWithUpdateStatus");
                return false;
            }
            finally
            {
                // Dispose of the connection to free up resources.
                db.Dispose();
            }
        }

    }
}
