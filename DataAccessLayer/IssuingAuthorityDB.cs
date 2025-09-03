using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    /// <summary>
    /// Repository class for managing the Issuing Authority entities in the database.
    /// It provides methods to interact with the MIssuingAuthority table.
    /// </summary>
    public class IssuingAuthorityDB : GenericRepositoryDL<MIssuingAuthority>, IIssuingAuthorityDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<IssuingAuthorityDB> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IssuingAuthorityDB"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="contextDP">The Dapper context for database operations.</param>
        /// <param name="logger">The logger for logging purposes.</param>
        public IssuingAuthorityDB(ApplicationDbContext context, DapperContext contextDP, ILogger<IssuingAuthorityDB> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }

        /// <summary>
        /// Retrieves a list of issuing authorities by the specified ApplyForId.
        /// </summary>
        /// <param name="ApplyForId">The ID of the ApplyFor entity used to filter the issuing authorities.</param>
        /// <returns>A list of <see cref="DTOIssuingAuthorityResponse"/> objects containing the issuing authority details.</returns>
        /// <remarks>
        /// This method performs an inner join between the MIssuingAuthority and MApplyFor tables,
        /// and retrieves the IssuingAuthorityId and IssuingAuthorityName for each matching record.
        /// </remarks>
        public async Task<List<DTOIssuingAuthorityResponse>> GetByApplyForId(byte ApplyForId)
        {
            var data = await (from m in _context.MIssuingAuthority
                               join mapp in _context.MApplyFor
                               on m.ApplyForId equals mapp.ApplyForId
                               where m.ApplyForId == ApplyForId
                               select new DTOIssuingAuthorityResponse
                               {
                                   IssuingAuthorityId = m.IssuingAuthorityId,
                                   IssuingAuthorityName = m.Name,
                               }).ToListAsync();
            return data;
        }
    }
}
