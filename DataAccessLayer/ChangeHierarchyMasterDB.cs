using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class ChangeHierarchyMasterDB: IChangeHierarchyMasterDB
    {
        private readonly DapperContext _contextDP;
        private readonly ILogger<ChangeHierarchyMasterDB> _logger;
        public ChangeHierarchyMasterDB(DapperContext dapperContext, ILogger<ChangeHierarchyMasterDB> logger)
        {
            _contextDP = dapperContext;
            _logger = logger;
        }
        public async Task<int> UpdateChageComdByCorps(MapUnit Data)
        {
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    connection.Execute("update MBde set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId });
                    connection.Execute("update MDiv set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId });
                    connection.Execute("update MapUnit set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId });
                }


            }
            catch (Exception ex)
            {

            }

            return 1;
        }
        public async Task<bool> UpdateComdCorpsByDivs(MapUnit Data)
        {
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@BdeId", Data.Bde.BdeId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@BdeName", Data.Bde.BdeName, DbType.String, ParameterDirection.Input,20);
                parameters.Add("@ComdId", Data.ComdId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@CorpsId", Data.CorpsId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@DivId", Data.DivId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                
                await db.ExecuteAsync("update MBde set BdeName=@BdeName,ComdId=@ComdId,CorpsId=@CorpsId,DivId=@DivId,IsActive=@IsActive,Updatedby=@Updatedby,UpdatedOn=@UpdatedOn where BdeId=@BdeId", parameters, transaction: transaction);
                await db.ExecuteAsync("update MBde set ComdId=@ComdId,CorpsId=@CorpsId where DivId=@DivId", new { comdId = Data.ComdId, corpsId = Data.CorpsId, divId = Data.DivId },transaction:transaction);
                await db.ExecuteAsync("update MapUnit set ComdId=@ComdId,CorpsId=@CorpsId where DivId=@DivId", new { comdId = Data.ComdId, corpsId = Data.CorpsId, divId = Data.DivId }, transaction: transaction);
                // Commit the transaction if all operations succeed
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "ChangeHierarchyMasterDB->UpdateComdCorpsByDivs");
                return false;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }
        public async Task<int> UpdateComdCorpsDivsBybdes(MapUnit Data)
        {
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                   
                    connection.Execute("update MapUnit set ComdId=@ComdId,CorpsId=@CorpsId,DivId=@DivId where BdeId=@BdeId", new { comdId = Data.ComdId, corpsId = Data.CorpsId, divId = Data.DivId, bdeId=Data.BdeId });
                }


            }
            catch (Exception ex) { }

            return 1;
        }
    }
}
