using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

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
        public async Task<DTODataTablesResponse<DTORegimentalResponse>> GetAllRegimental_Pagination(DTODataTablesRequest dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Name"] = "mreg.Name",
                ["UnitAbbreviation"] = "mreg.Abbreviation",
                ["RegId"] = "mreg.RegId",
            };
            selectFields = @"mreg.RegId,mreg.Name,mreg.Location,mreg.Abbreviation,mreg.UnitId,marmed.ArmedId,marmed.ArmedName, munit.Sus_no,munit.Suffix,munit.Abbreviation AS UnitAbbreviation,munit.UnitName";
            fromJoinClause = @"from MRegimental mreg
                                inner join MArmedType marmed on marmed.ArmedId=mreg.ArmedId
                                left join MapUnit mapunit on mapunit.UnitMapId = mreg.UnitId
                                left join MUnit munit on munit.UnitId =mapunit.UnitId";
            whereClause = @"WHERE
                                (
                                    mreg.Name LIKE '%' + @SearchTerm + '%' OR
                                    mreg.Abbreviation LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "mreg.RegId";
                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    dTO.searchValue = string.IsNullOrEmpty(dTO.searchValue) ? string.Empty : dTO.searchValue.Trim();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTORegimentalResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTORegimentalResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = records,
                    };
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RegimentalDB->GetAllArmed_Pagination");
                List<DTORegimentalResponse> dTORegimentals = new List<DTORegimentalResponse>();
                var responseData = new DTODataTablesResponse<DTORegimentalResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTORegimentals
                };
                return responseData;
            }
        }
        public async Task<bool> ValidateUnitIdInRegimental(int UnitId)
        {
            var ret = await _context.MRegimental.AnyAsync(x => x.UnitId == UnitId);
            return ret;
        }
    }
}