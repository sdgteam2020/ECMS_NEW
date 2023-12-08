using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class TrnICardRequestDB : GenericRepositoryDL<MTrnICardRequest>, ITrnICardRequestDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        public TrnICardRequestDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
        }
        private readonly IConfiguration configuration;

        public async Task<MTrnICardRequest> GetByAspNetUserBy(int AspnetuserId)
        {
            return null;// await _context.TrnICardRequest.Where(P => P.TrnDomainMappingId == AspnetuserId).ToListAsync();
        }
        public async Task<bool> GetRequestPending(int BasicDetailId)
        {
            string query = "Select count(*) from BasicDetails bd " +
                            "LEFT JOIN TrnICardRequest tr ON bd.BasicDetailId = tr.BasicDetailId WHERE bd.BasicDetailId = @BasicDetailId and tr.Status = 0 ";
            using (var connection = _contextDP.CreateConnection())
            {
                int PendingRequest = await connection.QueryFirstAsync<int>(query, new { BasicDetailId });
                if (PendingRequest > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
