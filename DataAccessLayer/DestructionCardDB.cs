using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    public class DestructionCardDB : GenericRepositoryDL<TrnDestructionCard>, IDestructionCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<DestructionCardDB> _logger;

        public DestructionCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<DestructionCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        /// <summary>
        /// Checks if any record with the specified RequestId exists in the DestructionCards table.
        /// </summary>
        /// <param name="RequestId">The RequestId to search for in the DestructionCards table.</param>
        /// <returns>
        /// Returns <c>true</c> if a record with the specified RequestId exists, otherwise returns <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method queries the `TrnDestructionCards` table to determine if any record has the provided `RequestId`.
        /// If a matching record is found, it returns <c>true</c>. If no record is found, it returns <c>false</c>.
        /// This can be used to verify the existence of a RequestId before performing further operations.
        /// </remarks>
        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            try
            {
                // Querying the DestructionCards table to check if the provided RequestId exists
                return await _context.TrnDestructionCards
                                    .AnyAsync(f => f.RequestId == RequestId);
            }
            catch (Exception ex)
            {
                // Logging the exception if any error occurs while querying the database
                _logger.LogError(1001, ex, "DestructionCardDB->FindAnyRequestId");
                return false;
            }
        }


        /// <summary>
        /// Retrieves a paginated list of destruction card records based on the provided search term and sorting criteria.
        /// </summary>
        /// <param name="dTO">The DataTables request object containing pagination, search, and sorting information.</param>
        /// <returns>
        /// A DTODataTablesResponse containing a list of DTODestructionCardGetResponse objects, representing the destruction card records.
        /// The response includes pagination information such as total records and filtered records.
        /// </returns>
        /// <remarks>
        /// This method builds a dynamic SQL query to fetch destruction card records from the database, including related data such as 
        /// service numbers, names, ranks, unit information, remarks, and destruction dates. It applies filtering, sorting, and pagination 
        /// based on the parameters passed in the DTODataTablesRequest object.
        /// </remarks>
        public async Task<DTODataTablesResponse<DTODestructionCardGetResponse>> GetAllDestruction(DTODataTablesRequest dTO)
        {
            // Initialize the response object with default values
            List<DTODestructionCardGetResponse> dTODestructionCardGetResponses = new List<DTODestructionCardGetResponse>();
            var responseData = new DTODataTablesResponse<DTODestructionCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTODestructionCardGetResponses
            };
            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "bas.ServiceNo",
                    ["UpdatedOn"] = "tdc.UpdatedOn",
                    ["RequestId"] = "req.RequestId",
                    ["Remark"] = "tdc.Remark"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "tdc.UpdatedOn";

                var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

                // Base query for fetching data
                string selectFields = "";
                string fromJoinClause = "";
                string whereClause = "";
                selectFields = @"appl.Name ApplyFor,
                                req.RequestId,tdc.DestructedCardId,
                                bas.ServiceNo,ranks.RankAbbreviation RankName,
                                bas.FName,bas.LName,
                                Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                                tdc.UpdatedOn,tdc.Remark,tdc.IsActive,
                                bas.NameAsPerRecord,
                                regi.Abbreviation RegimentalName,
                                tdc.DestructedOn,
                                (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(tdc.RemarksIds,','))) RemarksNameList,
                                tdc.RemarksIds";
                fromJoinClause = @"from TrnDestructionCards tdc
                                inner join TrnICardRequest req on req.RequestId = tdc.RequestId
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                                inner join MRank ranks on ranks.RankId=bas.RankId
                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                                left join MRegimental regi on regi.RegId=bas.RegimentalId";
                whereClause = @"Where bas.ServiceNo like '%' + @SearchTerm + '%' ";

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
                    var records = (await ret.ReadAsync<DTODestructionCardGetResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    responseData = new DTODataTablesResponse<DTODestructionCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = (from e in records
                                select new DTODestructionCardGetResponse()
                                {
                                    EncryptedId = protector.Protect(e.DestructedCardId.ToString()),
                                    NameAsPerRecord = e.NameAsPerRecord,
                                    FName = e.FName,
                                    LName = e.LName,
                                    ServiceNo = e.ServiceNo,
                                    UnitName = e.UnitName,
                                    UnitAbbreviation = e.UnitAbbreviation,
                                    RankName = e.RankName,
                                    ArmedName = e.ArmedName,
                                    RequestId = e.RequestId,
                                    UpdatedOn = e.UpdatedOn,
                                    ApplyFor = e.ApplyFor,
                                    DestructedCardId = e.DestructedCardId,
                                    Remark = e.Remark,
                                    RemarksIds = e.RemarksIds,
                                    RemarksNameList = e.RemarksNameList,
                                    IsActive = e.IsActive,
                                    DestructedOn = e.DestructedOn
                                }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DestructionCardDB->GetAllDestruction");
            }
            return responseData;
        }

        /// <summary>
        /// Retrieves the destruction card details for the given list of request IDs.
        /// This method is used for exporting data related to destruction cards, including associated details such as Army number, rank, unit, serial number, chip number, and reasons for destruction.
        /// </summary>
        /// <param name="Data">An object containing a list of request IDs to filter the destruction card records.</param>
        /// <returns>A list of DTODestructionCardExportResponse objects containing the destruction card details.</returns>
        /// <remarks>
        /// The query fetches data related to destruction cards from multiple tables, including BasicDetails, MRank, MUnit, and MRemarks. 
        /// It aggregates the reasons for destruction from the MRemarks table, and it only includes records where the request ID matches those provided in the input.
        /// The results are then returned as a list of DTODestructionCardExportResponse objects.
        /// </remarks>
        public async Task<List<DTODestructionCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            var records = new List<DTODestructionCardExportResponse>();
            try
            {
                // SQL query to retrieve destruction card details along with related information
                string query = @"select req.RequestId,tdc.DestructedCardId,bas.ServiceNo as ArmyNo,
	                                ranks.RankAbbreviation,bas.FName,bas.LName,Muni.Abbreviation Unit,
	                                tdc.UpdatedOn as DateAndTime,tdc.Remark,tdc.IsActive as IsActiveBool,
	                                req.CardSerialNo,req.ChipNo,tdc.DestructedOn,
                                    (select STRING_AGG(Remarks,' | ') from MRemarks where RemarksId in (select value from string_split(tdc.RemarksIds,','))) Reasons
	                                from TrnDestructionCards tdc
	                                inner join TrnICardRequest req on req.RequestId = tdc.RequestId
	                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId=bas.RankId
	                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
	                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                  Where req.RequestId in @Ids";

                // Create parameters for the query
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Data.Ids);

                // Execute the query and map the results to DTODestructionCardExportResponse
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTODestructionCardExportResponse>(query, parameters);
                    records = ret.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during query execution
                _logger.LogError(1001, ex, "DestructionCardDB->GetDetailsByRequestIds");
            }
            return records;
        }
    }
}
