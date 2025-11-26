using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class RankDB : GenericRepositoryDL<MRank>, IRankDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<RankDB> _logger;
        public RankDB(ApplicationDbContext context, ILogger<RankDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _logger = logger;
            _contextDP = contextDP;
        }
        /// <summary>
        /// Checks if a rank exists with the same abbreviation or name, excluding the specified rank ID.
        /// </summary>
        /// <param name="Dto">The rank object containing the RankAbbreviation and RankName to check against.</param>
        /// <returns>Returns true if a matching rank is found, false otherwise.</returns>
        public async Task<bool> GetByName(MRank Dto)
         {
            var ret = await _context.MRank.AnyAsync(p =>( p.RankAbbreviation.ToUpper() == Dto.RankAbbreviation.ToUpper() || p.RankName.ToUpper() == Dto.RankName) && p.RankId != Dto.RankId);
            return ret;
        }


        /// <summary>
        /// Retrieves the next available order by value for the ranks.
        /// </summary>
        /// <returns>Returns the next order value as a short (short + 1).</returns>
        public async Task<short> GetByMaxOrder()
        {
            short ret = await _context.MRank.MaxAsync(P => P.Orderby);
            return (short)(ret + 1);
        }


        /// <summary>
        /// Retrieves the RankId based on the given OrderBy value.
        /// </summary>
        /// <param name="OrderBy">The OrderBy value used to look up the RankId.</param>
        /// <returns>Returns the RankId associated with the given OrderBy, or a default value if not found.</returns>
        public async Task<short> GetRankIdbyOrderby(short OrderBy)
        {
            var ret= await _context.MRank.Where(P => P.Orderby == OrderBy).Select(c=>c.RankId).SingleOrDefaultAsync(); 
            return ret;
        }

        /// <summary>
        /// Retrieves all ranks ordered by their OrderBy value.
        /// </summary>
        /// <returns>Returns an enumerable list of MRank objects ordered by Orderby.</returns>
        public async Task<IEnumerable<MRank>> GetAllByorder()
        {
            var ret=  await _context.MRank.OrderBy(x => x.Orderby).ToListAsync();   
            return ret;
        }


        /// <summary>
        /// Retrieves all ranks by type, filtering active ones and ordered by Orderby.
        /// </summary>
        /// <param name="Type">The type used to filter ranks based on ApplyForId.</param>
        /// <returns>Returns an enumerable list of MRank objects filtered by Type and ordered by Orderby.</returns>
        public async Task<IEnumerable<MRank>> GetAllByType(int Type)
        {
            var ret=  await _context.MRank.Where(x => x.ApplyForId==Type && x.IsActive==true).OrderBy(x=>x.Orderby).ToListAsync();   
            return ret;
        }


        /// <summary>
        /// Checks if a RankId is referenced in any related foreign key tables (BasicDetails, BasicDetailTemps, and UserProfile).
        /// </summary>
        /// <param name="RankId">The RankId to check for references in foreign key tables.</param>
        /// <returns>Returns a DTORankIdCheckInFKTableResponse object containing counts of related entries in various tables.</returns> 
        public async Task<DTORankIdCheckInFKTableResponse?> RankIdCheckInFKTable(short RankId)
        {
            try
            {
                string query = "Select  count(distinct bd.BasicDetailId) as TotalBD, count(distinct bdt.BasicDetailTempId) as TotalBDT, count(distinct up.UserId) as TotalUP from MRank mrk" +
                                " left join BasicDetails bd on bd.RankId =mrk.RankId " +
                                " left join BasicDetailTemps bdt on bdt.RankId = mrk.RankId " +
                                " left join UserProfile up on up.RankId = mrk.RankId " +
                                " where mrk.RankId =@RankId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTORankIdCheckInFKTableResponse>(query, new { RankId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RankDB->RankIdCheckInFKTable");
                return null;
            }
        }
    }
}