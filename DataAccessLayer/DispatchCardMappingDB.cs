using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DispatchCardMappingDB: GenericRepositoryDL<TrnDispatchCardMapping>,IDispatchCardMappingDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<DispatchCardMappingDB> _logger;
        public DispatchCardMappingDB(ApplicationDbContext context, DapperContext contextDP, ILogger<DispatchCardMappingDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        public async Task<List<DTODispatchCardInRequest>> GetRequestIds(int DispatchCardId)
        {
            List<DTODispatchCardInRequest> requestCards = new List<DTODispatchCardInRequest>();
            try
            {
                string query = @"Select RequestId from TrnDispatchCardMapping WHERE DispatchCardId=@DispatchCardId";
                
                using (var connection = _contextDP.CreateConnection())
                {
                    var result = (await connection.QueryAsync<int>(query,new { DispatchCardId }));
                    // Map the result to DTODispatchCardInRequest
                    foreach (var requestId in result)
                    {
                        requestCards.Add(new DTODispatchCardInRequest { RequestId = requestId });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DispatchCardMappingDB->GetRequestIds");

            }
            return requestCards;
        }
    }
}
