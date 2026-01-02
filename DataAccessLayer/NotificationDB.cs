using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Logging;
using System.Data;
using static Dapper.SqlMapper;

namespace DataAccessLayer
{
    public class NotificationDB : GenericRepositoryDL<MTrnNotification>, INotificationDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<NotificationDB> _logger;
        public NotificationDB(ApplicationDbContext context, DapperContext contextDP, ILogger<NotificationDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        /// <summary>
        /// Updates the "Read" status of a notification for a specific user and display ID.
        /// </summary>
        /// <param name="Data">The notification data containing UserId and DisplayId.</param>
        /// <returns>Returns a boolean indicating if the operation was successful.</returns>
        public async Task<bool> UpdateRead(MTrnNotification Data)
        {

            string query = "UPDATE TrnNotification set [Read]=1 where ReciverAspNetUsersId=@UserId and DisplayId=@DisplayId";

            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                int UserId = Data.ReciverAspNetUsersId;
                int DisplayId = Data.DisplayId;
                var ret = await connection.QueryAsync<string>(query, new { UserId, DisplayId });

                return true;
            }
        }


        /// <summary>
        /// Updates the "Read" status of a notification based on the RequestId.
        /// </summary>
        /// <param name="Data">The notification data containing RequestId.</param>
        /// <returns>Returns a boolean indicating if the operation was successful.</returns>
        public async Task<bool> UpdatePrevious(DTOTrnNotificationRequest Data)
        {
            try
            {
                string query = "UPDATE TrnNotification set [Read]=1 where RequestId=@RequestId";

                using (var connection = _contextDP.CreateConnection())
                {
                    foreach (var requestId in Data.RequestIds)
                    {
                        int RequestId = requestId;
                        await connection.ExecuteAsync(query, new { RequestId });
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "NotificationDB->UpdatePrevious");
                return false;
            }
        }
        public async Task<bool> AddNotification(DTOTrnNotificationRequest Data)
        {
            try
            {
                string query = @"INSERT INTO TrnNotification([Read],DisplayId,SentAspNetUsersId,ReciverAspNetUsersId,Url,RequestId,StepId,UpdatedOn)
                             VALUES(@Read,@DisplayId,@SentAspNetUsersId,@ReciverAspNetUsersId,@Url,@RequestId,@StepId,@UpdatedOn)";

                using (var connection = _contextDP.CreateConnection())
                {
                    foreach (var requestId in Data.RequestIds)
                    {
                        int RequestId = requestId;

                        var parameters = new DynamicParameters();
                        parameters.Add("@Read", Data.Read, DbType.Boolean, ParameterDirection.Input);
                        parameters.Add("@DisplayId", Data.DisplayId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@SentAspNetUsersId", Data.SentAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ReciverAspNetUsersId", Data.ReciverAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Url", Data.Url, DbType.String, ParameterDirection.Input);
                        parameters.Add("@RequestId", RequestId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@StepId", Data.StepId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);

                        await connection.ExecuteAsync(query, parameters);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "NotificationDB->AddNotification");
                return false;
            }

        }
        public async Task<DTODataTablesResponse<DTONotificationResponse>> GetAllNotificationData(DTODataTablesRequestForNotification dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ServiceNo"] = "ServiceNo",
                ["ApplId"] = "tre.RequestId"
            };

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";
            selectFields = @"tre.RequestId as ApplId,noti.UpdatedOn,Message,ranks.RankAbbreviation,bas.FName,bas.LName,bas.ServiceNo,uplod.PhotoImagePath,dis.Url";

            fromJoinClause = @"from TrnNotification noti
                                inner join TrnNotificationDisplay dis on noti.DisplayId=dis.DisplayId
                                inner join AspNetUsers users on users.Id=noti.SentAspNetUsersId
                                inner join TrnStepCounter stepc on stepc.RequestId=noti.RequestId 
                                inner join TrnICardRequest tre on tre.RequestId = noti.RequestId 
                                inner join BasicDetails bas on bas.BasicDetailId=tre.BasicDetailId
                                inner join MRank ranks on ranks.RankId=bas.RankId
                                inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId";

            whereClause = @"WHERE
                            noti.ReciverAspNetUsersId=@ReciverAspNetUsersId 
                            AND [Read]=0
                            AND
                            ( (@SearchTerm IS NULL) OR (bas.ServiceNo LIKE @SearchTerm OR tre.RequestId LIKE @SearchTerm))";

            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "bas.ServiceNo";

                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    var searchTerm = string.IsNullOrEmpty(dTO.searchValue) ? null : $"%{dTO.searchValue}%";

                    var parameters = new DynamicParameters();
                    parameters.Add("@ReciverAspNetUsersId", dTO.ReciverAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", searchTerm, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTONotificationResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var allrecord = (from e in records
                                     select new DTONotificationResponse()
                                     {
                                         TotalFilteredRecords = e.TotalFilteredRecords,
                                         PhotoImagePath = e.PhotoImagePath,
                                         ApplId = e.ApplId,
                                         ServiceNo = e.ServiceNo,
                                         RankAbbreviation = e.RankAbbreviation,
                                         FName = e.FName,
                                         LName = e.LName,
                                         UpdatedOn = e.UpdatedOn,
                                         Message = e.Message,
                                         Url= e.Url,
                                     }).ToList();
                    var responseData = new DTODataTablesResponse<DTONotificationResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = allrecord,
                    };
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "NotificationDB->GetAllNotificationData");
                List<DTONotificationResponse> detailVMs = new List<DTONotificationResponse>();
                var responseData = new DTODataTablesResponse<DTONotificationResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = detailVMs
                };
                return responseData;
            }
        }
    }
}