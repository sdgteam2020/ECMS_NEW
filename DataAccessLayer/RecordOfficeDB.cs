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
    public class RecordOfficeDB : GenericRepositoryDL<MRecordOffice>, IRecordOfficeDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<RecordOfficeDB> _logger;
        public RecordOfficeDB(ApplicationDbContext context, DapperContext contextDP, ILogger<RecordOfficeDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }


        /// <summary>
        /// Checks if the provided record office name or abbreviation already exists, excluding the current record office.
        /// </summary>
        /// <param name="Dto">The MRecordOffice object containing the name and abbreviation to check against.</param>
        /// <returns>1 if valid, 2 if a duplicate exists with the same name or abbreviation but a different RecordOfficeId.</returns>
        public async Task<int> GetByName(MRecordOffice Dto)
        {
            List<MRecordOffice> mRecordOffices = await _context.MRecordOffice.AsNoTracking().ToListAsync();
            if (mRecordOffices.Any(x => (x.Name.ToUpper() == Dto.Name.ToUpper() || x.Abbreviation.ToUpper() == Dto.Abbreviation.ToUpper()) && x.RecordOfficeId != Dto.RecordOfficeId))
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }


        /// <summary>
        /// Checks if a RecordOffice with the given TDMId and UnitId already exists.
        /// </summary>
        /// <param name="UnitId">The unit ID to check.</param>
        /// <param name="TDMId">The TDMId to check against.</param>
        /// <returns>True if a record exists with the given TDMId and UnitId, otherwise false.</returns>
        public async Task<bool> GetByTDMId(int UnitId,int? TDMId)
        {
            List<MRecordOffice> mRecordOffices = await _context.MRecordOffice.AsNoTracking().ToListAsync();
            if(TDMId!=null)
            {
                var result = mRecordOffices.Any(x => x.TDMId == TDMId && x.UnitId== UnitId);
                return result;
            }
            else
            {
                var result = mRecordOffices.Any(x => x.UnitId == UnitId);
                return result;
            }

        }

        /// <summary>
        /// Retrieves the RecordOffice data based on the provided TDMId.
        /// </summary>
        /// <param name="TDMId">The TDMId to search for in the database.</param>
        /// <returns>A DTO containing RecordOffice details or null if not found.</returns>
        public async Task<DTOGetROByTDMIdResponse?> GetROByTDMId(int TDMId)
        {
            try
            {
                string query = "";
                query = "Select tdm.IsRO, tdm.IsORO, mrecord.TDMId,mrecord.RecordOfficeId,mrecord.UnitId from TrnDomainMapping tdm" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId = tdm.UnitId" +
                        " inner join MRecordOffice mrecord on mrecord.UnitId = mapunit.UnitMapId" +
                        " where tdm.Id=@TDMId ";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetROByTDMIdResponse>(query, new { TDMId });
                    return allrecord.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RecordOfficeDB->GetByUserId");
                return null;
            }

        }

        /// <summary>
        /// Retrieves all the RecordOffice data with related information like ArmedName, Rank, and Unit details.
        /// </summary>
        /// <returns>A list of DTO containing all RecordOffice data, or null if an error occurs.</returns>
        public async Task<List<DTORecordOfficeResponse>?> GetAllData()
        {
            try
            {
                string query = "";
                query = @"Select mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,mrecord.Message,mrecord.Abbreviation,mrecord.TDMId,mrecord.UnitId,marmed.ArmedId,marmed.ArmedName,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName
                        from MRecordOffice mrecord
                        inner join MArmedType marmed on marmed.ArmedId=mrecord.ArmedId
                        left join TrnDomainMapping trndomain on trndomain.Id=mrecord.TDMId
                        left join AspNetUsers users on users.Id=trndomain.AspNetUsersId
                        left join UserProfile usep on usep.UserId=trndomain.UserId
                        left join MRank ra on ra.RankId=usep.RankId 
                        left join MapUnit mapunit on mapunit.UnitMapId = mrecord.UnitId 
                        left join MUnit munit on munit.UnitId =mapunit.UnitId order by mrecord.RecordOfficeId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTORecordOfficeResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RecordOfficeDB->GetAllData");
                return null;
            }

        }
        public async Task<DTODataTablesResponse<DTORecordOfficeResponse>> GetAllRecordOffice_Pagination(DTODataTablesRequest dTO)
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
                ["Abbreviation"] = "mrecord.Abbreviation",
                ["RecordOfficeId"] = "mrecord.RecordOfficeId",
                ["ArmedName"] = "marmed.ArmedName",
            };
            selectFields = @"mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,mrecord.Message,mrecord.Abbreviation,mrecord.TDMId,mrecord.UnitId,marmed.ArmedId,marmed.ArmedName,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName";
            fromJoinClause = @"from MRecordOffice mrecord
                                inner join MArmedType marmed on marmed.ArmedId=mrecord.ArmedId
                                left join TrnDomainMapping trndomain on trndomain.Id=mrecord.TDMId
                                left join AspNetUsers users on users.Id=trndomain.AspNetUsersId
                                left join UserProfile usep on usep.UserId=trndomain.UserId
                                left join MRank ra on ra.RankId=usep.RankId 
                                left join MapUnit mapunit on mapunit.UnitMapId = mrecord.UnitId 
                                left join MUnit munit on munit.UnitId =mapunit.UnitId";
            whereClause = @"WHERE
                                (
                                    mrecord.Name LIKE '%' + @SearchTerm + '%' OR
                                    marmed.ArmedName LIKE '%' + @SearchTerm + '%' OR
                                    mrecord.Abbreviation LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "mrecord.RecordOfficeId";
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
                    var records = (await ret.ReadAsync<DTORecordOfficeResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTORecordOfficeResponse>
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
                _logger.LogError(1001, ex, "RecordOfficeDB->GetAllRecordOffice_Pagination");
                List<DTORecordOfficeResponse> dTORecordOffices = new List<DTORecordOfficeResponse>();
                var responseData = new DTODataTablesResponse<DTORecordOfficeResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTORecordOffices
                };
                return responseData;
            }
        }

        /// <summary>
        /// Retrieves the update data for a RecordOffice based on the RecordOfficeId.
        /// </summary>
        /// <param name="RecordOfficeId">The ID of the RecordOffice to update.</param>
        /// <returns>A DTO containing the RecordOffice update details, or null if an error occurs.</returns>
        public async Task<DTOGetUpdateRecordOfficeResponse?> GetUpdateRecordOffice(int RecordOfficeId)
        {
            try
            {
                string query = "";
                query = "Select mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,mrecord.Abbreviation,mrecord.Message,marmed.ArmedName from TrnDomainMapping trndomain" +
                        " inner join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " inner join MRecordOffice mrecord on mrecord.TDMId=trndomain.Id" +
                        " inner join MArmedType marmed on marmed.ArmedId=mrecord.ArmedId" +
                        " where mrecord.RecordOfficeId=@RecordOfficeId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetUpdateRecordOfficeResponse>(query, new { RecordOfficeId });
                    return allrecord.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RecordOfficeDB->GetMappedForRecord");
                return null;
            }
        }

        /// <summary>
        /// Retrieves all mapped records for the provided UnitMapId.
        /// </summary>
        /// <param name="UnitMapId">The ID of the UnitMap to search for.</param>
        /// <returns>A list of DTOs containing mapped user information.</returns>
        public async Task<List<DTOGetMappedForRecordResponse>?> GetDDMappedForRecord(int UnitMapId)
        {
            try
            {
                string query = "";
                query = "Select users.Id as AspNetUsersId,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name,trndomain.Id as TDMId from AspNetUsers users" +
                        " inner join TrnDomainMapping trndomain on users.Id=trndomain.AspNetUsersId" +
                        " inner join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " inner join MRank ra on ra.RankId=usep.RankId " +
                        " where trndomain.UnitId=@UnitMapId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetMappedForRecordResponse>(query, new { UnitMapId });
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RecordOfficeDB->GetDDMappedForRecord");
                return null;
            }

        }

        /// <summary>
        /// Updates the values of a RecordOffice record.
        /// </summary>
        /// <param name="dTO">The DTO containing the updated RecordOffice data.</param>
        /// <returns>True if the update is successful, false if the RecordOfficeId is not found, or null if an error occurs.</returns>
        public async Task<bool?> UpdateROValue(DTOUpdateROValueRequest dTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var roUpdate = await _context.MRecordOffice.FindAsync(dTO.RecordOfficeId); 
                    if(roUpdate == null)
                    {
                        return false;
                    }
                    else
                    {
                        roUpdate.TDMId = dTO.TDMId;
                        roUpdate.Message = dTO.Message;
                        roUpdate.Updatedby = dTO.Updatedby;
                        roUpdate.UpdatedOn = dTO.UpdatedOn;
                        _context.MRecordOffice.Update(roUpdate);
                        await _context.SaveChangesAsync();
                        
                        transaction.Commit();
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(1001, ex, "RecordOfficeDB->UpdateROValue");
                    return null;
                }
            }
        }
    }
}
