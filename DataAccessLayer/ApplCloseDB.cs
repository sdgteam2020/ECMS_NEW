using Azure.Core;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

            string query = @"Select ISNULL(basi.BasicDetailId, basi2.BasicDetailId) AS BasicDetailId ,req.StatusId,ISNULL(basi.UnitId, basi2.UnitId) AS UnitId,appclose.Id as ApplCloseId from TrnICardRequest req 
                            LEFT JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
							LEFT JOIN AFSAC2.dbo.BasicDetails basi2 on req.BasicDetailId=basi2.BasicDetailId  
                            LEFT JOIN TrnApplClose appclose on appclose.RequestId = req.RequestId
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
        public async Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data, ICardHistoryResponseAll? cardHistoryResponses)
        {
            // Creating a connection and transaction for database operations.
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
           
            try
            {
                string name = (cardHistoryResponses.BasicDetail.FName + " " + cardHistoryResponses.BasicDetail.LName).Trim();
                short RankId = cardHistoryResponses.BasicDetail.RankId;
                byte ApplyForId = cardHistoryResponses.BasicDetail.ApplyForId;
                string serviceNo = (cardHistoryResponses.BasicDetail.ServiceNo ?? string.Empty).Trim();
                int? DestructedCardId = null;
                List<int> AspnetuserId = new List<int>();
                if (cardHistoryResponses != null)
                {
                    foreach (var item in cardHistoryResponses.ICardHistory)
                    {
                        if (item.StepId == 2)
                        {
                            AspnetuserId.Add(item.ToAspNetUsersId);
                        }
                    }
                }
                // Serialize the card request history to store in the database
                var cardRequestHistoryJson = JsonConvert.SerializeObject(cardHistoryResponses);

                // SQL query to insert a new record into the TrnApplClose table.
                var insertSql = @$"INSERT INTO TrnApplClose (ReasonId, Authority, Remarks, RequestId, IsActive, UpdatedOn, Updatedby, UserId,CardRequestHistoryJson,Name,RankId,ServiceNo,ApplyForId,DestructedCardId) 
                                VALUES(@ReasonId, @Authority, @Remarks, @RequestId, @IsActive, @UpdatedOn, @Updatedby, @UserId, @CardRequestHistoryJson, @Name, @RankId, @ServiceNo,@ApplyForId,@DestructedCardId);
                                {(cardHistoryResponses?.FaultyCard?.Count > 0 ? "update TrnFaultyCard set TrnFwdId = null where RequestId = @RequestId;" : "")}
                                {(cardHistoryResponses?.PostingOut?.Count > 0 ? "update TrnPostingOut set TrnFwdId = null where RequestId = @RequestId;" : "")}                                    
                                Delete from TrnFwds where RequestId = @RequestId;
                                DECLARE @Id INT = SCOPE_IDENTITY();                                
                                Select @Id;";
               
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
                parameters.Add("@CardRequestHistoryJson", cardRequestHistoryJson, DbType.AnsiString, ParameterDirection.Input, size: -1);
                parameters.Add("@Name", name, DbType.String, ParameterDirection.Input,36 );
                parameters.Add("@RankId", RankId, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@ServiceNo", serviceNo, DbType.String, ParameterDirection.Input, 10);
                parameters.Add("@ApplyForId", ApplyForId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@DestructedCardId", DestructedCardId, DbType.Int32, ParameterDirection.Input);

                // Execute the insert query asynchronously with transaction.
                Data.Id =  await db.ExecuteScalarAsync<int>(insertSql, parameters, transaction: transaction);

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
                if(AspnetuserId != null && AspnetuserId.Count > 0)
                { 
                    foreach (var item in AspnetuserId)
                    {
                        // SQL query to update the status of the RequestId in the TrnICardRequest table.
                        string query3 = "Insert into TrnApplCloseMapping(CloseId,AspNetUsersId) VALUES(@CloseId,@AspNetUsersId)";
                        var query3_parameters = new DynamicParameters();
                        query3_parameters.Add("@CloseId", Data.Id, DbType.Int32, ParameterDirection.Input);
                        query3_parameters.Add("@AspNetUsersId", item, DbType.Int32, ParameterDirection.Input);
                        // Execute the update query asynchronously with transaction.
                        await db.ExecuteAsync(query3, query3_parameters, transaction: transaction);
                    }                        
                }

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
