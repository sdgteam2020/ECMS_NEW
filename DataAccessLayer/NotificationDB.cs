using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;

namespace DataAccessLayer
{
    public class NotificationDB : GenericRepositoryDL<MTrnNotification>, INotificationDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        public NotificationDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;


        }
        private readonly IConfiguration configuration;

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

        public async Task<bool> UpdatePrevious(MTrnNotification Data)
        {

            string query = "UPDATE TrnNotification set [Read]=1 where RequestId=@RequestId";

            using (var connection = _contextDP.CreateConnection())
            {
              
                int RequestId = Data.RequestId;
                var ret = await connection.QueryAsync<string>(query, new { RequestId });



                return true;
            }
        }
    }
}