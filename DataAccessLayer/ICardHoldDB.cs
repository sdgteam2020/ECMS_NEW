using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    /// <summary>
    /// Repository class for handling operations related to the MTrnICardHold entity.
    /// Inherits from the GenericRepositoryDL for common CRUD operations.
    /// Implements the IICardHoldDB interface for custom database operations specific to ICardHold.
    /// </summary>
    public class ICardHoldDB : GenericRepositoryDL<MTrnICardHold>, IICardHoldDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<MTrnICardHold> _logger;

        /// <summary>
        /// Constructor for ICardHoldDB class.
        /// Initializes the context and logger for database operations and logging.
        /// </summary>
        /// <param name="context">The ApplicationDbContext for database access.</param>
        /// <param name="contextDP">The DapperContext for Dapper queries.</param>
        /// <param name="logger">The logger for logging operations.</param>
        public ICardHoldDB(ApplicationDbContext context, DapperContext contextDP, ILogger<MTrnICardHold> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }

        /// <summary>
        /// Checks if an ICardHold record exists based on the given RequestId.
        /// If the ICardHoldId is not provided (i.e., it's 0), it checks if any record with the RequestId exists.
        /// If the ICardHoldId is provided, it checks if any record with the RequestId exists but excludes the current record's ICardHoldId.
        /// </summary>
        /// <param name="dTO">The DTO containing the RequestId and optional ICardHoldId.</param>
        /// <returns>True if a matching record exists; otherwise, false.</returns>
        public async Task<bool> GetByRequestId(MTrnICardHold dTO)
        {
            if(dTO.ICardHoldId == 0)
            {
                var ret = await _context.MTrnICardHold.AnyAsync(x => x.RequestId == dTO.RequestId);
                return ret;
            }
            else
            {
                var ret = await _context.MTrnICardHold.AnyAsync(x => x.RequestId == dTO.RequestId && x.ICardHoldId != dTO.ICardHoldId);
                return ret;
            }

        }
    }
}
