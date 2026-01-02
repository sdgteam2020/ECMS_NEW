using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;

namespace DataAccessLayer
{
    public class DistributeCardDB : GenericRepositoryDL<TrnDistributeCard>, IDistributeCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<DistributeCardDB> _logger;

        public DistributeCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<DistributeCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        /// <summary>
        /// Checks whether a given RequestId exists in the TrnDistributeCards table.
        /// This method is used to determine if any record with the specified RequestId exists in the database.
        /// </summary>
        /// <param name="RequestId">The RequestId to search for in the TrnDistributeCards table.</param>
        /// <returns>
        /// A boolean value indicating whether the RequestId exists in the TrnDistributeCards table.
        /// Returns <c>true</c> if the RequestId is found, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The method uses an asynchronous query to check if any records in the TrnDistributeCards table
        /// match the specified RequestId. It returns a boolean indicating the presence of the RequestId.
        /// </remarks>
        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            try
            {
                // Use the AnyAsync method to check if there is any record with the given RequestId
                return await _context.TrnDistributeCards
                                .AnyAsync(f => f.RequestId == RequestId);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the database operation
                _logger.LogError(1001, ex, "DistributeCardDB->FindAnyRequestId");
                return false;
            }
        }

        /// <summary>
        /// Retrieves a paginated list of destruction card records with filtering and sorting.
        /// The records are fetched from the `TrnDistributeCards` table and related tables such as `TrnICardRequest`, `BasicDetails`, `MRank`, `MUnit`, and others.
        /// This method also supports search and sorting functionalities for the retrieved records.
        /// </summary>
        /// <param name="dTO">The request object containing filtering, sorting, and pagination parameters.</param>
        /// <returns>
        /// A response object containing the total number of records, the total number of filtered records, and a list of destruction card records.
        /// </returns>
        /// <remarks>
        /// The method constructs a dynamic query to fetch destruction card records based on the provided parameters (e.g., search, sort, pagination).
        /// It uses SQL Common Table Expressions (CTEs) to calculate the total filtered records and paginate the results efficiently.
        /// </remarks>
        public async Task<DTODataTablesResponse<DTODistributeCardGetResponse>> GetAllDistribute(DTODataTablesRequest dTO)
        {
            List<DTODistributeCardGetResponse> dTODistributeCardGetResponses = new List<DTODistributeCardGetResponse>();
            var responseData = new DTODataTablesResponse<DTODistributeCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTODistributeCardGetResponses
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

                // Default sort column and order
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "tdc.UpdatedOn";

                var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

                string selectFields = @"appl.Name ApplyFor,
                                        req.RequestId,tdc.DistributeCardId,
                                        bas.ServiceNo,ranks.RankAbbreviation RankName,
                                        bas.FName,bas.LName,
                                        Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                                        tdc.UpdatedOn,tdc.Remark,tdc.IsActive,
                                        bas.NameAsPerRecord,
                                        regi.Abbreviation RegimentalName,
                                        tdc.DistributedOn";
                string fromJoinClause = @"from TrnDistributeCards tdc
                                        inner join TrnICardRequest req on req.RequestId = tdc.RequestId
                                        inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                        inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                                        inner join MRank ranks on ranks.RankId=bas.RankId
                                        inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                        inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                        inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                                        left join MRegimental regi on regi.RegId=bas.RegimentalId";
                string whereClause = @"Where bas.ServiceNo like '%' + @SearchTerm + '%' ";

                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    // Parameters for SQL query
                    dTO.searchValue = string.IsNullOrEmpty(dTO.searchValue) ? string.Empty : dTO.searchValue.Trim();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    // Execute the SQL query to get the records and total count
                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTODistributeCardGetResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    // Prepare the response data
                    responseData = new DTODataTablesResponse<DTODistributeCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = (from e in records
                                select new DTODistributeCardGetResponse()
                                {
                                    EncryptedId = protector.Protect(e.DistributeCardId.ToString()),
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
                                    DistributeCardId = e.DistributeCardId,
                                    Remark = e.Remark,
                                    IsActive = e.IsActive,
                                    DistributedOn = e.DistributedOn
                                }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DistributeCardDB->GetAllDistribute");
            }
            return responseData;
        }


        /// <summary>
        /// Retrieves the details of destruction cards based on the provided request IDs for exporting.
        /// This method fetches relevant information from various related tables such as `TrnDistributeCards`, `TrnICardRequest`, `BasicDetails`, `MRank`, `MUnit`, and others.
        /// </summary>
        /// <param name="Data">The DTO request object that contains the list of request IDs for which destruction card details are to be fetched.</param>
        /// <returns>A list of `DTODistributeCardExportResponse` containing the destruction card details that match the provided request IDs.</returns>
        /// <remarks>
        /// This method performs a SQL query to fetch destruction card data for the specified request IDs, including the service number, rank, unit, destruction time, and other relevant fields.
        /// The SQL query joins multiple tables and uses the request IDs provided in `Data.Ids` to filter the results.
        /// </remarks>
        public async Task<List<DTODistributeCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            var records = new List<DTODistributeCardExportResponse>();
            try
            {
                // SQL query to fetch destruction card details based on Request IDs
                string query = @"select req.RequestId,tdc.DistributeCardId,bas.ServiceNo as ArmyNo,
	                                ranks.RankAbbreviation,bas.FName,bas.LName,Muni.Abbreviation Unit,
	                                tdc.UpdatedOn as DateAndTime,tdc.Remark,tdc.IsActive as IsActiveBool,
	                                req.CardSerialNo,req.ChipNo,tdc.DistributedOn
	                                from TrnDistributeCards tdc
	                                inner join TrnICardRequest req on req.RequestId = tdc.RequestId
	                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId=bas.RankId
	                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
	                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                  Where req.RequestId in @Ids";

                // Define parameters to be passed to the SQL query
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Data.Ids);

                // Execute the query to fetch records
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTODistributeCardExportResponse>(query, parameters);
                    records = ret.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                _logger.LogError(1001, ex, "DistributeCardDB->GetDetailsByRequestIds");
            }

            // Return the list of destruction card details
            return records;
        }

        /// <summary>
        /// Saves the distribute card data into the database, updating the relevant records in related tables.
        /// This method handles inserting data into the `TrnDistributeCards` table, updating the `TrnICardRequest`, `TrnStepCounter`, and other related tables based on the request.
        /// It also manages the transactional flow to ensure consistency of operations.
        /// </summary>
        /// <param name="model">The `TrnDistributeCard` model containing the distribution details to be saved.</param>
        /// <param name="cardRequestHistory">The `ICardHistoryResponseAll` object containing the history of the card request, which is serialized for storage.</param>
        /// <returns>A `DTOCommonSaveResponse` object containing the result of the save operation (success/failure), the ID of the created record, and the current timestamp.</returns>
        /// <remarks>
        /// This method performs multiple database operations within a single transaction:
        /// 1. It inserts a new record into the `TrnDistributeCards` table.
        /// 2. It updates the `TrnICardRequest` and `TrnStepCounter` tables to reflect the changes.
        /// 3. It optionally updates the `TrnFaultyCard` and `TrnPostingOut` tables based on conditions from the `cardRequestHistory`.
        /// 4. It records the history of the card request in the `CompletedICardRequests` table.
        /// </remarks>
        public async Task<DTOCommonSaveResponse> SaveDistributeCard(TrnDistributeCard model, ICardHistoryResponseAll cardRequestHistory)
        {
            // Initialize the response DTO
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            DTOCommonSaveResponse dtoResponse = new DTOCommonSaveResponse();
            try
            {
                // Serialize the card request history to store in the database
                var cardRequestHistoryJson = JsonConvert.SerializeObject(cardRequestHistory);

                // SQL query to insert the distribution card and update relevant records
                var insertQuery = @$"Insert into TrnDistributeCards(RequestId,DistributedOn,Remark,UpdatedbyUserId,IsActive,Updatedby,UpdatedOn) 
                                                             Values(@RequestId,@DistributedOn,@Remark,@UpdatedbyUserId,@IsActive,@Updatedby,@UpdatedOn);

                                     DECLARE @DistributeCardId INT = SCOPE_IDENTITY();
                                     
                                     update TrnICardRequest set StatusId = 2,UpdatedOn = @UpdatedOn,Updatedby = @Updatedby where RequestId = @RequestId;
                                     
                                     update TrnStepCounter set StepId = 15,UpdatedOn = @UpdatedOn,Updatedby = @Updatedby where RequestId = @RequestId;
                                     {(cardRequestHistory?.FaultyCard?.Count > 0 ? "update TrnFaultyCard set TrnFwdId = null where RequestId = @RequestId;" : "")}
                                     {(cardRequestHistory?.PostingOut?.Count > 0 ? "update TrnPostingOut set TrnFwdId = null where RequestId = @RequestId;" : "")}
                                     
                                     Delete from TrnFwds where RequestId = @RequestId;
                                     Insert into CompletedICardRequests(RequestId,CardRequestHistoryJson,UpdatedbyUserId,IsActive,Updatedby,UpdatedOn)
                                     values(@RequestId,@CardRequestHistoryJson,@UpdatedbyUserId,@IsActive,@Updatedby,@UpdatedOn);
                                     
                                     Select @DistributeCardId;
                                    ";

                // Set up the parameters for the SQL query
                var parameters = new DynamicParameters();
                parameters.Add("@RequestId", model.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@DistributedOn", model.DistributedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Remark", model.Remark, DbType.String, ParameterDirection.Input);
                parameters.Add("@UpdatedbyUserId", model.UpdatedbyUserId, DbType.String, ParameterDirection.Input);
                parameters.Add("@IsActive", model.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", model.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", model.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@CardRequestHistoryJson", cardRequestHistoryJson, DbType.String, ParameterDirection.Input);

                // Execute the query and get the DistributeCardId of the newly created record
                model.DistributeCardId = await db.ExecuteScalarAsync<int>(insertQuery, parameters, transaction: transaction);

                // Commit the transaction if everything was successful
                transaction.Commit();

                // Set the success response
                dtoResponse.Result = true;
                dtoResponse.Message = "Record Created!";
                dtoResponse.Id = model.DistributeCardId.ToString();
                dtoResponse.CurrentTime = model.UpdatedOn.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                // Rollback the transaction if an exception occurs
                transaction.Rollback();
                _logger.LogError(1001, ex, "DistributeCardDB->SaveDistributeCard");
                dtoResponse.Result = false;
                dtoResponse.Message = "Internal Server Error!";
            }

            return dtoResponse;
        }
    }
}
