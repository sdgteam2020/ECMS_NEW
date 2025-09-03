using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer
{
    public class TrnLoginLogDB : ITrnLoginLogDB
    {
        private readonly DapperContextDb2 _contextDP2;
        private readonly DapperContext _context;
        public TrnLoginLogDB(DapperContextDb2 contextDP2, DapperContext context) 
        {
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
            using (var connection = _contextDP2.CreateConnection())
            {
                await connection.ExecuteAsync("INSERT INTO [dbo].[TrnLogin_Log]([AspNetUsersId],[UserId],[IP],[IsActive],[Updatedby],[UpdatedOn],[RoleId]) VALUES (@AspNetUsersId,@UserId,@IP,@IsActive,@Updatedby,@UpdatedOn,@RoleId)", new { Data.AspNetUsersId,Data.UserId,Data.IP,Data.IsActive,Data.Updatedby,Data.UpdatedOn,Data.RoleId });
                return true;
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
                if(Data.Id==0)
                    await connection.ExecuteAsync("INSERT INTO [dbo].[XmlFilesFwdLog]([XmlFiles],[RequestId],[Updatedby],[UpdatedOn],[IsActive]) VALUES (@XmlFiles,@RequestId,@Updatedby,@UpdatedOn,@IsActive)", new { Data.XmlFiles, Data.RequestId, Data.Updatedby, Data.UpdatedOn, Data.IsActive });
                else
                    await connection.ExecuteAsync("UPDATE [dbo].[XmlFilesFwdLog] SET [XmlFiles] =@XmlFiles ,[RequestId] = @RequestId,[Updatedby] = @Updatedby,[UpdatedOn] = @UpdatedOn,[IsActive] =  @IsActive WHERE [Id]= @Id", new { Data.XmlFiles, Data.RequestId, Data.Updatedby, Data.UpdatedOn, Data.IsActive,Data.Id });
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
        public async Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId, DateTime? FmDate, DateTime? ToDate)
        {
            string query = "select logs.[Id],logs.[AspNetUsersId],logs.[UserId],logs.[IP],logs.[Updatedby],logs.[UpdatedOn],logs.[RoleId],users.DomainId,roles.Name RoleName," +
                           " ran.RankAbbreviation RankName,prof.ArmyNo,prof.Name from [AFSAC2].[dbo].TrnLogin_Log logs" +
                           " inner join AspNetUsers users on users.Id=logs.AspNetUsersId" +
                           " inner join TrnDomainMapping map on map.AspNetUsersId=users.Id" +
                           " inner join AspNetRoles roles on roles.Id=logs.[RoleId]" +
                           " inner join UserProfile prof on prof.UserId=logs.UserId" +
                           " inner join MRank ran on ran.RankId=prof.RankId" +
                           " and map.AspNetUsersId=@AspnetUserId and CAST(logs.[UpdatedOn] as Date) BETWEEN CAST(@FmDate AS DATE)  AND CAST(@ToDate AS DATE) order by logs.[UpdatedOn] desc";
            using (var connection = _context.CreateConnection())
            {
                var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { AspnetUserId, FmDate, ToDate });

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
            using (var connection = _contextDP2.CreateConnection())
            {
                await connection.ExecuteAsync("INSERT INTO [dbo].[TrnExported]([AspNetUsersId],[UserId],[IP],[CreatedBy],[CreatedOn],[RequestId]) VALUES (@AspNetUsersId,@UserId,@IP,@CreatedBy,@CreatedOn,@RequestId)", new { Data.AspNetUsersId, Data.UserId, Data.IP, Data.CreatedBy, Data.CreatedOn,Data.RequestId });
                return true;
            }
        }
    }
}
