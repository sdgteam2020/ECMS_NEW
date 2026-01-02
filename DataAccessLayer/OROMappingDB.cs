using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    public class OROMappingDB : GenericRepositoryDL<OROMapping>, IOROMappingDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<OROMappingDB> _logger;
        public OROMappingDB(ApplicationDbContext context, DapperContext contextDP, ILogger<OROMappingDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        /// <summary>
        /// Gets a boolean value indicating whether there is any existing record in the OROMapping table
        /// that has a different OROMappingId than the one specified in the DTO.
        /// </summary>
        /// <param name="Dto">The DTO object containing the OROMappingId to compare.</param>
        /// <returns>A boolean value indicating whether a record exists with a different OROMappingId.</returns>
        public async Task<bool> GetByName(OROMapping Dto)
        {
            var ret =  await _context.OROMapping.AnyAsync(x => x.OROMappingId != Dto.OROMappingId);
            return ret;
        }

        /// <summary>
        /// Retrieves all OROMapping records along with related details like Rank, Record Office, and Unit information.
        /// </summary>
        /// <returns>A list of DTOOROMappingResponse objects containing OROMapping and related data.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the database query.</exception>
        public async Task<List<DTOOROMappingResponse>?> GetAllOROMapping()
        {
            try
            {
                string query = "";
                query = @"Select oromap.OROMappingId,oromap.ArmedIdList,oromap.RankId,mrak.RankName,mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,oromap.TDMId,oromap.UnitId,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName
                        ,(select STRING_AGG(ArmedName,'#') from MArmedType where ArmedId in (select value from string_split(oromap.ArmedIdList,','))) ArmNameList from OROMapping oromap
                        inner join MRecordOffice mrecord on mrecord.RecordOfficeId=oromap.RecordOfficeId
                        left join MRank mrak on mrak.RankId=oromap.RankId
                        left join TrnDomainMapping trndomain on trndomain.Id=oromap.TDMId
                        left join AspNetUsers users on users.Id=trndomain.AspNetUsersId
                        left join UserProfile usep on usep.UserId=trndomain.UserId
                        left join MRank ra on ra.RankId=usep.RankId 
                        left join MapUnit mapunit on mapunit.UnitMapId = oromap.UnitId 
                        left join MUnit munit on munit.UnitId =mapunit.UnitId order by oromap.OROMappingId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOOROMappingResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "OROMappingDB->GetAllOROMapping");
                return null;
            }

        }
        public async Task<DTODataTablesResponse<DTOOROMappingResponse>> GetAllOROMapping_Pagination(DTODataTablesRequest dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["RecordOfficeName"] = "mrecord.Name",
                ["OROMappingId"] = "oromap.OROMappingId",
            };
            selectFields = @"oromap.OROMappingId,oromap.ArmedIdList,oromap.RankId,mrak.RankName,mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,oromap.TDMId,oromap.UnitId,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName
                            ,(select STRING_AGG(ArmedName,'#') from MArmedType where ArmedId in (select value from string_split(oromap.ArmedIdList,','))) ArmNameList";
            fromJoinClause = @"from OROMapping oromap
                                inner join MRecordOffice mrecord on mrecord.RecordOfficeId=oromap.RecordOfficeId
                                left join MRank mrak on mrak.RankId=oromap.RankId
                                left join TrnDomainMapping trndomain on trndomain.Id=oromap.TDMId
                                left join AspNetUsers users on users.Id=trndomain.AspNetUsersId
                                left join UserProfile usep on usep.UserId=trndomain.UserId
                                left join MRank ra on ra.RankId=usep.RankId 
                                left join MapUnit mapunit on mapunit.UnitMapId = oromap.UnitId 
                                left join MUnit munit on munit.UnitId =mapunit.UnitId";
            whereClause = @"WHERE
                                (
                                    mrecord.Name LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "oromap.OROMappingId";
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
                    var records = (await ret.ReadAsync<DTOOROMappingResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTOOROMappingResponse>
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
                _logger.LogError(1001, ex, "OROMappingDB->GetAllOROMapping_Pagination");
                List<DTOOROMappingResponse> dTOOROs = new List<DTOOROMappingResponse>();
                var responseData = new DTODataTablesResponse<DTOOROMappingResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOOROs
                };
                return responseData;
            }
        }

        /// <summary>
        /// Retrieves a list of all OROMapping records along with their associated Record Office names.
        /// </summary>
        /// <returns>A list of DTOAllOROResponse objects containing OROMappingId and RecordOffice details.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the database query.</exception>
        public async Task<List<DTOAllOROResponse>> GetAllORO()
        {
            try
            {
                string query = "";
                query = @"SELECT oro.OROMappingId,rec.RecordOfficeId,rec.Name FROM OROMapping oro
                        INNER JOIN MRecordOffice rec on oro.RecordOfficeId=rec.RecordOfficeId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOAllOROResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "OROMappingDB->GetAllORO");
                return new List<DTOAllOROResponse>();
            }

        }
    }
}
