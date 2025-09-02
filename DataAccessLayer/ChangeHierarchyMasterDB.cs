using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using Microsoft.Extensions.Logging;
using System.Data;

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


        /// <summary>
        /// Updates the command (`ComdId`) associated with the given Corps (`CorpsId`) and propagates the changes to the related entities.
        /// This method performs an update on the `MCorps`, `MBde`, `MDiv`, and `MapUnit` tables.
        /// </summary>
        /// <param name="Data">An instance of `MapUnit` containing the new `ComdId`, `CorpsId`, and other necessary data.</param>
        /// <returns>
        /// Returns `true` if the update is successful; otherwise, `false`.
        /// </returns>
        /// <remarks>
        /// This method performs the following updates:
        /// 1. Updates the `MCorps` table to set the new `ComdId` and related fields like `CorpsName`, `IsActive`, etc.
        /// 2. Propagates the updated `ComdId` to related `MBde`, `MDiv`, and `MapUnit` tables where the `CorpsId` is referenced.
        /// 3. Uses transactions to ensure that all updates are applied atomically. If any update fails, the transaction is rolled back.
        /// </remarks>
        public async Task<bool> UpdateChageComdByCorps(MapUnit Data)
        {
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            try
            {
                // Prepare the parameters for the SQL queries
                var parameters = new DynamicParameters();
                parameters.Add("@CorpsId", Data.Corps.CorpsId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@CorpsName", Data.Corps.CorpsName, DbType.String, ParameterDirection.Input, 20);
                parameters.Add("@ComdId", Data.ComdId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.Corps.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Corps.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.Corps.UpdatedOn, DbType.DateTime, ParameterDirection.Input);

                // Perform the database operations using the parameters
                using (var connection = _contextDP.CreateConnection())
                {
                    // Update the MCorps table
                    await db.ExecuteAsync("update MCorps set CorpsName=@CorpsName,ComdId=@ComdId,IsActive=@IsActive,Updatedby=@Updatedby,UpdatedOn=@UpdatedOn where CorpsId=@CorpsId", parameters, transaction: transaction);

                    // Update the related tables with the new ComdId and CorpsId
                    await db.ExecuteAsync("update MBde set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId }, transaction: transaction);
                    await db.ExecuteAsync("update MDiv set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId }, transaction: transaction);
                    await db.ExecuteAsync("update MapUnit set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId }, transaction: transaction);

                    // Commit the transaction if all operations succeed
                    transaction.Commit();
                    return true;  // Indicating success
                }


            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "ChangeHierarchyMasterDB->UpdateChageComdByCorps");
                return false; // Indicating failure
            }
            finally
            {
                // Dispose of the database connection to release resources
                db.Dispose();
            }
        }


        /// <summary>
        /// Updates the DIV, command, and corps relationships in the database.
        /// This method performs the following updates:
        /// - Updates the DIV (`MDiv`) table with new DIV name, command ID, and corps ID.
        /// - Updates the brigade (`MBde`) table to reflect the changes in command and corps IDs.
        /// - Updates the map unit (`MapUnit`) table to reflect the changes in command and corps IDs.
        /// The method uses a transaction to ensure atomicity of the operations. If any update fails, the transaction is rolled back.
        /// </summary>
        /// <param name="Data">The MapUnit data containing the DIV, command, corps, and other related information.</param>
        /// <returns>
        /// Returns `true` if all operations succeed, `false` if an error occurs during the process.
        /// </returns>
        /// <remarks>
        /// This method is used to update hierarchical relationships (DIV, command, and corps) when changes occur in the DIV (MDiv) table.
        /// It ensures that the related tables (`MBde` and `MapUnit`) are also updated to maintain consistency.
        /// </remarks>
        public async Task<bool> UpdateComdCorpsByDivs(MapUnit Data)
        {
            // Create a new connection and transaction
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            try
            {
                // Prepare the parameters for the SQL queries
                var parameters = new DynamicParameters();
                parameters.Add("@DivId", Data.Div.DivId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@DivName", Data.Div.DivName, DbType.String, ParameterDirection.Input,20);
                parameters.Add("@ComdId", Data.ComdId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@CorpsId", Data.CorpsId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@DivId", Data.DivId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.Div.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Div.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.Div.UpdatedOn, DbType.DateTime, ParameterDirection.Input);

                // Update the MDiv table with new DIV information
                await db.ExecuteAsync("update MDiv set DivName=@DivName,ComdId=@ComdId,CorpsId=@CorpsId,IsActive=@IsActive,Updatedby=@Updatedby,UpdatedOn=@UpdatedOn where DivId=@DivId", parameters, transaction: transaction);

                // Update the MBde table with the new command and corps information
                await db.ExecuteAsync("update MBde set ComdId=@ComdId,CorpsId=@CorpsId where DivId=@DivId", new { comdId = Data.ComdId, corpsId = Data.CorpsId, divId = Data.DivId },transaction:transaction);

                // Update the MapUnit table with the new command and corps information
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


        /// <summary>
        /// Updates the Brigade (BDE), Command (Comd), Corps, and DIV (Div) information in the database.
        /// This method performs a transaction to update related tables such as `MBde` and `MapUnit` based on the provided data.
        /// It ensures data consistency by using a transaction to commit the changes across the affected tables.
        /// </summary>
        /// <param name="Data">The `MapUnit` object containing the Brigade, Command, Corps, and DIV data to be updated.</param>
        /// <returns>
        /// Returns `true` if the operation is successful and the transaction is committed. 
        /// Returns `false` if an error occurs during the update process and the transaction is rolled back.
        /// </returns>
        /// <remarks>
        /// This method updates the following tables:
        /// - `MBde`: Updates the `BdeName`, `ComdId`, `CorpsId`, `DivId`, `IsActive`, and other relevant fields.
        /// - `MapUnit`: Updates the `ComdId`, `CorpsId`, and `DivId` based on the `BdeId`.
        /// 
        /// A transaction is used to ensure that both the `MBde` and `MapUnit` updates are either both committed or both rolled back in case of an error.
        /// </remarks>
        public async Task<bool> UpdateComdCorpsDivsBybdes(MapUnit Data)
        {
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@BdeId", Data.Bde.BdeId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@BdeName", Data.Bde.BdeName, DbType.AnsiString, ParameterDirection.Input, 20);
                parameters.Add("@ComdId", Data.ComdId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@CorpsId", Data.CorpsId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@DivId", Data.DivId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.Bde.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Bde.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.Bde.UpdatedOn, DbType.DateTime, ParameterDirection.Input);

                // Update MBde with new values
                await db.ExecuteAsync("update MBde set BdeName=@BdeName,ComdId=@ComdId,CorpsId=@CorpsId,DivId=@DivId,IsActive=@IsActive,Updatedby=@Updatedby,UpdatedOn=@UpdatedOn where BdeId=@BdeId", parameters, transaction: transaction);

                // Update MapUnit with new values
                await db.ExecuteAsync("update MapUnit set ComdId=@ComdId,CorpsId=@CorpsId,DivId=@DivId where BdeId=@BdeId", new { comdId = Data.ComdId, corpsId = Data.CorpsId, divId = Data.DivId, bdeId = Data.BdeId }, transaction: transaction);

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
    }
}
