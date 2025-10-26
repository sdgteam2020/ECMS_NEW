using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System.Data;
using static Dapper.SqlMapper;

namespace DataAccessLayer
{
    public class NotificationDB : GenericRepositoryDL<MTrnNotification>, INotificationDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        public NotificationDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
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
                throw ex;
            }
        }
        public async Task<bool> AddNotification(DTOTrnNotificationRequest Data)
        {
            try
            {
                string query = @"INSERT INTO TrnNotification([Read],DisplayId,SentAspNetUsersId,ReciverAspNetUsersId,Url,RequestId,StepId)
                             VALUES(@Read,@DisplayId,@SentAspNetUsersId,@ReciverAspNetUsersId,@Url,@RequestId,@StepId)";

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

                        await connection.ExecuteAsync(query, parameters);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}