using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class RegimentalDB : GenericRepositoryDL<MRegimental>, IRegimentalDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<RegimentalDB> _logger;
        public RegimentalDB(ApplicationDbContext context, DapperContext contextDP, ILogger<RegimentalDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        /// <summary>
        /// Checks if a regimental name or abbreviation already exists in the database, excluding the current record.
        /// </summary>
        /// <param name="Dto">The regimental data to check.</param>
        /// <returns>Returns <c>true</c> if a matching record exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> GetByName(MRegimental Dto)
        {
            List<MRegimental> mRegimentals = await _context.MRegimental.AsNoTracking().ToListAsync();
            var ret = mRegimentals.Any(x => (x.Name.ToUpper() == Dto.Name.ToUpper() || x.Abbreviation.ToUpper() == Dto.Abbreviation.ToUpper()) && x.RegId != Dto.RegId);
            return ret;
        }


        /// <summary>
        /// Retrieves a list of regimentals for a specific armed type by its ArmedId.
        /// </summary>
        /// <param name="ArmedId">The ArmedId to filter the regimentals.</param>
        /// <returns>Returns a list of <see cref="DTORegimentalResponse"/> objects for the specified ArmedId.</returns>
        public async Task<List<DTORegimentalResponse>> GetByArmedId(byte ArmedId)
        {
            var data = await (from a in _context.MArmedType.AsNoTracking()
                               join r in _context.MRegimental.AsNoTracking()
                               on a.ArmedId equals r.ArmedId
                               where r.ArmedId == ArmedId
                               select new DTORegimentalResponse
                               {
                                   RegId = r.RegId,
                                   Name = r.Name,
                               }).ToListAsync();
            return data;
        }


        /// <summary>
        /// Retrieves all regimental data including regiment name, location, abbreviation, and associated unit details.
        /// </summary>
        /// <returns>Returns a list of <see cref="DTORegimentalResponse"/> containing all regimental records.</returns>
        /// <exception cref="Exception">Throws an exception if there is an error during the database query execution.</exception>
        public async Task<List<DTORegimentalResponse>> GetAllData()
        {
            try
            {
                string query = "";
                query = @"Select mreg.RegId,mreg.Name,mreg.Location,mreg.Abbreviation,mreg.UnitId,marmed.ArmedId,marmed.ArmedName, munit.Sus_no,munit.Suffix,munit.Abbreviation AS UnitAbbreviation,munit.UnitName
                        from MRegimental mreg
                        inner join MArmedType marmed on marmed.ArmedId=mreg.ArmedId
                        left join MapUnit mapunit on mapunit.UnitMapId = mreg.UnitId
                        left join MUnit munit on munit.UnitId =mapunit.UnitId order by mreg.RegId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTORegimentalResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RegimentalDB->GetAllData");
                return new List<DTORegimentalResponse>() ;
            }
        }
    }
}