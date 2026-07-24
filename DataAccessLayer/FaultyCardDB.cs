using Azure.Core;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class FaultyCardDB : GenericRepositoryDL<TrnFaultyCard>, IFaultyCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<FaultyCardDB> _logger;
        public FaultyCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<FaultyCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        /// <summary>
        /// Checks whether a request ID exists in the `TrnFaultyCard` table with the specified `RequestId` and the `IsComplete` flag set to `false`.
        /// </summary>
        /// <param name="RequestId">The request ID to check for existence in the `TrnFaultyCard` table.</param>
        /// <returns>
        /// Returns `true` if the specified `RequestId` exists in the `TrnFaultyCard` table with `IsComplete` set to `false`. 
        /// Returns `false` if no matching records are found or if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This method uses the `AnyAsync` method to perform an asynchronous query to check if any record in the `TrnFaultyCard` table matches the `RequestId` 
        /// and has the `IsComplete` flag set to `false`. If an exception occurs during the query, it is logged and `false` is returned.
        /// </remarks>
        public async Task<bool> FindRequestId(int RequestId)
        {
            try
            {
                // Query to check if there is any record in TrnFaultyCard with the given RequestId and IsComplete = false
                return await _context.TrnFaultyCard.AnyAsync(f => f.RequestId == RequestId && f.IsComplete == false);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the query
                _logger.LogError(1001, ex, "FaultyCardDB->FindRequestId");
                return false;
            }
        }


        /// <summary>
        /// Retrieves a concatenated list of remarks from the `MRemarks` table based on the provided `RemarksIds`.
        /// </summary>
        /// <param name="RemarksIds">An array of `RemarksIds` used to filter and retrieve the remarks data.</param>
        /// <returns>
        /// A string containing all the remarks concatenated with a `#` separator. 
        /// If no remarks are found for the provided `RemarksIds`, the method returns `null`.
        /// </returns>
        /// <remarks>
        /// The method executes a SQL query that uses the `STRING_AGG` function to concatenate all remarks for the given `RemarksIds` with a `#` symbol as the separator.
        /// </remarks>
        public async Task<string> GetRemarksData(int[] RemarksIds)
        {
            // SQL query to retrieve and concatenate remarks for the provided RemarksIds
            string query = @"select STRING_AGG(Remarks,'#') AS RemarksNameList from MRemarks where RemarksId in @RemarksIds";
            using (var connection = _contextDP.CreateConnection())
            {
                // Executes the SQL query and returns the concatenated remarks
                var result = await connection.QueryFirstOrDefaultAsync<string>(query, new { RemarksIds });
                return result; // Returns the concatenated string or null if no remarks found
            }
        }


        /// <summary>
        /// Retrieves a paginated and filtered list of faulty card data. The data includes service number, rank, unit, and fault details.
        /// It also handles search, sorting, and filtering based on the provided parameters.
        /// </summary>
        /// <param name="request">An object containing the request parameters, including sorting, filtering, pagination, and claim status.</param>
        /// <returns>
        /// A DTODataTablesResponse containing a list of DTOFaultyCardListResponse objects, which represent the details of each faulty card.
        /// If an error occurs, it returns an empty response with zero records.
        /// </returns>
        /// <remarks>
        /// This method checks whether to apply unit-based filtering when `Claim` is false. It also handles the sorting of data
        /// based on the specified column and direction. The method uses LINQ to filter and sort the data, and it paginates
        /// the results before returning them.
        /// </remarks>
        public async Task<DTODataTablesResponse<DTOFaultyCardListResponse>> GetAllFaulty(DTODataTablesRequestForFaultyCard request)
        {
            try
            {
                // Creating the base query to get faulty card data
                var queryableData = (from faulty in _context.TrnFaultyCard.OrderByDescending(x => x.TrnFaultyCardId)
                                     join mcat in _context.MCategory on faulty.CategoryId equals mcat.CategoryId
                                     join req in _context.TrnICardRequest on faulty.RequestId equals req.RequestId
                                     join tdm in _context.TrnDomainMapping on req.TrnDomainMappingId equals tdm.Id
                                     join bas1Temp in _context.BasicDetails on req.BasicDetailId equals bas1Temp.BasicDetailId into bas1Group 
                                     from bas1 in bas1Group.DefaultIfEmpty()
                                     join bas2Temp in _context.BasicDetailsAFSAC2 on req.BasicDetailId equals bas2Temp.BasicDetailId into bas2Group
                                     from bas2 in bas2Group.DefaultIfEmpty()
                                     join ranks in _context.MRank on (bas1 != null ? bas1.RankId : bas2.RankId) equals ranks.RankId
                                     join mapunit in _context.MapUnit on (bas1 != null ? bas1.UnitId : bas2.UnitId) equals mapunit.UnitMapId
                                     join munit in _context.MUnit on mapunit.UnitId equals munit.UnitId
                                     join appl in _context.MApplyFor on (bas1 != null ? bas1.ApplyForId : bas2.ApplyForId) equals appl.ApplyForId
                                     select new DTOFaultyCardListResponse()
                                     {
                                         EncryptedId = protector.Protect(faulty.TrnFaultyCardId.ToString()),
                                         FName_1 = bas1 != null ? bas1.FName : null,
                                         LName_1 = bas1 != null ? bas1.LName : null,

                                         FName_2 = bas2 != null ? bas2.FName : null,
                                         LName_2 = bas2 != null ? bas2.LName : null,
                                         ServiceNo = bas1 != null ? bas1.ServiceNo : bas2.ServiceNo,
                                         UnitMapId = mapunit.UnitMapId,
                                         UnitAbbreviation = munit.Abbreviation,
                                         RankName = ranks.RankAbbreviation,
                                         RequestId = req.RequestId,
                                         UpdatedOn = faulty.UpdatedOn ?? DateTime.Now,
                                         ApplyFor = appl.Name,
                                         TrnFaultyCardId = faulty.TrnFaultyCardId,
                                         RemarksIds = faulty.RemarksIds,
                                         FromRemark = faulty.FromRemark,
                                         ToRemark = faulty.ToRemark,
                                         CategoryId = faulty.CategoryId,
                                         FaultyStage = mcat.Name,
                                         IsEditAction = faulty.IsEditAction,
                                     }).AsQueryable();
                if (request.Claim == false)
                {
                    queryableData = queryableData.Where(x => x.UnitMapId == request.UnitMapId);
                }

                // Total records without filtering
                var totalRecords =await queryableData.CountAsync();

                // Apply filtering
                if (!string.IsNullOrWhiteSpace(request.searchValue))
                {
                    var searchValue = request.searchValue.Trim();

                    queryableData = queryableData.Where(x => x.ServiceNo != null && x.ServiceNo.Contains(searchValue));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    //queryableData = queryableData.OrderBy(request.SortColumn + " " + request.SortColumnDirection);
                    queryableData = request.sortDirection.ToLower() == "asc"
                    ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                    : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                }
                else
                {
                    queryableData = queryableData.OrderByDescending(x => x.TrnFaultyCardId);
                }

                // Total records after filtering
                var filteredRecords = await queryableData.CountAsync();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                foreach (var item in paginatedData)
                {
                    item.FName = item.FName_1 ?? item.FName_2 ?? string.Empty;
                    item.LName = item.LName_1 ?? item.LName_2;
                }

                var responseData = new DTODataTablesResponse<DTOFaultyCardListResponse>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords, // Total records without filtering
                    recordsFiltered = filteredRecords, // Total records after filtering
                    data = paginatedData
                };

                return responseData;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "FaultyCardDB->GetAllFaulty");
                List<DTOFaultyCardListResponse> dTOUserRegnResponses = new List<DTOFaultyCardListResponse>();
                var responseData = new DTODataTablesResponse<DTOFaultyCardListResponse>
                {
                    draw = request.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }


        /// <summary>
        /// Retrieves the details of a specific faulty card using the provided TrnFaultyCardId.
        /// The method queries various related tables such as MCategory, TrnICardRequest, and BasicDetails to fetch relevant information about the faulty card, including the service number, rank, unit, remarks, and fault details.
        /// </summary>
        /// <param name="TrnFaultyCardId">The ID of the faulty card for which details are being requested.</param>
        /// <returns>
        /// A <see cref="DTOFaultyCardListResponse"/> object containing the details of the faulty card. If no matching card is found, it returns null.
        /// </returns>
        /// <remarks>
        /// This method utilizes a SQL query to join multiple tables (MCategory, TrnICardRequest, BasicDetails, etc.) to gather the full details of a faulty card.
        /// The results include the service number (with formatting), rank, unit, remarks, fault category, and other related details.
        /// If an error occurs during the execution of the query, the method logs the error and returns null.
        /// </remarks>
        public async Task<DTOGenericResponse<DTOFaultyCardListResponse?>> GetTrnFaultyCardDetail(int TrnFaultyCardId)
        {
            var response = new DTOGenericResponse<DTOFaultyCardListResponse?>();
            try
            {
                // SQL query to fetch faulty card details from multiple related tables
                string query = @"SELECT mcat.CategoryId,req.RequestId,bd.ServiceNo,faulty.RemarksIds,faulty.FromRemark
                                from TrnFaultyCard faulty
                                inner join MCategory mcat on mcat.CategoryId = faulty.CategoryId
                                inner join TrnICardRequest req on req.RequestId = faulty.RequestId AND req.StatusId =1
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                inner join BasicDetails bd on bd.BasicDetailId=req.BasicDetailId
                                WHERE faulty.TrnFaultyCardId = @TrnFaultyCardId";


                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query and return the first matching record
                    var allrecord = await connection.QueryFirstOrDefaultAsync<DTOFaultyCardListResponse>(query, new { TrnFaultyCardId });

                    if (allrecord == null)
                    {
                        response.Result = false;
                        response.Message = "Invalid Id.";
                    }
                    else
                    {
                        response.Result = true;
                        response.Value = allrecord;
                        response.Message = "ok";
                    }

                }
            }
            catch (Exception ex)
            {
                // Log the error and return null if an exception occurs
                _logger.LogError(1001, ex, "FaultyCardDB->GetTrnFaultyCardDetail");
                response.Result = false;
                response.Message = "An error occurred while fetching the record.";
            }
            return response;
        }


        /// <summary>
        /// Saves or updates a faulty card record based on the provided data. If the `TrnFaultyCardId` is greater than 0, 
        /// it updates the existing record; otherwise, it inserts a new record. 
        /// Additionally, it handles "Accept" (Choice == 2) or "Reject" (Choice == 3) actions by performing related updates
        /// on various tables like `TrnStepCounter`, `TrnFwds`, and `TrnICardRequest`.
        /// </summary>
        /// <param name="dTO">The DTO containing the data for the faulty card (e.g., remarks, status, request details).</param>
        /// <param name="mTrnFwd">Optional parameter containing forwarding information (required when the card is rejected).</param>
        /// <returns>A `DTOCommonSaveResponse` containing the status of the save operation, including success or failure message and the current time.</returns>
        public async Task<DTOCommonSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO, MTrnFwd? mTrnFwd)
        {
            DTOCommonSaveResponse saveResponse = new DTOCommonSaveResponse();
            string XmlFiles = string.Empty;

            string RemarksIds = dTO.RemarksIds != null && dTO.RemarksIds.Any() ? string.Join(",", dTO.RemarksIds) : string.Empty;
            // Open a database connection and start a transaction
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();

            // SQL query strings for performing insert, update, and related queries
            string insert = "";
            string update = "";
            string query2 = "";
            string query3 = "";
            string query4 = "";
            string query5 = "";
            string query6 = "";

            try
            {
                // If the TrnFaultyCardId is provided (existing record), update the record
                if (dTO.TrnFaultyCardId > 0)
                {
                    update = @"UPDATE TrnFaultyCard set ToRemark = @ToRemark,IsEditAction = @IsEditAction,IsComplete = @IsComplete WHERE TrnFaultyCardId=@TrnFaultyCardId";

                    // Set parameters for the update query
                    var parameters = new DynamicParameters();
                    parameters.Add("@TrnFaultyCardId", dTO.TrnFaultyCardId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ToRemark", dTO.ToRemark, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@IsEditAction", dTO.IsEditAction, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@IsComplete", dTO.IsComplete, DbType.Boolean, ParameterDirection.Input);

                    // Execute the update query
                    await db.ExecuteAsync(update, parameters, transaction: transaction);

                    // Set response message for successful update
                    saveResponse.Id = dTO.TrnFaultyCardId.ToString();
                    saveResponse.Message = "Data Updated";
                }
                else
                {
                    // SQL query to insert a new faulty card record
                    insert = @"INSERT INTO TrnFaultyCard(RemarksIds,FromRemark,ToRemark,CategoryId,RequestId,IsActive,UserId,Updatedby,UpdatedOn,IsEditAction,TrnFwdId,IsComplete)
                                OUTPUT INSERTED.TrnFaultyCardId
                                VALUES(@RemarksIds,@FromRemark,@ToRemark,@CategoryId,@RequestId,@IsActive,@UserId,@Updatedby,@UpdatedOn,@IsEditAction,@TrnFwdId,@IsComplete)";

                    // Set parameters for the insert query
                    var parameters = new DynamicParameters();
                    parameters.Add("@TrnFaultyCardId", dTO.TrnFaultyCardId, DbType.Int32, ParameterDirection.Output);
                    parameters.Add("@RemarksIds", RemarksIds, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@FromRemark", dTO.FromRemark, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@ToRemark", dTO.ToRemark, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@CategoryId", dTO.CategoryId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@RequestId", dTO.RequestId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@IsActive", dTO.IsActive, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@UserId", dTO.UserId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Updatedby", dTO.Updatedby, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UpdatedOn", dTO.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@IsEditAction", dTO.IsEditAction, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@TrnFwdId", dTO.TrnFwdId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@IsComplete", dTO.IsComplete, DbType.Boolean, ParameterDirection.Input);

                    // Execute the insert query and retrieve the newly inserted record's ID
                    var Id = await db.QuerySingleAsync<int>(insert, parameters, transaction: transaction);

                    // Set response message for successful insert
                    saveResponse.Id = Id.ToString();
                    saveResponse.Message = "Data has been saved";

                }
                // Handle "Accept" action if Choice is 2
                if (dTO.Choice == 2)
                {
                    // SQL query to update the step counter (StepId = 4 for acceptance)
                    query2 = @" UPDATE TrnStepCounter set StepId = 4 where RequestId=@RequestId 
                                UPDATE TrnFwds set IsComplete = 0 where TrnFwdId=@TrnFwdId 
                                UPDATE TrnICardRequest set CardSerialNo=null ,ChipNo=null ,CardExportedOn=null, CardExportedByAspNetUserId=null, CardExportedByUserId=null, CardPrintedByAspNetUserId=null, CardPrintedByUserId=null where RequestId=@RequestId ";
                    await db.ExecuteAsync(query2, new { dTO.RequestId, dTO.TrnFwdId }, transaction: transaction);

                }
                /// Handle "Reject" action if Choice is 3
                else if (dTO.Choice == 3)
                {
                    // If forwarding information is provided, insert a new forward record
                    if (mTrnFwd != null)
                    {
                        insert = @"INSERT INTO TrnFwds(RequestId,ToUserId,FromUserId,FromAspNetUsersId,ToAspNetUsersId,UnitId,Remark,TypeId,IsComplete,IsActive,Updatedby,UpdatedOn,RemarksIds,FwdStatusId,StepId)
                                OUTPUT INSERTED.TrnFwdId
                                VALUES(@RequestId,@ToUserId,@FromUserId,@FromAspNetUsersId,@ToAspNetUsersId,@UnitId,@Remark,@TypeId,@IsComplete,@IsActive,@Updatedby,@UpdatedOn,@RemarksIds,@FwdStatusId,@StepId)";

                        // Set parameters for the insert query
                        var parameters = new DynamicParameters();
                        parameters.Add("@TrnFwdId", mTrnFwd.TrnFwdId, DbType.Int32, ParameterDirection.Output);
                        parameters.Add("@RequestId", mTrnFwd.RequestId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ToUserId", mTrnFwd.ToUserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@FromUserId", mTrnFwd.FromUserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@FromAspNetUsersId", mTrnFwd.FromAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ToAspNetUsersId", mTrnFwd.ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@UnitId", mTrnFwd.UnitId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Remark", mTrnFwd.Remark, DbType.String, ParameterDirection.Input, 100);
                        parameters.Add("@TypeId", mTrnFwd.TypeId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@IsComplete", mTrnFwd.IsComplete, DbType.Boolean, ParameterDirection.Input);
                        parameters.Add("@IsActive", mTrnFwd.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parameters.Add("@Updatedby", mTrnFwd.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@UpdatedOn", mTrnFwd.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("@RemarksIds", mTrnFwd.RemarksIds, DbType.String, ParameterDirection.Input, 100);
                        parameters.Add("@FwdStatusId", mTrnFwd.FwdStatusId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@StepId", mTrnFwd.StepId, DbType.Byte, ParameterDirection.Input);

                        // Execute the forward insertion query
                        var Id = await db.QuerySingleAsync<int>(insert, parameters, transaction: transaction);

                        query6 = @"INSERT INTO TrnNotification([Read],DisplayId,SentAspNetUsersId,ReciverAspNetUsersId,Url,RequestId,StepId,UpdatedOn)
                             VALUES(@Read,@DisplayId,@SentAspNetUsersId,@ReciverAspNetUsersId,@Url,@RequestId,@StepId,@UpdatedOn)";

                        int DisplayId = 0;

                        if (dTO.ApplyForId == 1)
                        {
                            DisplayId = mTrnFwd.StepId;
                        }
                        else
                        {
                            DisplayId = mTrnFwd.StepId + 10;
                        }

                        var parameters2 = new DynamicParameters();
                        parameters2.Add("@Read", false, DbType.Boolean, ParameterDirection.Input);
                        parameters2.Add("@DisplayId", DisplayId, DbType.Int32, ParameterDirection.Input);
                        parameters2.Add("@SentAspNetUsersId", mTrnFwd.FromAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters2.Add("@ReciverAspNetUsersId", mTrnFwd.ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters2.Add("@Url", null, DbType.String, ParameterDirection.Input);
                        parameters2.Add("@RequestId", mTrnFwd.RequestId, DbType.Int32, ParameterDirection.Input);
                        parameters2.Add("@StepId", mTrnFwd.StepId, DbType.Byte, ParameterDirection.Input);
                        parameters2.Add("@UpdatedOn", mTrnFwd.UpdatedOn, DbType.DateTime, ParameterDirection.Input);

                        await db.ExecuteAsync(query6, parameters2, transaction: transaction);
                    }
                    // SQL queries to reset the XmlFiles and update the step counter for rejection
                    query3 = @" UPDATE AFSAC2.dbo.XmlFilesFwdLog SET XmlFiles=@XmlFiles WHERE RequestId=@RequestId
                                UPDATE TrnStepCounter set StepId = 9 where RequestId=@RequestId
                                UPDATE TrnICardRequest set CardSerialNo=null ,ChipNo=null ,CardExportedOn=null, CardExportedByAspNetUserId=null, CardExportedByUserId=null, CardPrintedByAspNetUserId=null, CardPrintedByUserId=null where RequestId=@RequestId
                                Update BasicDetails set IsLock = 0 where BasicDetailId=@BasicDetailId";
                    await db.ExecuteAsync(query3, new { dTO.RequestId, XmlFiles, dTO.BasicDetailId }, transaction: transaction);
                }


                // Commit the transaction if all operations succeed
                transaction.Commit();

                // Set response for successful transaction
                saveResponse.CurrentTime = dTO.UpdatedOn ?? DateTime.Now;
                saveResponse.Result = true;
                return saveResponse;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "FaultyCardDB->SaveFaultyCard");
                saveResponse.Result = false;
                saveResponse.Message = ex.Message;
                return saveResponse;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }

        public async Task<DTOBeforeFaultyCardReportResponse> CheckBeforeFaultyCardReport(DTOFaultyCardRequest dTOFaulty)
        {
            var response = new DTOBeforeFaultyCardReportResponse();
            try
            {
                string query="";
                if (dTOFaulty.Claim == true)
                {
                    if (dTOFaulty.TrnFaultyCardId > 0)
                    {
                        query = @"SELECT 
                              (SELECT MAX(TrnFwdId) 
                                FROM TrnFwds 
                                WHERE RequestId = currentReq.RequestId) AS TrnFwdId,
                                currentReq.RequestId,ISNULL(bd.ApplyForId, basic_2.ApplyForId) AS ApplyForId,ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) AS BasicDetailId,
                                       CASE
                                            WHEN ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) IS NULL THEN 0
                                            WHEN faul.IsEditAction = 1 THEN 0
                                            WHEN currentReq.StatusId IN (2,3) THEN 0
                                            WHEN stepcount.StepId != 14 THEN 0
                                            ELSE 1
                                        END AS Result,
		                                case
                                            WHEN ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) IS NULL THEN 'Invalid Id.'
                                            WHEN faul.IsEditAction = 1 THEN 'This action has already been completed by you.'
                                            WHEN currentReq.StatusId IN (2,3) THEN 'The application is no longer active.'
                                            WHEN stepcount.StepId != 14 THEN 'The application is currently being processed.'
		                                    ELSE 'Valid'
		                                END as Message
                                FROM TrnFaultyCard faul
                                INNER JOIN TrnICardRequest currentReq on faul.RequestId=currentReq.RequestId
                                INNER JOIN TrnStepCounter stepcount on currentReq.RequestId=stepcount.RequestId
                                LEFT JOIN BasicDetails bd on currentReq.BasicDetailId=bd.BasicDetailId
                                LEFT JOIN AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId=currentReq.BasicDetailId
                                WHERE faul.TrnFaultyCardId = @TrnFaultyCardId;";
                    }
                    else
                    {
                        query = @"SELECT 
                                (SELECT MAX(TrnFwdId) 
                                    FROM TrnFwds 
                                    WHERE RequestId = currentReq.RequestId) AS TrnFwdId,
                                    currentReq.RequestId,ISNULL(bd.ApplyForId, basic_2.ApplyForId) AS ApplyForId,ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) AS BasicDetailId,
                                        CASE
                                            WHEN ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) IS NULL THEN 0
                                            WHEN faul.IsEditAction = 0 THEN 0
                                            WHEN currentReq.StatusId IN (2,3) THEN 0
                                            WHEN stepcount.StepId != 6 THEN 0
                                            ELSE 1
                                        END AS Result,
		                                case
                                            WHEN ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) IS NULL THEN 'Invalid Id.'
                                            WHEN faul.IsEditAction = 0 THEN 'The faulty request already exists, first take action!'
                                            WHEN currentReq.StatusId IN (2,3) THEN 'The application is no longer active.'
                                            WHEN stepcount.StepId != 6 THEN 'The application is currently being processed.'
		                                    ELSE 'Valid'
		                                END as Message
                                FROM TrnICardRequest currentReq
                                INNER JOIN TrnStepCounter stepcount on currentReq.RequestId=stepcount.RequestId 
                                LEFT JOIN BasicDetails bd on currentReq.BasicDetailId=bd.BasicDetailId
                                LEFT JOIN AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId=currentReq.BasicDetailId
                                LEFT JOIN TrnFaultyCard faul on faul.TrnFaultyCardId = (SELECT MAX(fal.TrnFaultyCardId) FROM TrnFaultyCard fal WHERE fal.RequestId =currentReq.RequestId)
                                WHERE currentReq.RequestId = @RequestId;";
                                                    }

                }
                else
                {
                    query = @"SELECT
                            (SELECT MAX(TrnFwdId) 
                                FROM TrnFwds 
                                WHERE RequestId = currentReq.RequestId) AS TrnFwdId,
                                currentReq.RequestId,ISNULL(bd.ApplyForId, basic_2.ApplyForId) AS ApplyForId,ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) AS BasicDetailId,
                                    CASE
                                        WHEN ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) IS NULL THEN 0
                                        WHEN ISNULL(bd.UnitId, basic_2.UnitId) != @UnitId THEN 0
                                        WHEN EXISTS (
                                            SELECT 1
                                            FROM TrnFaultyCard faul
                                            WHERE faul.RequestId = currentReq.RequestId
                                                AND faul.IsComplete = 0
                                        ) THEN 0
                                        WHEN currentReq.StatusId IN (2,3) THEN 0
                                        WHEN stepcount.StepId != 14 THEN 0
                                        ELSE 1
                                    END AS Result,
		                            case
                                        WHEN ISNULL(bd.BasicDetailId, basic_2.BasicDetailId) IS NULL THEN 'Invalid Id.'
                                        WHEN ISNULL(bd.UnitId, basic_2.UnitId) != @UnitId THEN 'You are not an authorized user.'
                                        WHEN EXISTS (
                                            SELECT 1
                                            FROM TrnFaultyCard faul
                                            WHERE faul.RequestId = currentReq.RequestId
                                                AND faul.IsComplete = 0
                                        ) THEN 'The faulty request already exists!'
                                        WHEN currentReq.StatusId IN (2,3) THEN 'The application is no longer active.'
                                        WHEN stepcount.StepId != 14 THEN 'The application is currently being processed.'
		                                ELSE 'Valid'
		                            END as Message
                            FROM TrnICardRequest currentReq
                            INNER JOIN TrnStepCounter stepcount on currentReq.RequestId=stepcount.RequestId 
                            LEFT JOIN BasicDetails bd on currentReq.BasicDetailId=bd.BasicDetailId
                            LEFT JOIN AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId=currentReq.BasicDetailId
                            WHERE currentReq.RequestId = @RequestId;";
                }


                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@RequestId", dTOFaulty.RequestId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitId", dTOFaulty.UnitId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TrnFaultyCardId", dTOFaulty.TrnFaultyCardId, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.QueryFirstOrDefaultAsync<DTOBeforeFaultyCardReportResponse>(query, parameters);
                    return result ?? new DTOBeforeFaultyCardReportResponse
                    {
                        Result = false,
                        Message = "Application not found",
                    };
                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "DestructionCardDB->CheckBeforeDestructionCardReport");
                response.Result = false;
                response.Message = "Something went wrong";
                return response;
            }
        }

    }
}
