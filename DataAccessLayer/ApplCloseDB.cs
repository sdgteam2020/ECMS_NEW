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
    public class ApplCloseDB : GenericRepositoryDL<TrnApplClose>, IApplCloseDB
    {
        private readonly DapperContext _contextDP;
        private readonly ILogger<TrnApplClose> _logger;
        public ApplCloseDB(ApplicationDbContext context, DapperContext contextDP, ILogger<TrnApplClose> logger) : base(context)
        {
            _contextDP = contextDP;
            _logger = logger;
        }

        public async Task<bool> RequestIdExists(TrnApplClose DTo)
        {
            string query = "select count(*) from TrnApplClose where RequestId=@RequestId";
            using (var connection = _contextDP.CreateConnection())
            {
                int chk = await connection.QueryFirstAsync<int>(query, new { DTo.RequestId });
                if (chk > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data)
        {
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();

            try
            {
                var insertSql = " INSERT INTO TrnApplClose (BasicDetailId, ReasonId, Authority, Remarks, RequestId, IsActive, UpdatedOn, Updatedby)" +
                                " VALUES (@BasicDetailId, @ReasonId, @Authority, @Remarks, @RequestId, @IsActive, @UpdatedOn, @Updatedby);";
                var parameters = new DynamicParameters();
                parameters.Add("@BasicDetailId", Data.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ReasonId", Data.ReasonId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@Authority", Data.Authority, DbType.String, ParameterDirection.Input, 50);
                parameters.Add("@Remarks", Data.Remarks, DbType.String, ParameterDirection.Input,50);
                parameters.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync(insertSql, parameters, transaction: transaction);

                string query1 = " UPDATE TrnICardRequest set StatusId=3 where RequestId=@RequestId ";
                var query1_parameters = new { RequestId = Data.RequestId };
                await db.ExecuteAsync(query1, query1_parameters, transaction: transaction);

                // Commit the transaction if all operations succeed
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "ApplCloseDB->ApplCloseWithUpdateStatus");
                return false;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }
    }
}
