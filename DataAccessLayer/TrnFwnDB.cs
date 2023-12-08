using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataAccessLayer
{
    public class TrnFwnDB : GenericRepositoryDL<MTrnFwd>, ITrnFwnDB
    {
        private readonly DapperContext _contextDP;
        protected readonly ApplicationDbContext _context;
        public TrnFwnDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> UpdateAllBYRequestId(int RequestId)
        {
            string query = "";
            using (var connection = _contextDP.CreateConnection())
            {
                connection.Execute("UPDATE TrnFwds set IsComplete=1 where RequestId=@RequestId", new { RequestId });
               
                return true;

            }
        }
    }

    
}
