using Azure.Core;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Asynchronously checks if a record with the given RequestId exists in the TrnApplClose table.
        /// </summary>
        /// <param name="DTo">The TrnApplClose DTO object containing the RequestId to be checked.</param>
        /// <returns>
        /// Returns true if a record with the given RequestId exists, otherwise false.
        /// </returns>
        public async Task<bool> RequestIdExists(TrnApplClose DTo)
        {
            // SQL query to count the number of records in the TrnApplClose table where the RequestId matches the provided value.
            string query = "select count(*) from TrnApplClose where RequestId = @RequestId";

            // Using the database connection to execute the query and retrieve the count of matching records.
            using (var connection = _contextDP.CreateConnection())
            {
                // Execute the query asynchronously and get the count of matching records.
                int chk = await connection.QueryFirstAsync<int>(query, new { DTo.RequestId });

                // If the count is greater than 0, return true indicating the RequestId exists.
                if (chk > 0)
                {
                    return true;
                }
                else
                {
                    // If no matching records are found, return false.
                    return false;
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
                var insertSql = "INSERT INTO TrnApplClose (BasicDetailId, ReasonId, Authority, Remarks, RequestId, IsActive, UpdatedOn, Updatedby, UserId) " +
                                "VALUES (@BasicDetailId, @ReasonId, @Authority, @Remarks, @RequestId, @IsActive, @UpdatedOn, @Updatedby, @UserId);";

                // Creating dynamic parameters for the insert query.
                var parameters = new DynamicParameters();
                parameters.Add("@BasicDetailId", Data.BasicDetailId, DbType.Int32, ParameterDirection.Input);
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

                // Execute the update query asynchronously with transaction.
                var query1_parameters = new { RequestId = Data.RequestId };
                await db.ExecuteAsync(query1, query1_parameters, transaction: transaction);

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
