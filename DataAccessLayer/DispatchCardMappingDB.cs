using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.Extensions.Logging;

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
        /// <summary>
        /// Retrieves a list of request IDs associated with a given DispatchCardId from the TrnDispatchCardMapping table.
        /// This method is used to fetch all the request IDs that are mapped to a specific Dispatch Card.
        /// </summary>
        /// <param name="DispatchCardId">The DispatchCardId used to query the TrnDispatchCardMapping table for associated request IDs.</param>
        /// <returns>A list of DTODispatchCardInRequest objects, each containing a RequestId associated with the DispatchCardId.</returns>
        /// <remarks>
        /// The method queries the TrnDispatchCardMapping table to fetch all the RequestIds linked to a specific DispatchCardId.
        /// The results are returned as a list of DTODispatchCardInRequest objects, with each object containing a RequestId.
        /// </remarks>
        public async Task<List<DTODispatchCardInRequest>> GetRequestIds(int DispatchCardId)
        {
            List<DTODispatchCardInRequest> requestCards = new List<DTODispatchCardInRequest>();
            try
            {
                // SQL query to fetch RequestIds associated with the provided DispatchCardId
                string query = @"Select RequestId from TrnDispatchCardMapping WHERE DispatchCardId=@DispatchCardId";
                
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query and retrieve the result
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
                // Log any errors encountered during the database operation
                _logger.LogError(1001, ex, "DispatchCardMappingDB->GetRequestIds");

            }
            return requestCards;
        }
    }
}
