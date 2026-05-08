using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Logging;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class TrnLoginLogDB : ITrnLoginLogDB
    {
        private readonly DapperContextDb2 _contextDP2;
        private readonly DapperContext _context;
        private readonly ILogger<TrnLoginLogDB> _logger;// For logging
        public TrnLoginLogDB(DapperContextDb2 contextDP2, DapperContext context, ILogger<TrnLoginLogDB> logger) 
        {
            _logger = logger;
            _contextDP2 = contextDP2;
            _context = context;
        }


        /// <summary>
        /// Adds a new login log entry to the TrnLogin_Log table.
        /// </summary>
        /// <param name="Data">The login log data to insert.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        public async Task<bool> Add(TrnLogin_Log Data)
        {
            try
            {
                using (var connection = _contextDP2.CreateConnection())
                {
                    await connection.ExecuteAsync("INSERT INTO [dbo].[TrnLogin_Log]([AspNetUsersId],[UserId],[IP],[IsActive],[Updatedby],[UpdatedOn],[RoleId],[LoginGuid],[ExpiresOn],[IsUsed]) VALUES (@AspNetUsersId,@UserId,@IP,@IsActive,@Updatedby,@UpdatedOn,@RoleId,@LoginGuid,@ExpiresOn,@IsUsed)", new { Data.AspNetUsersId, Data.UserId, Data.IP, Data.IsActive, Data.Updatedby, Data.UpdatedOn, Data.RoleId, Data.LoginGuid, Data.ExpiresOn, Data.IsUsed });
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnLoginLogDB->Add");
                return false;
            }
        }


        /// <summary>
        /// Inserts or updates a record in the XmlFilesFwdLog table for digital signature tracking.
        /// </summary>
        /// <param name="Data">The data for the XML file and associated forward log.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        public async Task<bool> XmlFileDigitalSign(DTOXmlFilesFwdLogRequest Data)
        {
            using (var connection = _contextDP2.CreateConnection())
            {
                string query1=string.Empty;
                if (Data.Id == 0)
                    query1 = @"INSERT INTO [dbo].[XmlFilesFwdLog]([XmlFiles],[RequestId],[Updatedby],[UpdatedOn],[IsActive]) VALUES (@XmlFiles,@RequestId,@Updatedby,@UpdatedOn,@IsActive)";
                else
                    query1 = @"UPDATE [dbo].[XmlFilesFwdLog] SET [XmlFiles] =@XmlFiles ,[RequestId] = @RequestId,[Updatedby] = @Updatedby,[UpdatedOn] = @UpdatedOn,[IsActive] =  @IsActive WHERE [Id]= @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Data.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@XmlFiles", Data.XmlFiles, DbType.String, ParameterDirection.Input);
                parameters.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);

                await connection.ExecuteAsync(query1, parameters);
                return true;
            }
        }


        /// <summary>
        /// Retrieves the XmlFile digital signature record based on the RequestId.
        /// </summary>
        /// <param name="RequestId">The array of RequestId(s) to query.</param>
        /// <returns>Returns the first matching XmlFilesFwdLog record.</returns>
        public async Task<DTOXmlFilesFwdLogRequest> XmlFileDigitalSignFromData(int[] RequestId)
        {
          
            string query = "select Id,[XmlFiles],[RequestId],[Updatedby],[UpdatedOn],[IsActive] from XmlFilesFwdLog where RequestId in @RequestId";

            using (var connection = _contextDP2.CreateConnection())
            {
                var Ret = await connection.QueryAsync<DTOXmlFilesFwdLogRequest>(query, new { RequestId });
                return Ret.FirstOrDefault();
            }
        }

        public async Task<TrnLogin_Log?> GetByToken(Guid loginGuid)
        {
            try
            {
                string query = @"select * from TrnLogin_Log where LoginGuid=@LoginGuid";

                using (var connection = _contextDP2.CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<TrnLogin_Log>(query, new { LoginGuid = loginGuid });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnLoginLogDB->GetByToken");
                return null;
            }

        }

        public async Task<bool> Update(TrnLogin_Log Data)
        {
            try
            {
                using (var connection = _contextDP2.CreateConnection())
                {
                    string query1 = @"UPDATE [dbo].[TrnLogin_Log] SET [IsUsed] =  @IsUsed WHERE [LoginGuid]= @LoginGuid";

                    var parameters = new DynamicParameters();
                    parameters.Add("@LoginGuid", Data.LoginGuid, DbType.Guid, ParameterDirection.Input);
                    parameters.Add("@IsUsed", Data.IsActive, DbType.Boolean, ParameterDirection.Input);

                    await connection.ExecuteAsync(query1, parameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnLoginLogDB->Update");
                return false;
            }

        }


        /// <summary>
        /// Retrieves all login logs for a specific unit.
        /// </summary>
        /// <param name="UnitId">The UnitId to filter the login logs by.</param>
        /// <returns>Returns a list of DTOLoginLogResponse containing login details for the specified unit.</returns>
        public async Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId)
        {
            string query = "select users.Id [AspNetUsersId],users.DomainId,roles.Name RoleName,"+
                           " ran.RankAbbreviation RankName,prof.ArmyNo,prof.Name from AspNetUsers users"+
                           " inner join TrnDomainMapping map on map.AspNetUsersId=users.Id"+
                           " inner join AspNetUserRoles urole on urole.UserId=users.Id"+
                           " inner join AspNetRoles roles on roles.Id=urole.[RoleId]"+
                           " inner join UserProfile prof on prof.UserId=map.UserId " +
                           " inner join MRank ran on ran.RankId=prof.RankId"+
                           " and map.UnitId=@UnitId";

            using (var connection = _context.CreateConnection())
            {
                var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { UnitId });

                return Ret.ToList();
            }
        }


        /// <summary>
        /// Retrieves login logs for a specific user within a date range.
        /// </summary>
        /// <param name="AspnetUserId">The AspNetUserId to filter login logs by.</param>
        /// <param name="FmDate">The start date for filtering logs.</param>
        /// <param name="ToDate">The end date for filtering logs.</param>
        /// <returns>Returns a list of DTOLoginLogResponse containing login details for the specified user within the date range.</returns>
        public async Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId, int UnitId, DateTime? FmDate, DateTime? ToDate)
        {
            string query = @"select logs.[Id],logs.[AspNetUsersId],logs.[UserId],logs.[IP],logs.[Updatedby],logs.[UpdatedOn],logs.[RoleId],users.DomainId,roles.Name RoleName,
                            ran.RankAbbreviation RankName,prof.ArmyNo,prof.Name from [AFSAC2].[dbo].TrnLogin_Log logs
                            inner join AspNetUsers users on users.Id=logs.AspNetUsersId
                            inner join TrnDomainMapping map on map.AspNetUsersId=users.Id
                            inner join AspNetRoles roles on roles.Id=logs.[RoleId]
                            inner join UserProfile prof on prof.UserId=logs.UserId
                            inner join MRank ran on ran.RankId=prof.RankId
                            and map.AspNetUsersId=@AspnetUserId and map.UnitId=@UnitId and CAST(logs.[UpdatedOn] as Date) BETWEEN CAST(@FmDate AS DATE)  AND CAST(@ToDate AS DATE) order by logs.[UpdatedOn] desc";
            using (var connection = _context.CreateConnection())
            {
                var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { AspnetUserId, UnitId, FmDate, ToDate });

                return Ret.ToList();
            }
        }

        /// <summary>
        /// Inserts a data export log entry into the TrnExported table.
        /// </summary>
        /// <param name="Data">The export log data to insert.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        public async Task<bool> AddDataExport(DTODataExported Data)
        {
            try
            {
                using (var connection = _contextDP2.CreateConnection())
                {
                    string query1 = string.Empty;
                    query1 = @"INSERT INTO [dbo].[TrnExported]([AspNetUsersId],[UserId],[IP],[CreatedBy],[CreatedOn],[RequestId]) VALUES (@AspNetUsersId,@UserId,@IP,@CreatedBy,@CreatedOn,@RequestId)";

                    var parameters = new DynamicParameters();
                    parameters.Add("@AspNetUsersId", Data.AspNetUsersId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UserId", Data.UserId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@IP", Data.IP, DbType.String, ParameterDirection.Input);
                    parameters.Add("@CreatedBy", Data.CreatedBy, DbType.AnsiString, ParameterDirection.Input, size: 100);
                    parameters.Add("@CreatedOn", Data.CreatedOn, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);

                    await connection.ExecuteAsync(query1, parameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnLoginLogDB->AddDataExport");
                return false;
            }
        }
    }
}
