using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Healpers;
using DataAccessLayer.Logger;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Data;

namespace DataAccessLayer
{
    /// <summary>
    /// Data Access Layer for BasicDetail entity, providing database operations.
    /// and implements the IBasicDetailDB interface.
    /// for basic CRUD operations.
    /// </summary>
    public class BasicDetailDB : GenericRepositoryDL<BasicDetail>, IBasicDetailDB
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly UserManager<ApplicationUser> userManager;//    For user management
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly IDataProtector protector;// For data protection
        private readonly ILogger<BasicDetailDB> _logger;// For logging
        private readonly IServiceProvider _serviceProvider;// For accessing services

        //constants for dispatch card status and pending
        public BasicDetailDB(ApplicationDbContext context, DapperContext contextDP, IServiceProvider serviceProvider, IDataProtectionProvider dataProtectionProvider, ILogger<BasicDetailDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _serviceProvider = serviceProvider;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            this.userManager = userManager;
        }
        
        
        /// <summary>
        /// Asynchronously exports the dispatch card data for a list of RequestIds and returns it as a list of DTODispatchCardForCSVResponse objects.
        /// This method retrieves data from the TrnICardRequest, BasicDetails, and MRank tables and formats it for CSV export.
        /// </summary>
        /// <param name="RequestIds">An array of RequestIds for which the dispatch card data is to be retrieved.</param>
        /// <returns>
        /// A list of DTODispatchCardForCSVResponse objects containing the dispatch card data for the provided RequestIds.
        /// </returns>
        public async Task<List<DTODispatchCardForCSVResponse>> ExportCsvFileForDispatchCard(int[] RequestIds)
        {
            // List to hold the results that will be returned as CSV response
            List<DTODispatchCardForCSVResponse> dTOs = new List<DTODispatchCardForCSVResponse>();

            // SQL query to select dispatch card data based on the provided RequestIds
            string query = @"SELECT req.RequestId as ApplId,
                            ranks.RankAbbreviation as RankName,
                            basi.FName, basi.LName, basi.ServiceNo,
                            req.ChipNo, req.CardSerialNo
                         from TrnICardRequest req
                         INNER JOIN BasicDetails basi on req.BasicDetailId = basi.BasicDetailId
                         INNER JOIN MRank ranks on ranks.RankId = basi.RankId
                         WHERE req.RequestId IN (SELECT RequestId FROM @RequestIds)";  // Use parameterized query for RequestIds

            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    // Create a DataTable to hold the RequestIds for the query
                    var table = new DataTable();
                    table.Columns.Add("RequestId", typeof(int));

                    // Add each RequestId from the array into the DataTable
                    foreach (var id in RequestIds)
                    {
                        table.Rows.Add(id);
                    }

                    // Create a DynamicParameters object to pass the table-valued parameter
                    var parameters = new DynamicParameters();
                    parameters.Add("@RequestIds", table.AsTableValuedParameter("RequestIdList"));  // Pass table as a TVP

                    // Execute the query asynchronously and map the results to DTODispatchCardForCSVResponse objects
                    dTOs = (await connection.QueryAsync<DTODispatchCardForCSVResponse>(query, parameters)).ToList();
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the query execution
                _logger.LogError(1001, ex, "BasicDetailDB->ExportCsvFileForDispatchCard");
            }

            // Return the list of dispatch card data
            return dTOs;
        }


        /// <summary>
        /// Retrieves the Dispatch Card Status List based on Claim Value and dialog-specific filters.
        /// </summary>
        /// <param name="dTO">The DTO that holds the data for filtering and pagination.</param>
        /// <param name="ClaimValue">The claim value that determines which query to execute.</param>
        /// <returns>A response object containing a list of dispatch card statuses, along with pagination information.</returns>
        public async Task<DTODataTablesWithSelectedIdsResponse<DTODispatchCardStatusResponse>> GetDispatchCardStatusListForDialog(DTODataTablesRequestForCardStatusList dTO, byte ClaimValue)
        {
            // Declare the necessary variables for query construction
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            string searchFilter = "";
            byte finalValue=0;
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection;

            // Map the allowed sort columns to the DB fields for flexibility
            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ApplyForId"] = "mappl.ApplyForId",
                ["RequestId"] = "req.RequestId",
                ["StepId"] = "stepc.StepId",
                ["ArmedAbbreviation"] = "marmed.Abbreviation",
                ["ServiceNo"] = "basi.ServiceNo",
                ["RecordOfficeName"] = "mrec.Abbreviation",
                ["RegimentalName"] = "regi.Abbreviation",
                ["ChipNo"] = "req.ChipNo",
                ["CardSerialNo"] = "req.CardSerialNo",
                ["SUSNo"] = "munit.Sus_no"
            };
            // Depending on the ClaimValue, we adjust the SELECT, JOIN, and WHERE clauses
            if (ClaimValue == 1)
            {
                selectFields = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,regi.RegId,mrec.Abbreviation as RecordOfficeName,mrec.RecordOfficeId,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo";
                fromJoinClause = @"from TrnStepCounter stepc
                                    INNER JOIN TrnICardRequest req on stepc.RequestId=req.RequestId
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MApplyFor mappl on mappl.ApplyForId=basi.ApplyForId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                    INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                                    LEFT JOIN MRegimental regi on regi.RegId=basi.RegimentalId
                                    LEFT JOIN MRecordOffice mrec on req.RecordOfficeId = mrec.RecordOfficeId";
                whereClause = @"WHERE
                            (stepc.StepId=6)";
            }
            else if (ClaimValue == 2)
            {
                selectFields = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo";
                fromJoinClause = @"from TrnStepCounter stepc
                                    INNER JOIN TrnICardRequest req on stepc.RequestId=req.RequestId
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MApplyFor mappl on mappl.ApplyForId=basi.ApplyForId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                    INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                                    INNER JOIN OROMapping oro on req.RecordOfficeId = oro.RecordOfficeId
                                    INNER JOIN MRecordOffice mrec on oro.RecordOfficeId = mrec.RecordOfficeId";
                whereClause = @"WHERE
                                (stepc.StepId=12)";
            }
            else if (ClaimValue == 3)
            {
                selectFields = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo";
                fromJoinClause = @"from TrnStepCounter stepc
                                    INNER JOIN TrnICardRequest req on stepc.RequestId=req.RequestId
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MApplyFor mappl on mappl.ApplyForId=basi.ApplyForId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                    INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                                    INNER JOIN MRegimental regi on regi.RegId=basi.RegimentalId";
                whereClause = @"WHERE
                                (stepc.StepId=12)";
            }
            else
            {
                selectFields = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,regi.RegId,mrec.Abbreviation as RecordOfficeName,mrec.RecordOfficeId,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo";
                fromJoinClause = @"from TrnStepCounter stepc
                            INNER JOIN TrnICardRequest req on stepc.RequestId=req.RequestId
                            INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                            INNER JOIN MApplyFor mappl on mappl.ApplyForId=basi.ApplyForId
                            INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                            INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                            INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                            INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                            LEFT JOIN MRegimental regi on regi.RegId=basi.RegimentalId
                            LEFT JOIN MRecordOffice mrec on req.RecordOfficeId = mrec.RecordOfficeId";
                whereClause = @"WHERE
                                (stepc.StepId=14)";
            }
            if (!string.IsNullOrWhiteSpace(dTO.SearchField) && !string.IsNullOrWhiteSpace(dTO.SearchText))
            {
                string safeField = dTO.SearchField.Trim().ToLower();
                switch (safeField)
                {
                    //case "categery":
                    //    searchFilter = @"AND mappl.Name=@SearchText";
                    //    break;
                    //case "requestid":
                    //    searchFilter = @"AND req.RequestId=@SearchText";
                    //    break;
                    case "serviceno":
                        searchFilter = @"AND basi.ServiceNo LIKE '%' + @SearchText + '%'";
                        break;
                    case "susno":
                        searchFilter = @"AND unit.UnitMapId=@SearchText";
                        break;
                    case "regimentalname":
                        searchFilter = @"AND mappl.ApplyForId = 2 AND regi.RegId=@SearchText";
                        break;
                    case "recordofficename":
                        searchFilter = @"AND mappl.ApplyForId = 1 AND mrec.RecordOfficeId=@SearchText";
                        break;
                    //case "chipno":
                    //    searchFilter = @"AND req.ChipNo LIKE '%' + @SearchText + '%'";
                    //    break;
                    //case "cardserialno":
                    //    searchFilter = @"AND req.CardSerialNo LIKE '%' + @SearchText + '%'";
                    //    break;
                    //case "status":
                    //    if (dTO.SearchText?.Trim().ToLower() == "pending" || dTO.SearchText?.Trim().ToLower() == "card distribute")
                    //    {
                    //        finalValue = PendingStepId;
                    //        searchFilter = @"AND stepc.StepId = @FinalStepId";
                    //    }
                    //    else
                    //    {
                    //        finalValue = DispatchStepId;
                    //        searchFilter = @"AND stepc.StepId >= @FinalStepId";
                    //    }
                    //    break;
                    default:
                        // optional fallback to global filter
                        searchFilter = @"";
                        break;
                }
            }

            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "basi.ServiceNo";
                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause} {searchFilter}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";
                string queryRequestIds = $@"SELECT req.RequestId {fromJoinClause} {whereClause} {searchFilter}";



                    using (var connection = _contextDP.CreateConnection())
                    {
                        dTO.SearchText = string.IsNullOrEmpty(dTO.SearchText) ? string.Empty : dTO.SearchText.Trim();
                        var parameters = new DynamicParameters();
                        parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                        if (ClaimValue == 0)
                        {
                            parameters.Add("@SearchText", dTO.SearchText, DbType.String, ParameterDirection.Input);
                        }
                        else
                        {
                            parameters.Add("@SearchText", dTO.SearchText != string.Empty ? Convert.ToInt32(dTO.SearchText) : 0 , DbType.Int32, ParameterDirection.Input);
                        }

                        parameters.Add("@FinalStepId", finalValue, DbType.Byte, ParameterDirection.Input);
                        //parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                        var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                        var records = (await ret.ReadAsync<DTODispatchCardStatusResponse>()).ToList();
                        var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                        List<int>? selectedIds = new List<int>();

                        if (dTO.AllChecked == true && (string.IsNullOrEmpty(dTO.SearchText) || dTO.SearchTextChanged == true))
                        {
                            var result = await connection.QueryMultipleAsync(queryRequestIds, parameters);
                            selectedIds = (await result.ReadAsync<int>()).ToList();
                        }
                        else
                        {
                            selectedIds = null;
                        }

                    var responseData = new DTODataTablesWithSelectedIdsResponse<DTODispatchCardStatusResponse>
                        {
                            draw = dTO.Draw,
                            recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                            recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                            selectedIds = selectedIds,
                            data = records,
                        };
                        return responseData;
                    }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllDispatchCard");
                List<DTODispatchCardStatusResponse> dTOCards = new List<DTODispatchCardStatusResponse>();
                var responseData = new DTODataTablesWithSelectedIdsResponse<DTODispatchCardStatusResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    selectedIds = null,
                    data = dTOCards
                };
                return responseData;
            }
        }


        /// <summary>
        /// Retrieves a list of dispatch card status data for export based on claim value and filtering options.
        /// </summary>
        /// <param name="ClaimValue">The claim value to determine the step ID for filtering the dispatch status (1, 2, 3, or other values).</param>
        /// <param name="Data">The data object containing filtering criteria for the export such as unchedRequestId, checkedRequestId, and Allstatus.</param>
        /// <returns>A DTO response containing the filtered dispatch card status list.</returns>
        public async Task<DTODataTablesResponse<DTODispatchCardStatusResponse>> GetDispatchCardStatusListForExport(byte ClaimValue, DTOExportDispatch Data)
        {
            string query = "";
            string wherequery = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();



            // Query construction based on ClaimValue (StepId logic)
            if (ClaimValue == 1)
            {
                // Query for ClaimValue 1 (StepId = 6)
                query = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,mrec.Abbreviation as RecordOfficeName,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo,
                        CASE 
                            WHEN stepc.StepId = 6 THEN 'Pending' 
                            ELSE 'Unknown' 
                        END AS Status
                        from TrnStepCounter stepc
                        INNER JOIN TrnICardRequest req on stepc.RequestId=req.RequestId
                        INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                        INNER JOIN MApplyFor mappl on mappl.ApplyForId=basi.ApplyForId
                        INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                        INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                        INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                        INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                        LEFT JOIN MRegimental regi on regi.RegId=basi.RegimentalId
                        LEFT JOIN MRecordOffice mrec on req.RecordOfficeId = mrec.RecordOfficeId";
                wherequery = @"WHERE
                            (stepc.StepId=6)

                           ";
            }
            else if (ClaimValue == 2 || ClaimValue == 3)
            {
                // Query for ClaimValue 2 or 3 (StepId = 12)
                query = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,mrec.Abbreviation as RecordOfficeName,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo,
                        CASE 
                            WHEN stepc.StepId = 12 THEN 'Pending' 
                            ELSE 'Unknown' 
                        END AS Status
                        from TrnStepCounter stepc
                        INNER JOIN TrnICardRequest req on stepc.RequestId=req.RequestId
                        INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                        INNER JOIN MApplyFor mappl on mappl.ApplyForId=basi.ApplyForId
                        INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                        INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                        INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                        INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                        LEFT JOIN MRegimental regi on regi.RegId=basi.RegimentalId
                        LEFT JOIN MRecordOffice mrec on req.RecordOfficeId = mrec.RecordOfficeId";
                wherequery = @"WHERE
                            (stepc.StepId=12)
                           ";
            }
            else
            {
                // Query for other ClaimValues (StepId = 14)
                query = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,mrec.Abbreviation as RecordOfficeName,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo,
                        CASE 
                            WHEN stepc.StepId = 14 THEN 'Pending' 
                            ELSE 'Unknown' 
                        END AS Status
                        from TrnStepCounter stepc
                        INNER JOIN TrnICardRequest req on stepc.RequestId=req.RequestId
                        INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                        INNER JOIN MApplyFor mappl on mappl.ApplyForId=basi.ApplyForId
                        INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                        INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                        INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                        INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                        LEFT JOIN MRegimental regi on regi.RegId=basi.RegimentalId
                        LEFT JOIN MRecordOffice mrec on req.RecordOfficeId = mrec.RecordOfficeId";
                wherequery = @"WHERE
                            (stepc.StepId=14)
                           ";
            }

            try
            {
               
                var multiQuery = query = $@" SELECT
                        {query} {wherequery}
                        
                        ";
                var Dataforfilter = @"";
                if (Data.unchedRequestId != null && Data.unchedRequestId.Length > 0)
                {
                    Dataforfilter = @" And stepc.RequestId not in @unchedRequestId";
                    Dataforfilter = query = $@" {query} {Dataforfilter}";
                }
                if (Data.checkedRequestId != null && Data.checkedRequestId.Length > 0)
                {
                    Dataforfilter = @" And stepc.RequestId in @checkedRequestId";
                    Dataforfilter = query = $@" {query}  {Dataforfilter}";
                }

                using (var connection = _contextDP.CreateConnection())
                {

                    var parameters = new DynamicParameters();


                    if (Data.unchedRequestId != null && Data.unchedRequestId.Length > 0)
                    {
                        parameters.Add("@unchedRequestId", Data.unchedRequestId); // List<int> or int[]
                    }

                    else if (Data.checkedRequestId != null && Data.checkedRequestId.Length > 0 && Data.Allstatus == false)
                    {
                        parameters.Add("@checkedRequestId", Data.checkedRequestId); // List<int> or int[]
                    }
                    else if(Data.Allstatus == false)
                    {
                        //responseData.data = null;
                    }

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTODispatchCardStatusResponse>()).ToList();
                 
                    var responseData = new DTODataTablesResponse<DTODispatchCardStatusResponse>
                    {
                        data = records,
                    };
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllDispatchCard");
                List<DTODispatchCardStatusResponse> dTOCards = new List<DTODispatchCardStatusResponse>();
                var responseData = new DTODataTablesResponse<DTODispatchCardStatusResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOCards
                };
                return responseData;
            }
        }


        /// <summary>
        /// Retrieves the dispatch card data for the dialog based on the provided request parameters.
        /// </summary>
        /// <param name="dTO">The data transfer object containing the request parameters, including search terms, sorting, and pagination information.</param>
        /// <returns>A <see cref="DTODataTablesResponse{DTOCardDispatchDialogResponse}"/> containing the dispatch card data, total records, and filtered records.</returns>
        public async Task<DTODataTablesWithSelectedIdsResponse<DTOCardDispatchDialogResponse>> GetDispatchCardDataForDialog(DTODataTablesRequestForCardDispatchDialog dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            string searchFilter = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection;

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            { 
                ["RequestId"] = "req.RequestId",
                ["ArmedAbbreviation"] = "marmed.Abbreviation",
                ["ServiceNo"] = "basi.ServiceNo",
                ["RecordOfficeName"] = "mrec.Abbreviation",
                ["RegimentalName"] = "regi.Abbreviation",
                ["ChipNo"] = "req.ChipNo",
                ["CardSerialNo"] = "req.CardSerialNo",
                ["SUSNo"] = "munit.Sus_no"
            };
            selectFields = @"dcm.DispatchCardMappingId,req.RequestId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,mrec.Abbreviation as RecordOfficeName,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo";
            fromJoinClause = @"from TrnDispatchCardMapping dcm
                                INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId
                                INNER JOIN TrnICardRequest req on dcm.ChipNo=req.ChipNo
                                INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                                LEFT JOIN MRegimental regi on regi.RegId=basi.RegimentalId
                                LEFT JOIN MRecordOffice mrec on dcard.RecordOfficeId = mrec.RecordOfficeId";
            whereClause = @"WHERE dcm.DispatchCardId=@DispatchCardId";
            searchFilter = @"AND (
                                req.RequestId LIKE '%' + @SearchTerm + '%' OR
                                marmed.Abbreviation LIKE '%' + @SearchTerm + '%' OR
                                basi.ServiceNo LIKE '%' + @SearchTerm + '%' OR
                                req.ChipNo LIKE '%' + @SearchTerm + '%' OR
                                req.CardSerialNo LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "basi.ServiceNo";
                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause} {searchFilter}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";
                string queryRequestIds = $@"SELECT req.RequestId {fromJoinClause} {whereClause} {searchFilter}";

                using (var connection = _contextDP.CreateConnection())
                {
                    dTO.searchValue = string.IsNullOrEmpty(dTO.searchValue) ? string.Empty : dTO.searchValue.Trim();
                    var parameters = new DynamicParameters();
                    parameters.Add("@DispatchCardId", dTO.DispatchCardId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTOCardDispatchDialogResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    List<int>? selectedIds = new List<int>();

                    if (dTO.AllChecked == true && (string.IsNullOrEmpty(dTO.searchValue) || dTO.SearchTextChanged == true))
                    {
                        var result = await connection.QueryMultipleAsync(queryRequestIds, parameters);
                        selectedIds = (await result.ReadAsync<int>()).ToList();
                    }
                    else
                    {
                        selectedIds = null;
                    }

                    var responseData = new DTODataTablesWithSelectedIdsResponse<DTOCardDispatchDialogResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        selectedIds = selectedIds,
                        data = records,
                    };
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllDispatchCard");
                List<DTOCardDispatchDialogResponse> dTOCards = new List<DTOCardDispatchDialogResponse>();
                var responseData = new DTODataTablesWithSelectedIdsResponse<DTOCardDispatchDialogResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    selectedIds = null,
                    data = dTOCards
                };
                return responseData;
            }
        }


        /// <summary>
        /// Retrieves all dispatch cards based on the claim value and search parameters.
        /// </summary>
        /// <param name="dTO">The DTO containing data table request parameters such as ClaimValue, SearchTerm, and pagination details.</param>
        /// <returns>A DTO containing the response data including dispatch card details and pagination info.</returns>

        public async Task<DTODataTablesResponse<DTODispatchCardListResponse>> GetAllDispatchCard(DTODataTablesRequestForCardDispatch dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection;
            if (dTO.ClaimValue == 1)
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ApplyFor"] = "mappl.Name",
                    ["DispatchCardId"] = "dcard.DispatchCardId",
                    ["DispatchDate"] = "dcard.DispatchDate",
                    ["RegimentalName"] = "regi.Abbreviation",
                    ["RecordOfficeName"] = "mrec.Name",
                    ["ToSUSNo"] = "toMuni.Sus_no",
                    ["ReceiptDate"] = "dcard.ReceiptDate"
                };
                selectFields = @"dcard.DispatchCardId,dcard.Step,mappl.Name as ApplyFor,mappl.ApplyForId,regi.Abbreviation RegimentalName,mrec.Name as RecordOfficeName,dcard.OutDate,dcard.ReceiptDate,dcard.DispatchDate,mdis.Description as DispatchMode,dcard.RefOfDispatch,dcard.NameOfCourierIncharge,dcard.UploadFilePath,dcard.FromRemark,dcard.ToRemark,fromMuni.Abbreviation as FromUnit,toMuni.Abbreviation as ToUnit,concat(toMuni.Sus_no,toMuni.Suffix) as ToSUSNo,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID,dcard.IsComplete,dcard.IsActive,dcard.UpdatedOn";
                fromJoinClause = @"from TrnDispatchCard dcard 
                                    INNER JOIN MApplyFor mappl on mappl.ApplyForId=dcard.ApplyForId
                                    INNER JOIN MDispatchMode mdis on dcard.DispatchModeId =mdis.DispatchModeId
                                    INNER JOIN MapUnit fromunit on dcard.FromUnitId=fromunit.UnitMapId
                                    INNER JOIN MUnit fromMuni on fromunit.UnitId=fromMuni.UnitId
                                    INNER JOIN MapUnit tounit on dcard.ToUnitId=tounit.UnitMapId
                                    INNER JOIN MUnit toMuni on tounit.UnitId=toMuni.UnitId
                                    INNER JOIN UserProfile fromUp on dcard.FromUserId=fromUp.UserId
                                    INNER JOIN MRank fromRanks on fromUp.RankId=fromRanks.RankId
                                    INNER JOIN UserProfile toUp on dcard.ToUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers fromAspUser on dcard.FromAspNetUsersId = fromAspUser.Id
                                    INNER JOIN AspNetUsers toAspUser on dcard.ToAspNetUsersId = toAspUser.Id
                                    LEFT JOIN MRegimental regi on regi.RegId=dcard.RegId
                                    LEFT JOIN MRecordOffice mrec on dcard.RecordOfficeId = mrec.RecordOfficeId ";
                whereClause = @"WHERE
                            dcard.Step=1
                            AND (
                                regi.Abbreviation LIKE '%' + @SearchTerm + '%' OR
                                mrec.Name LIKE '%' + @SearchTerm + '%' OR
                                dcard.DispatchCardId LIKE '%' + @SearchTerm + '%' OR
                                concat(toMuni.Sus_no,toMuni.Suffix) LIKE '%' + @SearchTerm + '%' OR
                                mappl.Name LIKE '%' + @SearchTerm + '%'
                                )";
            }
            else if (dTO.ClaimValue == 2)
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Step"] = "dcard.Step",
                    ["DispatchCardId"] = "dcard.DispatchCardId",
                    ["FromSUSNo"] = "fromMuni.Sus_no",
                    ["ToSUSNo"] = "toMuni.Sus_no",
                    ["DispatchDate"] = "dcard.DispatchDate",
                    ["ReceiptDate"] = "dcard.ReceiptDate"
                };
                selectFields = @"dcard.DispatchCardId,dcard.Step,mappl.Name as ApplyFor,mappl.ApplyForId,mrec.Name as RecordOfficeName,dcard.OutDate,dcard.ReceiptDate,dcard.DispatchDate,mdis.Description as DispatchMode,dcard.RefOfDispatch,dcard.NameOfCourierIncharge,dcard.UploadFilePath,dcard.FromRemark,dcard.ToRemark,fromMuni.Abbreviation as FromUnit,concat(fromMuni.Sus_no,fromMuni.Suffix) as FromSUSNo,toMuni.Abbreviation as ToUnit,concat(toMuni.Sus_no,toMuni.Suffix) as ToSUSNo,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID,dcard.IsComplete,dcard.IsActive,dcard.UpdatedOn";
                fromJoinClause = @"from TrnDispatchCard dcard 
                                    INNER JOIN MApplyFor mappl on mappl.ApplyForId=dcard.ApplyForId
                                    INNER JOIN MDispatchMode mdis on dcard.DispatchModeId =mdis.DispatchModeId
                                    INNER JOIN MapUnit fromunit on dcard.FromUnitId=fromunit.UnitMapId
                                    INNER JOIN MUnit fromMuni on fromunit.UnitId=fromMuni.UnitId
                                    INNER JOIN MapUnit tounit on dcard.ToUnitId=tounit.UnitMapId
                                    INNER JOIN MUnit toMuni on tounit.UnitId=toMuni.UnitId
                                    INNER JOIN UserProfile fromUp on dcard.FromUserId=fromUp.UserId
                                    INNER JOIN MRank fromRanks on fromUp.RankId=fromRanks.RankId
                                    INNER JOIN UserProfile toUp on dcard.ToUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers fromAspUser on dcard.FromAspNetUsersId = fromAspUser.Id
                                    INNER JOIN AspNetUsers toAspUser on dcard.ToAspNetUsersId = toAspUser.Id
                                    INNER JOIN OROMapping oro on dcard.RecordOfficeId = oro.RecordOfficeId
                                    INNER JOIN MRecordOffice mrec on oro.RecordOfficeId = mrec.RecordOfficeId ";
                whereClause = @"WHERE
                                oro.TDMId=@TDMId
                                AND (
                                    dcard.Step LIKE '%' + @SearchTerm + '%' OR
                                    dcard.DispatchCardId LIKE '%' + @SearchTerm + '%' OR
                                    concat(fromMuni.Sus_no,fromMuni.Suffix) LIKE '%' + @SearchTerm + '%' OR
                                    concat(toMuni.Sus_no,toMuni.Suffix) LIKE '%' + @SearchTerm + '%'
                                )";
            }
            else if (dTO.ClaimValue == 3)
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Step"] = "dcard.Step",
                    ["DispatchCardId"] = "dcard.DispatchCardId",
                    ["FromSUSNo"] = "fromMuni.Sus_no",
                    ["ToSUSNo"] = "toMuni.Sus_no",
                    ["DispatchDate"] = "dcard.DispatchDate",
                    ["ReceiptDate"] = "dcard.ReceiptDate"
                };
                selectFields = @"dcard.DispatchCardId,dcard.Step,mappl.Name as ApplyFor,mappl.ApplyForId,regi.Abbreviation RegimentalName,dcard.OutDate,dcard.ReceiptDate,dcard.DispatchDate,mdis.Description as DispatchMode,dcard.RefOfDispatch,dcard.NameOfCourierIncharge,dcard.UploadFilePath,dcard.FromRemark,dcard.ToRemark,fromMuni.Abbreviation as FromUnit,concat(fromMuni.Sus_no,fromMuni.Suffix) as FromSUSNo,toMuni.Abbreviation as ToUnit,concat(toMuni.Sus_no,toMuni.Suffix) as ToSUSNo,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID,dcard.IsComplete,dcard.IsActive,dcard.UpdatedOn";
                fromJoinClause = @"from TrnDispatchCard dcard 
                                    INNER JOIN MApplyFor mappl on mappl.ApplyForId=dcard.ApplyForId
                                    INNER JOIN MDispatchMode mdis on dcard.DispatchModeId=mdis.DispatchModeId
                                    INNER JOIN MapUnit fromunit on dcard.FromUnitId=fromunit.UnitMapId
                                    INNER JOIN MUnit fromMuni on fromunit.UnitId=fromMuni.UnitId
                                    INNER JOIN MapUnit tounit on dcard.ToUnitId=tounit.UnitMapId
                                    INNER JOIN MUnit toMuni on tounit.UnitId=toMuni.UnitId
                                    INNER JOIN UserProfile fromUp on dcard.FromUserId=fromUp.UserId
                                    INNER JOIN MRank fromRanks on fromUp.RankId=fromRanks.RankId
                                    INNER JOIN UserProfile toUp on dcard.ToUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers fromAspUser on dcard.FromAspNetUsersId = fromAspUser.Id
                                    INNER JOIN AspNetUsers toAspUser on dcard.ToAspNetUsersId = toAspUser.Id
                                    INNER join MRegimental regi on dcard.RegId = regi.RegId";
                whereClause = @"WHERE
                            regi.UnitId=@UnitId
                            AND (
                                dcard.Step LIKE '%' + @SearchTerm + '%' OR
                                dcard.DispatchCardId LIKE '%' + @SearchTerm + '%' OR
                                concat(fromMuni.Sus_no,fromMuni.Suffix) LIKE '%' + @SearchTerm + '%' OR
                                concat(toMuni.Sus_no,toMuni.Suffix) LIKE '%' + @SearchTerm + '%'
                                )";
            }
            else
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ApplyFor"] = "mappl.Name",
                    ["DispatchCardId"] = "dcard.DispatchCardId",
                    ["FromSUSNo"] = "fromMuni.Sus_no",
                    ["RecordOfficeName"] = "mrec.Name",
                    ["RegimentalName"] = "regi.Abbreviation",
                    ["DispatchDate"] = "dcard.DispatchDate",
                    ["ReceiptDate"] = "dcard.ReceiptDate"
                };
                selectFields = @"dcard.DispatchCardId,dcard.Step,mappl.Name as ApplyFor,mappl.ApplyForId,regi.Abbreviation RegimentalName,mrec.Name as RecordOfficeName,dcard.OutDate,dcard.ReceiptDate,dcard.DispatchDate,mdis.Description as DispatchMode,dcard.RefOfDispatch,dcard.NameOfCourierIncharge,dcard.UploadFilePath,dcard.FromRemark,dcard.ToRemark,fromMuni.Abbreviation as FromUnit,concat(fromMuni.Sus_no,fromMuni.Suffix) as FromSUSNo,toMuni.Abbreviation as ToUnit,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID,dcard.IsComplete,dcard.IsActive,dcard.UpdatedOn";
                fromJoinClause = @"from TrnDispatchCard dcard 
                                    INNER JOIN MApplyFor mappl on mappl.ApplyForId=dcard.ApplyForId
                                    INNER JOIN MDispatchMode mdis on dcard.DispatchModeId =mdis.DispatchModeId
                                    INNER JOIN MapUnit fromunit on dcard.FromUnitId=fromunit.UnitMapId
                                    INNER JOIN MUnit fromMuni on fromunit.UnitId=fromMuni.UnitId
                                    INNER JOIN MapUnit tounit on dcard.ToUnitId=tounit.UnitMapId
                                    INNER JOIN MUnit toMuni on tounit.UnitId=toMuni.UnitId
                                    INNER JOIN UserProfile fromUp on dcard.FromUserId=fromUp.UserId
                                    INNER JOIN MRank fromRanks on fromUp.RankId=fromRanks.RankId
                                    INNER JOIN UserProfile toUp on dcard.ToUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers fromAspUser on dcard.FromAspNetUsersId = fromAspUser.Id
                                    INNER JOIN AspNetUsers toAspUser on dcard.ToAspNetUsersId = toAspUser.Id
                                    LEFT JOIN MRegimental regi on regi.RegId=dcard.RegId
                                    LEFT JOIN MRecordOffice mrec on dcard.RecordOfficeId = mrec.RecordOfficeId ";
                whereClause = @"WHERE
                                dcard.Step=2
                                AND dcard.ToUnitId=@UnitId
                                AND (
                                    mappl.Name LIKE '%' + @SearchTerm + '%' OR
                                    dcard.DispatchCardId LIKE '%' + @SearchTerm + '%' OR
                                    concat(fromMuni.Sus_no,fromMuni.Suffix) LIKE '%' + @SearchTerm + '%' OR
                                    mrec.Name LIKE '%' + @SearchTerm + '%' OR
                                    regi.Abbreviation LIKE '%' + @SearchTerm + '%'
                                )";
            }
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "toUp.ArmyNo";
                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    dTO.searchValue = string.IsNullOrEmpty(dTO.searchValue) ? string.Empty : dTO.searchValue.Trim();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UnitId", dTO.UnitId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TDMId", dTO.TDMId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTODispatchCardListResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTODispatchCardListResponse>
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
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllDispatchCard");
                List<DTODispatchCardListResponse> dTODispatchCardLists = new List<DTODispatchCardListResponse>();
                var responseData = new DTODataTablesResponse<DTODispatchCardListResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTODispatchCardLists
                };
                return responseData;
            }
        }


        /// <summary>
        /// Checks the dispatch card details for validity based on the request IDs and ClaimValue.
        /// </summary>
        /// <param name="RequestIds">An array of request IDs to be checked.</param>
        /// <param name="ClaimValue">The claim value that determines the step and remarks for validation.</param>
        /// <param name="dTO">The DTO containing dispatch request parameters like ApplyForId and RegId.</param>
        /// <returns>A list of DTOCardDispatchCheckRequest containing the dispatch card check results.</returns>
        public async Task<List<DTOCardDispatchCheckRequest>> CardDispatchCSVCheck(int[] RequestIds, byte ClaimValue, DTODispatchOutRequest dTO)
        {
            #region Old Code
            //byte StepId;
            //string Remarks = string.Empty;
            //if (ClaimValue == 1)
            //{
            //    StepId = 6;
            //    Remarks = "The card application is not available for printing.";
            //}
            //else
            //{
            //    StepId = 12;
            //    Remarks = "The card application is not in the Regiment/Officer Record Office.";
            //}
            //var response = new List<DTOCardDispatchCheckRequest>();
            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    // ✅ Get scoped services like DbContext inside the scope
            //    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    var connection = context.Database.GetDbConnection();
            //    try
            //    {
            //        if (connection.State != ConnectionState.Open)
            //            connection.Open(); // Keep connection open throughout

            //        foreach (var batchRecords in requests.Chunk(5000))
            //        {
            //            var resultInChunks = await Task.Run(() =>
            //            {
            //                if (ClaimValue == 1)
            //                {
            //                    if (dTO.ApplyForId == 1)
            //                    {
            //                        return (from record in batchRecords
            //                                join chipNoMatch in context.TrnICardRequest on record.ChipNo equals chipNoMatch.ChipNo into chipNoJoin
            //                                from chipNoExists in chipNoJoin.DefaultIfEmpty()
            //                                join stepStatus in context.TrnStepCounter on new { RequestId = chipNoExists?.RequestId ?? 0, StepId } equals new { stepStatus.RequestId, stepStatus.StepId } into stepStatusJoin
            //                                from stepStatus in stepStatusJoin.DefaultIfEmpty()
            //                                select new DTOCardDispatchCheckRequest
            //                                {
            //                                    ChipNo = record.ChipNo,
            //                                    RequestId = chipNoExists?.RequestId ?? 0,
            //                                    IsValid = chipNoExists != null && chipNoExists.RecordOfficeId == dTO.RecordOfficeId && stepStatus != null,
            //                                    Status = chipNoExists != null && chipNoExists.RecordOfficeId == dTO.RecordOfficeId && stepStatus != null ? "Valid" : "DbInvalid",
            //                                    Remarks = (chipNoExists == null ? "ChipNo not exists; " : "") +
            //                                              (chipNoExists != null && chipNoExists.RecordOfficeId != dTO.RecordOfficeId ? "ChipNo not Valid match to RecordOffice; " : "") +
            //                                              (chipNoExists != null && stepStatus == null ? Remarks : "")
            //                                }).ToList();
            //                    }
            //                    else
            //                    {
            //                        return (from record in batchRecords
            //                                join chipNoMatch in context.TrnICardRequest on record.ChipNo equals chipNoMatch.ChipNo into chipNoJoin
            //                                from chipNoExists in chipNoJoin.DefaultIfEmpty()
            //                                join bdMatch in context.BasicDetails on new { BasicDetailId = chipNoExists?.BasicDetailId ?? 0, RegimentalId = dTO.RegId } equals new { bdMatch.BasicDetailId, bdMatch.RegimentalId } into bdMatchJoin
            //                                from bdMatch in bdMatchJoin.DefaultIfEmpty()
            //                                join stepStatus in context.TrnStepCounter on new { RequestId = chipNoExists?.RequestId ?? 0, StepId } equals new { stepStatus.RequestId, stepStatus.StepId } into stepStatusJoin
            //                                from stepStatus in stepStatusJoin.DefaultIfEmpty()
            //                                select new DTOCardDispatchCheckRequest
            //                                {
            //                                    ChipNo = record.ChipNo,
            //                                    RequestId = chipNoExists?.RequestId ?? 0,
            //                                    IsValid = chipNoExists != null && bdMatch != null && stepStatus != null,
            //                                    Status = chipNoExists != null && bdMatch != null && stepStatus != null ? "Valid" : "DbInvalid",
            //                                    Remarks = (chipNoExists == null ? "ChipNo not exists; " : "") +
            //                                              (chipNoExists != null && bdMatch == null ? "ChipNo not Valid match to Regiment; " : "") +
            //                                              (chipNoExists != null && stepStatus == null ? Remarks : "")
            //                                }).ToList();
            //                    }
            //                }
            //                else
            //                {
            //                    return (from record in batchRecords
            //                            join chipNoMatch in context.TrnICardRequest on record.ChipNo equals chipNoMatch.ChipNo into chipNoJoin
            //                            from chipNoExists in chipNoJoin.DefaultIfEmpty()
            //                            join bdMatch in context.BasicDetails on new { BasicDetailId = chipNoExists?.BasicDetailId ?? 0, UnitId = dTO.ToUnitId } equals new { bdMatch.BasicDetailId, bdMatch.UnitId } into bdMatchJoin
            //                            from bdMatch in bdMatchJoin.DefaultIfEmpty()
            //                            join stepStatus in context.TrnStepCounter on new { RequestId = chipNoExists?.RequestId ?? 0, StepId } equals new { stepStatus.RequestId, stepStatus.StepId } into stepStatusJoin
            //                            from stepStatus in stepStatusJoin.DefaultIfEmpty()
            //                            select new DTOCardDispatchCheckRequest
            //                            {
            //                                ChipNo = record.ChipNo,
            //                                RequestId = chipNoExists?.RequestId ?? 0,
            //                                IsValid = chipNoExists != null && bdMatch != null && stepStatus != null,
            //                                Status = chipNoExists != null && bdMatch != null && stepStatus != null ? "Valid" : "DbInvalid",
            //                                Remarks = (chipNoExists == null ? "ChipNo not exists; " : "") +
            //                                          (chipNoExists != null && bdMatch == null ? "ChipNo not Valid match to Unit; " : "") +
            //                                          (chipNoExists != null && stepStatus == null ? Remarks : "")
            //                            }).ToList();
            //                }

            //            });
            //            response.AddRange(resultInChunks);
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Error: " + ex.Message);
            //    }
            //    finally
            //    {
            //        if (connection.State == ConnectionState.Open)
            //            connection.Close();
            //    }
            //}
            //return response;
            #endregion
            byte StepId;
            string Remarks = string.Empty;
            if (ClaimValue == 1)
            {
                StepId = 6;
                Remarks = "The card application is not available for printing.";
            }
            else
            {
                StepId = 12;
                Remarks = "The card application is not in the Regiment/Officer Record Office.";
            }
            var response = new List<DTOCardDispatchCheckRequest>();
            using (var scope = _serviceProvider.CreateScope())
            {
                // ✅ Get scoped services like DbContext inside the scope
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var connection = context.Database.GetDbConnection();
                try
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open(); // Keep connection open throughout

                    foreach (var batchRecords in RequestIds.Chunk(5000))
                    {
                        var resultInChunks = await Task.Run(() =>
                        {
                            if (ClaimValue == 1)
                            {
                                if (dTO.ApplyForId == 1)
                                {
                                    return (from record in batchRecords
                                            join RequestIdMatch in context.TrnICardRequest on record equals RequestIdMatch.RequestId into RequestIdJoin
                                            from RequestIdExists in RequestIdJoin.DefaultIfEmpty()
                                            join stepStatusMatch in context.TrnStepCounter on new { RequestId = RequestIdExists?.RequestId ?? 0, StepId } equals new { stepStatusMatch.RequestId, stepStatusMatch.StepId } into stepStatusJoin
                                            from stepStatusExists in stepStatusJoin.DefaultIfEmpty()
                                            select new DTOCardDispatchCheckRequest
                                            {
                                                ChipNo = RequestIdExists?.ChipNo ?? string.Empty,
                                                RequestId = RequestIdExists?.RequestId ?? 0,
                                                IsValid = RequestIdExists != null && RequestIdExists.RecordOfficeId == dTO.RecordOfficeId && stepStatusExists != null,
                                                Status = RequestIdExists != null && RequestIdExists.RecordOfficeId == dTO.RecordOfficeId && stepStatusExists != null ? "Valid" : "DbInvalid",
                                                Remarks = (RequestIdExists == null ? "Appl number not exists; " : "") +
                                                          (RequestIdExists != null && RequestIdExists.RecordOfficeId != dTO.RecordOfficeId ? "Appl number not Valid match to RecordOffice; " : "") +
                                                          (RequestIdExists != null && stepStatusExists == null ? Remarks : "")
                                            }).ToList();
                                }
                                else
                                {
                                    return (from record in batchRecords
                                            join RequestIdMatch in context.TrnICardRequest on record equals RequestIdMatch.RequestId into RequestIdJoin
                                            from RequestIdExists in RequestIdJoin.DefaultIfEmpty()
                                            join bdMatch in context.BasicDetails on new { BasicDetailId = RequestIdExists?.BasicDetailId ?? 0, RegimentalId = dTO.RegId } equals new { bdMatch.BasicDetailId, bdMatch.RegimentalId } into bdMatchJoin
                                            from bdMatchExists in bdMatchJoin.DefaultIfEmpty()
                                            join stepStatusMatch in context.TrnStepCounter on new { RequestId = RequestIdExists?.RequestId ?? 0, StepId } equals new { stepStatusMatch.RequestId, stepStatusMatch.StepId } into stepStatusJoin
                                            from stepStatusExists in stepStatusJoin.DefaultIfEmpty()
                                            select new DTOCardDispatchCheckRequest
                                            {
                                                ChipNo = RequestIdExists?.ChipNo ?? string.Empty,
                                                RequestId = RequestIdExists?.RequestId ?? 0,
                                                IsValid = RequestIdExists != null && bdMatchExists != null && stepStatusExists != null,
                                                Status = RequestIdExists != null && bdMatchExists != null && stepStatusExists != null ? "Valid" : "DbInvalid",
                                                Remarks = (RequestIdExists == null ? "Appl number not exists; " : "") +
                                                          (RequestIdExists != null && bdMatchExists == null ? "Appl number not Valid match to Regiment; " : "") +
                                                          (RequestIdExists != null && stepStatusExists == null ? Remarks : "")
                                            }).ToList();
                                }
                            }
                            else
                            {
                                return (from record in batchRecords
                                        join RequestIdMatch in context.TrnICardRequest on record equals RequestIdMatch.RequestId into RequestIdJoin
                                        from RequestIdExists in RequestIdJoin.DefaultIfEmpty()
                                        join bdMatch in context.BasicDetails on new { BasicDetailId = RequestIdExists?.BasicDetailId ?? 0, UnitId = dTO.ToUnitId } equals new { bdMatch.BasicDetailId, bdMatch.UnitId } into bdMatchJoin
                                        from bdMatchExists in bdMatchJoin.DefaultIfEmpty()
                                        join stepStatusMatch in context.TrnStepCounter on new { RequestId = RequestIdExists?.RequestId ?? 0, StepId } equals new { stepStatusMatch.RequestId, stepStatusMatch.StepId } into stepStatusJoin
                                        from stepStatusExists in stepStatusJoin.DefaultIfEmpty()
                                        select new DTOCardDispatchCheckRequest
                                        {
                                            ChipNo = RequestIdExists?.ChipNo ?? string.Empty,
                                            RequestId = RequestIdExists?.RequestId ?? 0,
                                            IsValid = RequestIdExists != null && bdMatchExists != null && stepStatusExists != null,
                                            Status = RequestIdExists != null && bdMatchExists != null && stepStatusExists != null ? "Valid" : "DbInvalid",
                                            Remarks = (RequestIdExists == null ? "Appl number not exists; " : "") +
                                                      (RequestIdExists != null && bdMatchExists == null ? "Appl number not Valid match to Unit; " : "") +
                                                      (RequestIdExists != null && stepStatusExists == null ? Remarks : "")
                                        }).ToList();
                            }

                        });
                        response.AddRange(resultInChunks);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return response;
        }


        /// <summary>
        /// Retrieves the user details including UserId, ArmyNo, Name, and RankAbbreviation based on the provided AspNetUsersId.
        /// </summary>
        /// <param name="AspNetUsersId">The ID of the user from the AspNetUsers table to fetch details for.</param>
        /// <returns>A DTOGenericResponse containing the user details or an error message.</returns>
        public async Task<DTOGenericResponse<DTODispatchToResponse?>> GetUserIdWithName(int AspNetUsersId) 
        {
            DTODispatchToResponse? ret = new DTODispatchToResponse();
            DTOGenericResponse<DTODispatchToResponse?> response = new DTOGenericResponse<DTODispatchToResponse?>();
            string query = @"Select up.UserId,up.ArmyNo,up.Name,mran.RankAbbreviation from AspNetUsers aspuser
                            inner join TrnDomainMapping tdm on aspuser.Id = tdm.AspNetUsersId
                            inner join UserProfile up on tdm.UserId=up.UserId
                            inner join MRank mran on up.RankId=mran.RankId
                            Where aspuser.Id=@AspNetUsersId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    ret = await connection.QueryFirstOrDefaultAsync<DTODispatchToResponse>(query, new { AspNetUsersId });
                }
                if (ret != null && ret.UserId != null)
                {
                    response.Result = true;  // Operation successful
                    response.Message = "Data retrieved successfully.";
                    response.Value = ret;
                }
                else
                {
                    response.Message = "User not found.";
                    response.Result = false; // Operation failed
                    response.Value = ret;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetUserIdWithName");
                response.Result = false;
                response.Message = "An error occurred while fetching data.";
                response.Value = ret;
                return response;
            }
        }


        /// <summary>
        /// Retrieves dispatch data based on the provided category ID and ID. The method performs different queries based on the CategoryId.
        /// </summary>
        /// <param name="CategeryId">The category ID used to determine which query to execute (1 for ORO Mapping, 2 for Regimental).</param>
        /// <param name="Id">The ID used to filter the records (RecordOfficeId for CategoryId=1, RegId for CategoryId=2).</param>
        /// <returns>A DTOGenericResponse containing the dispatch data or an error message.</returns>
        public async Task<DTOGenericResponse<DTODispatchToResponse?>> GetDispatchToData(byte CategeryId, int Id)
        {
            DTODispatchToResponse? ret = new DTODispatchToResponse();
            DTOGenericResponse<DTODispatchToResponse?> response = new DTOGenericResponse<DTODispatchToResponse?>();
            string query = string.Empty;
            if (CategeryId == 1)
            {
                query = @"Select oro.UnitId,mun.Abbreviation as UnitAbbreviation, CONCAT(mun.Sus_no,mun.Suffix) as Sus_no,tdm.UserId,tdm.AspNetUsersId,aspuser.DomainId,up.ArmyNo,up.Name,mran.RankAbbreviation from OROMapping oro
                            left join MapUnit mapu on oro.UnitId = mapu.UnitMapId
                            left join MUnit mun on mapu.UnitId = mun.UnitId
                            left join TrnDomainMapping tdm on oro.TDMId = tdm.Id
                            left join AspNetUsers aspuser on tdm.AspNetUsersId=aspuser.Id
                            left join UserProfile up on tdm.UserId=up.UserId
                            left join MRank mran on up.RankId=mran.RankId
                            Where oro.RecordOfficeId=@Id";
                try
                {
                    using (var connection = _contextDP.CreateConnection())
                    {
                        ret = await connection.QueryFirstOrDefaultAsync<DTODispatchToResponse>(query, new { Id });
                    }
                    if (ret != null && ret.UnitId != null && ret.UserId != null && ret.AspNetUsersId != null)
                    {
                        response.Result = true;  // Operation successful
                        response.Message = "Data retrieved successfully.";
                        response.Value = ret;
                    }
                    else
                    {
                        response.Message = "Unit not bind with ORO Mapping Master.Contact to MP6 Cell.";
                        response.Result = false; // Operation failed
                        response.Value = ret;
                    }
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, "BasicDetailDB->GetUnitIdAndTDMIdForDispatch");
                    response.Result = false;
                    response.Message = "An error occurred while fetching data.";
                    response.Value = ret;
                    return response;
                }
            }
            else if (CategeryId == 2)
            {
                query = @"Select UnitId from MRegimental Where RegId=@Id";
                string query2 = @"Select AspNetUsersId from TrnDomainMapping where UnitId=@UnitId";
                string query3 = @"Select mreg.UnitId,mun.Abbreviation as UnitAbbreviation, CONCAT(mun.Sus_no,mun.Suffix) as Sus_no,tdm.UserId,tdm.AspNetUsersId,aspuser.DomainId,up.ArmyNo,up.Name,mran.RankAbbreviation from MRegimental mreg
                                left join MapUnit mapu on mreg.UnitId = mapu.UnitMapId
                                left join MUnit mun on mapu.UnitId = mun.UnitId
                                left join AspNetUsers aspuser on aspuser.Id=@AspNetUsersId
                                left join TrnDomainMapping tdm on aspuser.Id = tdm.AspNetUsersId
                                left join UserProfile up on tdm.UserId=up.UserId
                                left join MRank mran on up.RankId=mran.RankId
                                Where mreg.RegId=@Id";
                try
                {
                    using (var connection = _contextDP.CreateConnection())
                    {
                        int? UnitId = await connection.QueryFirstOrDefaultAsync<int?>(query, new { Id });
                        bool found = false;
                        if (UnitId == null)
                        {
                            response.Result = false; // Operation failed
                            response.Message = "Unit not bind with Regimental Master. Contact to MP6 Cell";
                            response.Value = ret;
                            
                        }
                        else
                        {
                            var AspNetUsersIds = await connection.QueryAsync<int>(query2, new { UnitId });
                            int? AspNetUsersId=null;
                            if (AspNetUsersIds.Any())
                            {
                                foreach (var item in AspNetUsersIds)
                                {
                                    var user = await userManager.FindByIdAsync(item.ToString());
                                    
                                    if (user == null) continue;
                                    
                                    var UserClaims = await userManager.GetClaimsAsync(user);
                                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                                    {
                                        AspNetUsersId = item;
                                        found = true;
                                        break;
                                    }
                                }
                                if(found == true && AspNetUsersId !=null)
                                {
                                    ret = await connection.QueryFirstOrDefaultAsync<DTODispatchToResponse>(query3, new { Id, UnitId, AspNetUsersId });
                                    if (ret != null && ret.UnitId != null && ret.UserId != null && ret.AspNetUsersId != null)
                                    {
                                        response.Result = true;  // Operation successful
                                        response.Message = "Data retrieved successfully.";
                                        response.Value = ret;
                                    }
                                    else
                                    {
                                        response.Message = "Profile not bind with DID.Please Contact to MP6 Cell.";
                                        response.Result = false; // Operation failed
                                        response.Value = ret;
                                    }
                                }
                                else
                                {
                                    response.Result = false; // Operation failed
                                    response.Message = "Claim not assign to any DID.Please Contact to MP6 Cell.";
                                    response.Value = ret;
                                }
                            }
                            else
                            {
                                response.Result = false; // Operation failed
                                response.Message = "Unit not contain any DID.Please Contact to MP6 Cell.";
                                response.Value = ret;
                            }

                        }
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, "BasicDetailDB->GetUnitIdAndTDMIdForDispatch");
                    response.Result = false;
                    response.Message = "An error occurred while fetching data.";
                    response.Value = ret;
                    return response;
                }
            }
            else
            {
                response.Result = false; // Operation failed
                response.Message = "Invalid CategeryId provided.";
                response.Value = ret;
                return response;
            }
        }


        /// <summary>
        /// Retrieves the record office or regimental details along with unit information based on the provided parameters.
        /// </summary>
        /// <param name="ClaimValue">The claim value that determines which query to execute (2 for ORO Mapping, 3 for Regimental).</param>
        /// <param name="TDMId">The TDMId used to filter the OROMapping record (only used when ClaimValue is 2).</param>
        /// <param name="UnitId">The UnitId used to filter the MRegimental record (only used when ClaimValue is 3).</param>
        /// <param name="ToUnitId">The UnitMapId for fetching unit information (used for both ClaimValue 2 and 3).</param>
        /// <returns>A DTOGenericResponse containing the record office, regimental, and unit details, or an error message.</returns>
        public async Task<DTOGenericResponse<DTOOROWithRegimentAndUnitResponse>> GetddlRecordRegiment(byte ClaimValue,int TDMId,int UnitId,int ToUnitId)
        {
            DTOOROWithRegimentAndUnitResponse ret = new DTOOROWithRegimentAndUnitResponse();
            DTOOROWithRegimentAndUnitResponse ret2 = new DTOOROWithRegimentAndUnitResponse();
            DTOGenericResponse<DTOOROWithRegimentAndUnitResponse> response = new DTOGenericResponse<DTOOROWithRegimentAndUnitResponse>();
            string query=string.Empty;
            string query2 = string.Empty;
            if (ClaimValue == 2) 
            {
                query = @"Select TOP 1 oro.RecordOfficeId as Id,mrec.Name from OROMapping oro
                        inner join MRecordOffice mrec on oro.RecordOfficeId = mrec.RecordOfficeId WHERE oro.TDMId=@TDMId";
            }
            else if (ClaimValue == 3)
            {
                query = @"Select TOP 1 RegId as Id, Name  from MRegimental WHERE UnitId=@UnitId";
            }
                query2 = @"Select CONCAT(mu.Sus_no,mu.Suffix) as SUSNo,mu.Abbreviation as UnitAbbreviation from MUnit mu
                            INNER JOIN MapUnit munit on mu.UnitId=munit.UnitId
                            WHERE munit.UnitMapId=@ToUnitId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                     ret = await connection.QueryFirstOrDefaultAsync<DTOOROWithRegimentAndUnitResponse>(query,new { TDMId , UnitId });
                     ret2 = await connection.QueryFirstOrDefaultAsync<DTOOROWithRegimentAndUnitResponse>(query2, new { ToUnitId });
                }
                if (ret == null)
                {
                    response.Result = false; 
                    response.Message = "No records found.";
                    response.Value = new DTOOROWithRegimentAndUnitResponse();
                }
                else
                {
                    response.Result = true;  // Operation successful
                    response.Message = "Data retrieved successfully.";
                    if (ret2 !=null)
                    {
                        ret.SUSNo= ret2.SUSNo;
                        ret.UnitAbbreviation = ret2.UnitAbbreviation;
                    }
                    response.Value = ret;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetddlRecordRegiment");
                response.Result = false;
                response.Message = "An error occurred while fetching data.";
                response.Value = ret;
            }
            return response;
        }


        /// <summary>
        /// Retrieves the Record Office ID based on the provided ApplyForId, ServiceNo, ArmedId, RankId, and additional conditions from the DTO.
        /// </summary>
        /// <param name="ApplyForId">The ID indicating the type of application (e.g., 1 for military applications).</param>
        /// <param name="ServiceNo">The service number used for querying specific records.</param>
        /// <param name="ArmedId">The ID representing the armed forces type.</param>
        /// <param name="RankId">The ID representing the rank of the individual.</param>
        /// <param name="dTOApplFwdCondition">The DTO containing conditions for forwarding the application, including military prefix and rank info.</param>
        /// <returns>The Record Office ID as a byte if found, otherwise null.</returns>
        public async Task<byte?> GetRecordOfficeId(byte ApplyForId,string ServiceNo,byte ArmedId,short RankId, DTOApplFwdConditionRequest dTOApplFwdCondition)
        {
            try
            {
                string subquery = "";
                string finalquery = "";
                byte? RecordOfficeId;
                using (var connection = _contextDP.CreateConnection())
                {
                    if (ApplyForId == 1)
                    {
                        string ini = ServiceNo.Substring(0, 2).ToUpper();
                        string MP6F = dTOApplFwdCondition.MP6F.Name;
                        string MPRSO = dTOApplFwdCondition.MPRSO.Name;
                        var ArmedAbbreviation = dTOApplFwdCondition.MPRSO.ArmedAbbreviation;

                        subquery = @"declare @Orderby tinyint=0
                                    declare @ArmedAbbreviation varchar(10)=''

                                    Select @Orderby=Orderby from MRank where RankId=@RankId
                                    Select @ArmedAbbreviation=Abbreviation from MArmedType where ArmedId=@ArmedId

                                    Select @Orderby Orderby,@ArmedAbbreviation ArmedAbbreviation";

                        var subqueryResult = await connection.QuerySingleOrDefaultAsync<DTOFwdSubqueryResponse>(subquery, new { RankId, ArmedId });

                        if (subqueryResult != null)
                        {
                            if (ArmedAbbreviation.Contains(subqueryResult.ArmedAbbreviation, StringComparer.OrdinalIgnoreCase))
                            {
                                finalquery = @"Select RecordOfficeId from MRecordOffice where Name=@MPRSO";
                                RecordOfficeId = await connection.QueryFirstAsync<byte>(finalquery, new { MPRSO });

                                if (RecordOfficeId != null)
                                {
                                    return RecordOfficeId;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else if (ini == dTOApplFwdCondition.MP6F.ArmyNoPrefix)
                            {
                                finalquery = @"Select RecordOfficeId from MRecordOffice where Name=@MP6F";
                                RecordOfficeId = await connection.QueryFirstAsync<byte>(finalquery, new { MP6F });

                                if (RecordOfficeId != null)
                                {
                                    return RecordOfficeId;
                                }
                                else
                                {
                                    return null;
                                }

                            }
                            else if (subqueryResult.Orderby <= dTOApplFwdCondition.MP6A.RankOrderby)
                            {
                                finalquery = @"Select RecordOfficeId from OROMapping where RankId is not null";
                                RecordOfficeId = await connection.QueryFirstAsync<byte>(finalquery);

                                if (RecordOfficeId != null)
                                {
                                    return RecordOfficeId;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                finalquery = @"Select RecordOfficeId from OROMapping where @ArmedId in (select value from string_split(ArmedIdList,','))";
                                RecordOfficeId = await connection.QueryFirstAsync<byte>(finalquery, new { ArmedId });

                                if (RecordOfficeId != null)
                                {
                                    return RecordOfficeId;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            return null;
                        }

                    }
                    else
                    {
                        finalquery = @"Select RecordOfficeId from MRecordOffice where ArmedId=@ArmedId";
                        RecordOfficeId = await connection.QueryFirstAsync<byte>(finalquery, new { ArmedId });

                        if (RecordOfficeId != null)
                        {
                            return RecordOfficeId;
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetRecordOfficeId");
                return null;
            }

        }


        /// <summary>
        /// Checks whether the given Army No (Service No) exists in the BasicDetails table.
        /// </summary>
        /// <param name="ArmyNo">The Army No (Service No) to be checked for existence in the database.</param>
        /// <returns>True if the Army No exists in the BasicDetails table, otherwise false.</returns>
        public async Task<bool> CheckArmyNO(string ArmyNo)
        {
            return await _context.BasicDetails.AnyAsync(x => x.ServiceNo == ArmyNo);
        }


        /// <summary>
        /// Uploads Chip Numbers and Card Serial Numbers for a list of records, updating the corresponding fields in the TrnICardRequest table.
        /// The operation is wrapped in a transaction to ensure data integrity.
        /// </summary>
        /// <param name="Data">A list of DTOUploadChipAndSerialRequest objects containing the RequestId, CardSerialNo, and ChipNo to be updated.</param>
        /// <returns>A DTOUploadChipAndSerialResponse indicating whether the operation was successful or not, along with a message.</returns>
        public async Task<DTOUploadChipAndSerialResponse> UploadChipAndSerial(List<DTOUploadChipAndSerialRequest> Data)
        {
            int i = 0;
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();
            try
            {
                foreach (var item in Data)
                {
                    if (item.IsValid == true)
                    {
                        string query = " UPDATE TrnICardRequest set CardSerialNo=@CardSerialNo, ChipNo=@ChipNo where RequestId=@RequestId ";

                        var parameters = new DynamicParameters();
                        parameters.Add("@RequestId", item.RequestId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@CardSerialNo", item.CardSerialNo, DbType.String, ParameterDirection.Input, 30);
                        parameters.Add("@ChipNo", item.ChipNo, DbType.String, ParameterDirection.Input, 30);

                        await db.ExecuteAsync(query, parameters, transaction: transaction);
                    }
                }
                // Commit the transaction if all operations succeed
                transaction.Commit();
                response.Result = true;
                response.Message = "Data processed successfully!";
                return response;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "BasicDetailDB->UploadChipAndSerial");
                response.Result = false;
                response.Message = ex.Message;
                return response;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }


        /// <summary>
        /// Retrieves the top 5 records from the TrnICardRequest table where the ArmyNo matches the ServiceNo in the BasicDetails table, 
        /// and the request status is active (StatusId == 1).
        /// </summary>
        /// <param name="ArmyNo">The Army No (Service No) to search for in the BasicDetails table.</param>
        /// <returns>A list of DTOTopArmyNoFromICardRequestResponse objects representing the top matching records, or null if an error occurs.</returns>
        public async Task<List<DTOTopArmyNoFromICardRequestResponse>?> GetTopArmyNoFromICardRequest(string ArmyNo)
        {
            try
            {
                var ret = await (from bd in _context.BasicDetails
                                 join irequest in _context.TrnICardRequest on bd.BasicDetailId equals irequest.BasicDetailId
                                 where bd.ServiceNo.Contains(ArmyNo) && irequest.StatusId == 1
                                 select new DTOTopArmyNoFromICardRequestResponse
                                 {
                                     RequestId = irequest.RequestId,
                                     ServiceNo = bd.ServiceNo,
                                 }
                                ).Take(5).ToListAsync();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetTopArmyNoFromICardRequest");
                return null;
            }

        }

        /// <summary>
        /// Retrieves basic details (Rank, First Name, Last Name, and Unit Name) by the provided RequestId from the TrnICardRequest and related tables.
        /// </summary>
        /// <param name="RequestId">The RequestId used to filter and fetch the corresponding record from the TrnICardRequest table.</param>
        /// <returns>A DTOBDetailByRequestIdResponse object containing the rank, first name, last name, and unit name, or null if not found or an error occurs.</returns>
        public async Task<DTOBDetailByRequestIdResponse?> GetBDetailByRequestId(int RequestId)
        {
            try
            {
                var ret = await (from irequest in _context.TrnICardRequest
                                 join bd in _context.BasicDetails on irequest.BasicDetailId equals bd.BasicDetailId
                                 join rk in _context.MRank on bd.RankId equals rk.RankId
                                 join umap in _context.MapUnit on bd.UnitId equals umap.UnitMapId
                                 join munit in _context.MUnit on umap.UnitId equals munit.UnitId
                                 where irequest.RequestId == RequestId
                                 select new DTOBDetailByRequestIdResponse
                                 {
                                     RankName = rk.RankAbbreviation,
                                     FName = bd.FName,
                                     LName = bd.LName,
                                     UnitName = munit.UnitName,
                                 }
                                ).FirstOrDefaultAsync();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailByRequestId");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a paginated list of ICard request hold details based on the provided filter and sorting criteria.
        /// </summary>
        /// <param name="dTO">The data transfer object containing sorting, pagination, and search criteria.</param>
        /// <returns>A DTODataTablesResponse containing a paginated list of ICard request hold details.</returns>
        public async Task<DTODataTablesResponse<DTOICardRequestHoldResponse>> GetAllICardRequestHold(DTODataTablesRequest dTO)
        {
            #region Old Code
            //string query = "";
            //query = " SELECT munit.UnitName,B.FName,B.LName,B.ServiceNo,trnicrd.RequestId,Afor.Name ApplyFor,ran.RankAbbreviation RankName,thold.ICardHoldId,thold.HoldReason,thold.UnHoldReason,thold.IsHold,u.DomainId,u.UpdatedOn " +
            //        " FROM MTrnICardHold thold " +
            //        " inner join AspNetUsers u on u.Id = thold.Updatedby " +
            //        " inner join TrnICardRequest trnicrd on trnicrd.RequestId = thold.RequestId " +
            //        " inner join BasicDetails B on B.BasicDetailId = trnicrd.BasicDetailId " +
            //        " inner join MRank ran on ran.RankId=B.RankId " +
            //        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
            //        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
            //        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId ";
            //try
            //{
            //    using (var connection = _contextDP.CreateConnection())
            //    {
            //        var allrecord = await connection.QueryAsync<DTOICardRequestHoldResponse>(query);
            //        return await Task.FromResult(allrecord.ToList());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(1001, ex, "BasicDetailDB->GetAllICardRequestHold");
            //    return null;
            //}
            #endregion
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();
            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["DispatchCardId"] = "RequestId",
                ["ApplyFor"] = "ApplyFor",
            };

            var sortOrder = dTO.sortDirection;
            selectFields = @"munit.UnitName,B.FName,B.LName,B.ServiceNo,trnicrd.RequestId,Afor.Name ApplyFor,ran.RankAbbreviation RankName,thold.ICardHoldId,thold.HoldReason,thold.UnHoldReason,thold.IsHold,u.DomainId,u.UpdatedOn";
            fromJoinClause = @"FROM MTrnICardHold thold
                                inner join AspNetUsers u on u.Id = thold.Updatedby
                                inner join TrnICardRequest trnicrd on trnicrd.RequestId = thold.RequestId
                                inner join BasicDetails B on B.BasicDetailId = trnicrd.BasicDetailId
                                inner join MRank ran on ran.RankId=B.RankId
                                inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId
                                inner join MUnit munit on munit.UnitId=mapunit.UnitId
                                inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId";
            whereClause = @"WHERE
                                trnicrd.RequestId LIKE '%' + @SearchTerm + '%' OR
                                B.ServiceNo LIKE '%' + @SearchTerm + '%'
                                ";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "thold.ICardHoldId";
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
                    var records = (await ret.ReadAsync<DTOICardRequestHoldResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTOICardRequestHoldResponse>
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
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllDispatchCard");
                List<DTOICardRequestHoldResponse> dTODispatchCardLists = new List<DTOICardRequestHoldResponse>();
                var responseData = new DTODataTablesResponse<DTOICardRequestHoldResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTODispatchCardLists
                };
                return responseData;
            }
        }

        /// <summary>
        /// Saves or updates the basic details, address, identity information, upload data, ICard request, and step counter 
        /// in the database, depending on whether the provided BasicDetailId is zero or an existing record.
        /// </summary>
        /// <param name="Data">The basic detail data to be saved or updated.</param>
        /// <param name="address">The address data associated with the basic details.</param>
        /// <param name="trnUpload">The upload data (e.g., signature and photo paths) associated with the basic details.</param>
        /// <param name="mTrnIdentityInfo">The identity information for the individual associated with the basic details.</param>
        /// <param name="mTrnICardRequest">The ICard request data to be saved or updated.</param>
        /// <param name="mStepCounter">The step counter data related to the ICard request.</param>
        /// <returns>A DTOBasicDetailsSaveResponse object indicating the success or failure of the operation, along with a message.</returns>
        public async Task<DTOBasicDetailsSaveResponse> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address, MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter)
        {
            bool EFCoreOrDapper = true; // true mean EFCore
            using var transaction_ = _context.Database.BeginTransaction();
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            DTOBasicDetailsSaveResponse dTOBasicDetailsSaveResponse = new DTOBasicDetailsSaveResponse();
            if (EFCoreOrDapper)
            {
                try
                {
                    if (Data.BasicDetailId == 0)
                    {
                        _context.BasicDetails.Add(Data);
                        await _context.SaveChangesAsync();
                        int BasicDetailId = Data.BasicDetailId;
                        address.BasicDetailId = BasicDetailId;
                        _context.TrnAddress.Add(address);
                        await _context.SaveChangesAsync();
                        trnUpload.BasicDetailId = BasicDetailId;
                        _context.TrnUpload.Add(trnUpload);
                        await _context.SaveChangesAsync();
                        mTrnIdentityInfo.BasicDetailId = BasicDetailId;
                        _context.TrnIdentityInfo.Add(mTrnIdentityInfo);
                        await _context.SaveChangesAsync();
                        mTrnICardRequest.BasicDetailId = BasicDetailId;
                        _context.TrnICardRequest.Add(mTrnICardRequest);
                        await _context.SaveChangesAsync();
                        mStepCounter.RequestId = mTrnICardRequest.RequestId;
                        _context.TrnStepCounter.Add(mStepCounter);

                        await _context.SaveChangesAsync();

                        transaction_.Commit();
                        dTOBasicDetailsSaveResponse.Result = true;
                        dTOBasicDetailsSaveResponse.Message = "Save";
                        return dTOBasicDetailsSaveResponse;
                    }
                    else
                    {

                        address.BasicDetailId = Data.BasicDetailId;
                        trnUpload.BasicDetailId = Data.BasicDetailId;
                        mTrnIdentityInfo.BasicDetailId = Data.BasicDetailId;

                        _context.Update(address);
                        await _context.SaveChangesAsync();
                        _context.Update(trnUpload);
                        await _context.SaveChangesAsync();
                        _context.Update(mTrnIdentityInfo);
                        await _context.SaveChangesAsync();
                        _context.Update(mTrnICardRequest);
                        await _context.SaveChangesAsync();

                        _context.Entry(Data).State = EntityState.Modified;
                        _context.Update(Data);
                        await _context.SaveChangesAsync();

                        transaction_.Commit();
                        dTOBasicDetailsSaveResponse.Result = true;
                        dTOBasicDetailsSaveResponse.Message = "Update";
                        return dTOBasicDetailsSaveResponse;
                    }
                    //do other things, then commit or rollback


                }
                catch (ReferenceConstraintException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1001, ex, "ReferenceConstraintException");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;

                }
                catch (UniqueConstraintException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1002, ex, "UniqueConstraintException");
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.Message.Contains("IX_AadhaarNo"))
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = "The provided Aadhaar number already exists. Please check and try again.";
                            return dTOBasicDetailsSaveResponse;
                        }
                        else if (ex.InnerException.Message.Contains("IX_PaperIcardNo"))
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = "The provided PaperIcardNo number already exists. Please check and try again.";
                            return dTOBasicDetailsSaveResponse;
                        }
                        else
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = ex.Message;
                            return dTOBasicDetailsSaveResponse;
                        }
                    }
                    else
                    {
                        dTOBasicDetailsSaveResponse.Result = false;
                        dTOBasicDetailsSaveResponse.Message = ex.Message;
                        return dTOBasicDetailsSaveResponse;

                    }


                }
                catch (MaxLengthExceededException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1003, ex, "MaxLengthExceededException");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
                catch (CannotInsertNullException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1004, ex, "CannotInsertNullException");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
                catch (NumericOverflowException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1005, ex, "NumericOverflowException");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
                catch (Exception ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1006, ex, "Exception");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
            }
            else
            {
                try
                {
                    if (Data.BasicDetailId == 0)
                    {
                        var insertBasicDetail = " INSERT INTO BasicDetails (ArmedId, RankId, ServiceNo, DOB, PlaceOfIssue, DateOfIssue, DateOfCommissioning, ApplyForId, UnitId, PaperIcardNo,IsActive, Updatedby, UpdatedOn, IssuingAuthorityId, NameAsPerRecord, RegimentalId, FName, LName, PreviousBasicDetailId)" +
                                                " OUTPUT INSERTED.BasicDetailId " +
                                                " VALUES (@ArmedId, @RankId, @ServiceNo, @DOB, @PlaceOfIssue, @DateOfIssue, @DateOfCommissioning, @ApplyForId, @UnitId, @PaperIcardNo, @IsActive, @Updatedby, @UpdatedOn, @IssuingAuthorityId, @NameAsPerRecord, @RegimentalId, @FName, @LName, @PreviousBasicDetailId);";
                        var parametersBD = new DynamicParameters();
                        //parametersBD.Add("@BasicDetailId", Data.BasicDetailId, DbType.Int32, ParameterDirection.Output);
                        parametersBD.Add("@ArmedId", Data.ArmedId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@RankId", Data.RankId, DbType.Int16, ParameterDirection.Input);
                        parametersBD.Add("@ServiceNo", Data.ServiceNo, DbType.String, ParameterDirection.Input, 10);
                        parametersBD.Add("@DOB", Data.DOB, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@PlaceOfIssue", Data.PlaceOfIssue, DbType.String, ParameterDirection.Input, 50);
                        parametersBD.Add("@DateOfIssue", Data.DateOfIssue, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@DateOfCommissioning", Data.DateOfCommissioning, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@ApplyForId", Data.ApplyForId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@UnitId", Data.UnitId, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@PaperIcardNo", Data.PaperIcardNo, DbType.String, ParameterDirection.Input, 12);
                        parametersBD.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersBD.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@IssuingAuthorityId", Data.IssuingAuthorityId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@NameAsPerRecord", Data.NameAsPerRecord, DbType.AnsiString, ParameterDirection.Input, 36);
                        parametersBD.Add("@RegimentalId", Data.RegimentalId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@FName", Data.FName, DbType.AnsiString, ParameterDirection.Input, 18);
                        parametersBD.Add("@LName", Data.LName, DbType.AnsiString, ParameterDirection.Input, 18);
                        parametersBD.Add("@PreviousBasicDetailId", Data.PreviousBasicDetailId, DbType.Int32, ParameterDirection.Input);
                        int BasicDetailId = await db.QuerySingleAsync<int>(insertBasicDetail, parametersBD, transaction: transaction);

                        address.BasicDetailId = BasicDetailId;

                        var insertAddress = " INSERT INTO TrnAddress (BasicDetailId, State, District, PS, PO, Tehsil, Village, PinCode)" +
                                            " VALUES (@BasicDetailId, @State, @District, @PS, @PO, @Tehsil, @Village, @PinCode);";
                        var parametersAddr = new DynamicParameters();
                        //parametersAddr.Add("@AddressId", address.AddressId, DbType.Int32, ParameterDirection.Input);
                        parametersAddr.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersAddr.Add("@State", address.State, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@District", address.District, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PS", address.PS, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PO", address.PO, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@Tehsil", address.Tehsil, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@Village", address.Village, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PinCode", address.PinCode, DbType.Int32, ParameterDirection.Input);
                        await db.ExecuteAsync(insertAddress, parametersAddr, transaction: transaction);

                        trnUpload.BasicDetailId = BasicDetailId;

                        var insertTrnUpload = " INSERT INTO TrnUpload (BasicDetailId, SignatureImagePath, PhotoImagePath)" +
                                              " VALUES (@BasicDetailId, @SignatureImagePath, @PhotoImagePath);";
                        var parametersUpload = new DynamicParameters();
                        //parametersUpload.Add("@UploadId", address.AddressId, DbType.Int32, ParameterDirection.Input);
                        parametersUpload.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersUpload.Add("@SignatureImagePath", trnUpload.SignatureImagePath, DbType.String, ParameterDirection.Input, 100);
                        parametersUpload.Add("@PhotoImagePath", trnUpload.PhotoImagePath, DbType.String, ParameterDirection.Input, 100);
                        await db.ExecuteAsync(insertTrnUpload, parametersUpload, transaction: transaction);

                        mTrnIdentityInfo.BasicDetailId = BasicDetailId;

                        var insertIdentityInfo = " INSERT INTO TrnIdentityInfo (BasicDetailId, IdenMark1, IdenMark2, AadhaarNo, Height, BloodGroupId)" +
                                                 " VALUES (@BasicDetailId, @IdenMark1, @IdenMark2, @AadhaarNo, @Height, @BloodGroupId);";
                        var parametersIdentityInfo = new DynamicParameters();
                        //parametersIdentityInfo.Add("@InfoId", mTrnIdentityInfo.InfoId, DbType.Int32, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@BasicDetailId", mTrnIdentityInfo.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@IdenMark1", mTrnIdentityInfo.IdenMark1, DbType.String, ParameterDirection.Input, 200);
                        parametersIdentityInfo.Add("@IdenMark2", mTrnIdentityInfo.IdenMark2, DbType.String, ParameterDirection.Input, 200);
                        parametersIdentityInfo.Add("@AadhaarNo", mTrnIdentityInfo.AadhaarNo, DbType.Int64, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@Height", mTrnIdentityInfo.Height, DbType.Single, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@BloodGroupId", mTrnIdentityInfo.BloodGroupId, DbType.Byte, ParameterDirection.Input);
                        await db.ExecuteAsync(insertIdentityInfo, parametersIdentityInfo, transaction: transaction);

                        mTrnICardRequest.BasicDetailId = BasicDetailId;

                        var insertTrnICardRequest = " INSERT INTO TrnICardRequest (BasicDetailId, TypeId, RegistrationId, TrnDomainMappingId, IsActive, Updatedby, UpdatedOn, StatusId, CardSerialNo, ChipNo)" +
                                                    " OUTPUT INSERTED.RequestId " +
                                                    " VALUES (@BasicDetailId, @TypeId, @RegistrationId, @TrnDomainMappingId, @IsActive, @Updatedby, @UpdatedOn, @StatusId, @CardSerialNo, @ChipNo);";
                        var parametersTrnICardRequest = new DynamicParameters();
                        //parametersTrnICardRequest.Add("@RequestId", mTrnICardRequest.RequestId, DbType.Int32, ParameterDirection.Output);
                        parametersTrnICardRequest.Add("@BasicDetailId", mTrnICardRequest.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TypeId", mTrnICardRequest.TypeId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@RegistrationId", mTrnICardRequest.RegistrationId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TrnDomainMappingId", mTrnICardRequest.TrnDomainMappingId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@IsActive", mTrnICardRequest.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@Updatedby", mTrnICardRequest.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@UpdatedOn", mTrnICardRequest.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@StatusId", mTrnICardRequest.StatusId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@CardSerialNo", mTrnICardRequest.CardSerialNo, DbType.String, ParameterDirection.Input, 30);
                        parametersTrnICardRequest.Add("@ChipNo", mTrnICardRequest.ChipNo, DbType.String, ParameterDirection.Input, 30);
                        int RequestId = await db.QuerySingleAsync<int>(insertTrnICardRequest, parametersTrnICardRequest, transaction: transaction);
                        mStepCounter.RequestId = RequestId;

                        var insertTrnStepCounter = " INSERT INTO TrnStepCounter (RequestId, StepId, IsActive, Updatedby, UpdatedOn, ApplyForId)" +
                                                   " VALUES (@RequestId, @StepId, @IsActive, @Updatedby, @UpdatedOn, @ApplyForId);";
                        var parametersTrnStepCounter = new DynamicParameters();
                        //parametersTrnStepCounter.Add("@Id", mStepCounter.Id, DbType.Int32, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@RequestId", mStepCounter.RequestId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@StepId", mStepCounter.StepId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@IsActive", mStepCounter.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@Updatedby", mStepCounter.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@UpdatedOn", mStepCounter.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@ApplyForId", mStepCounter.ApplyForId, DbType.Byte, ParameterDirection.Input);
                        await db.ExecuteAsync(insertTrnStepCounter, parametersTrnStepCounter, transaction: transaction);

                        transaction.Commit();
                        dTOBasicDetailsSaveResponse.Result = true;
                        dTOBasicDetailsSaveResponse.Message = "Save";
                        return dTOBasicDetailsSaveResponse;
                    }
                    else
                    {
                        address.BasicDetailId = Data.BasicDetailId;
                        trnUpload.BasicDetailId = Data.BasicDetailId;
                        mTrnIdentityInfo.BasicDetailId = Data.BasicDetailId;

                        var updateBasicDetail = " UPDATE BasicDetails SET ArmedId=@ArmedId, RankId=@RankId, ServiceNo=@ServiceNo, DOB=@DOB, PlaceOfIssue=@PlaceOfIssue, DateOfIssue=@DateOfIssue, DateOfCommissioning=@DateOfCommissioning, ApplyForId=@ApplyForId, UnitId=@UnitId, PaperIcardNo=@PaperIcardNo,IsActive=@IsActive, Updatedby=@Updatedby, UpdatedOn=@UpdatedOn, IssuingAuthorityId=@IssuingAuthorityId, NameAsPerRecord=@NameAsPerRecord, RegimentalId=@RegimentalId, FName=@FName, LName=@LName, PreviousBasicDetailId=@PreviousBasicDetailId WHERE BasicDetailId=@BasicDetailId ";
                        var parametersBD = new DynamicParameters();
                        parametersBD.Add("@BasicDetailId", Data.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@ArmedId", Data.ArmedId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@RankId", Data.RankId, DbType.Int16, ParameterDirection.Input);
                        parametersBD.Add("@ServiceNo", Data.ServiceNo, DbType.String, ParameterDirection.Input, 10);
                        parametersBD.Add("@DOB", Data.DOB, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@PlaceOfIssue", Data.PlaceOfIssue, DbType.String, ParameterDirection.Input, 50);
                        parametersBD.Add("@DateOfIssue", Data.DateOfIssue, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@DateOfCommissioning", Data.DateOfCommissioning, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@ApplyForId", Data.ApplyForId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@UnitId", Data.UnitId, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@PaperIcardNo", Data.PaperIcardNo, DbType.String, ParameterDirection.Input, 12);
                        parametersBD.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersBD.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@IssuingAuthorityId", Data.IssuingAuthorityId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@NameAsPerRecord", Data.NameAsPerRecord, DbType.AnsiString, ParameterDirection.Input, 36);
                        parametersBD.Add("@RegimentalId", Data.RegimentalId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@FName", Data.FName, DbType.AnsiString, ParameterDirection.Input, 18);
                        parametersBD.Add("@LName", Data.LName, DbType.AnsiString, ParameterDirection.Input, 18);
                        parametersBD.Add("@PreviousBasicDetailId", Data.PreviousBasicDetailId, DbType.Int32, ParameterDirection.Input);
                        await db.ExecuteAsync(updateBasicDetail, parametersBD, transaction: transaction);

                        var updateAddress = " UPDATE TrnAddress SET BasicDetailId=@BasicDetailId, State=@State, District=@District, PS=@PS, PO=@PO, Tehsil=@Tehsil, Village=@Village, PinCode=@PinCode WHERE AddressId=@AddressId";
                        var parametersAddr = new DynamicParameters();
                        parametersAddr.Add("@AddressId", address.AddressId, DbType.Int32, ParameterDirection.Input);
                        parametersAddr.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersAddr.Add("@State", address.State, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@District", address.District, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PS", address.PS, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PO", address.PO, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@Tehsil", address.Tehsil, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@Village", address.Village, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PinCode", address.PinCode, DbType.Int32, ParameterDirection.Input);
                        await db.ExecuteAsync(updateAddress, parametersAddr, transaction: transaction);

                        var updateTrnUpload = " UPDATE TrnUpload SET BasicDetailId=@BasicDetailId, SignatureImagePath=@SignatureImagePath, PhotoImagePath=@PhotoImagePath WHERE UploadId=@UploadId";
                        var parametersUpload = new DynamicParameters();
                        parametersUpload.Add("@UploadId", address.AddressId, DbType.Int32, ParameterDirection.Input);
                        parametersUpload.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersUpload.Add("@SignatureImagePath", trnUpload.SignatureImagePath, DbType.String, ParameterDirection.Input, 100);
                        parametersUpload.Add("@PhotoImagePath", trnUpload.PhotoImagePath, DbType.String, ParameterDirection.Input, 100);
                        await db.ExecuteAsync(updateTrnUpload, parametersUpload, transaction: transaction);

                        var updateIdentityInfo = " UPDATE TrnIdentityInfo SET BasicDetailId=@BasicDetailId, IdenMark1=@IdenMark1, IdenMark2=@IdenMark2, AadhaarNo=@AadhaarNo, Height=@Height, BloodGroupId=@BloodGroupId WHERE InfoId=@InfoId";
                        var parametersIdentityInfo = new DynamicParameters();
                        parametersIdentityInfo.Add("@InfoId", mTrnIdentityInfo.InfoId, DbType.Int32, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@BasicDetailId", mTrnIdentityInfo.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@IdenMark1", mTrnIdentityInfo.IdenMark1, DbType.String, ParameterDirection.Input, 200);
                        parametersIdentityInfo.Add("@IdenMark2", mTrnIdentityInfo.IdenMark2, DbType.String, ParameterDirection.Input, 200);
                        parametersIdentityInfo.Add("@AadhaarNo", mTrnIdentityInfo.AadhaarNo, DbType.Int64, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@Height", mTrnIdentityInfo.Height, DbType.Single, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@BloodGroupId", mTrnIdentityInfo.BloodGroupId, DbType.Byte, ParameterDirection.Input);
                        await db.ExecuteAsync(updateIdentityInfo, parametersIdentityInfo, transaction: transaction);

                        var updateTrnICardRequest = " UPDATE TrnICardRequest SET BasicDetailId=@BasicDetailId, TypeId=@TypeId, RegistrationId=@RegistrationId, TrnDomainMappingId=@TrnDomainMappingId, IsActive=@IsActive, Updatedby=@Updatedby, UpdatedOn=@UpdatedOn, StatusId=@StatusId, CardSerialNo=@CardSerialNo, ChipNo=@ChipNo,RecordOfficeId=@RecordOfficeId WHERE RequestId=@RequestId";
                        var parametersTrnICardRequest = new DynamicParameters();
                        parametersTrnICardRequest.Add("@RequestId", mTrnICardRequest.RequestId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@BasicDetailId", mTrnICardRequest.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TypeId", mTrnICardRequest.TypeId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@RegistrationId", mTrnICardRequest.RegistrationId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TrnDomainMappingId", mTrnICardRequest.TrnDomainMappingId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@IsActive", mTrnICardRequest.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@Updatedby", mTrnICardRequest.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@UpdatedOn", mTrnICardRequest.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@StatusId", mTrnICardRequest.StatusId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@CardSerialNo", mTrnICardRequest.CardSerialNo, DbType.String, ParameterDirection.Input, 30);
                        parametersTrnICardRequest.Add("@ChipNo", mTrnICardRequest.ChipNo, DbType.String, ParameterDirection.Input, 30);
                        parametersTrnICardRequest.Add("@RecordOfficeId", mTrnICardRequest.RecordOfficeId, DbType.Byte, ParameterDirection.Input);
                        await db.ExecuteAsync(updateTrnICardRequest, parametersTrnICardRequest, transaction: transaction);

                        transaction.Commit();
                        dTOBasicDetailsSaveResponse.Result = true;
                        dTOBasicDetailsSaveResponse.Message = "Updae";
                        return dTOBasicDetailsSaveResponse;
                    }
                }
                
                catch (Microsoft.Data.SqlClient.SqlException ex) // Unique constraint violation error number
                {
                    transaction.Rollback();  // Rollback the transaction
                    _logger.LogError(1006, ex, "BasicDetailDB->SaveBasicDetailsWithAll");
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        if (ex.Message.Contains("IX_AadhaarNo"))
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = "The provided Aadhaar number already exists. Please check and try again.";
                            return dTOBasicDetailsSaveResponse;
                        }
                        else if (ex.Message.Contains("IX_PaperIcardNo"))
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = "The provided PaperIcardNo number already exists. Please check and try again.";
                            return dTOBasicDetailsSaveResponse;
                        }
                        else
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = ex.Message;
                            return dTOBasicDetailsSaveResponse;
                        }
                    }
                    else
                    {
                        dTOBasicDetailsSaveResponse.Result = false;
                        dTOBasicDetailsSaveResponse.Message = ex.Message;
                        return dTOBasicDetailsSaveResponse;
                    }


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //_logger.LogError(1006, ex, "BasicDetailDB->SaveBasicDetailsWithAll");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
                finally
                {
                    // Dispose of the connection
                    db.Dispose();
                }
            }
        }


        /// <summary>
        /// Finds a BasicDetail record based on the provided ServiceNo.
        /// </summary>
        /// <param name="ServiceNo">The Service Number (ArmyNo) used to query the BasicDetails table.</param>
        /// <returns>A BasicDetail object if a matching record is found, otherwise null.</returns>
        public async Task<BasicDetail?> FindServiceNo(string ServiceNo)
        {
            string query = @"Select * from BasicDetails where ServiceNo = @ServiceNo ";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    BasicDetail? basicDetail = await connection.QuerySingleOrDefaultAsync<BasicDetail>(query, new { ServiceNo });
                    if (basicDetail != null)
                    {
                        return basicDetail;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->FindServiceNo");
                return null;
            }


        }


        /// <summary>
        /// Retrieves the maximum BasicDetailId for a given ServiceNo from the BasicDetails table.
        /// </summary>
        /// <param name="ServiceNo">The Service Number (ArmyNo) used to find the corresponding BasicDetail records.</param>
        /// <returns>The maximum BasicDetailId for the specified ServiceNo, or null if no records are found or an error occurs.</returns>
        public async Task<int?> MaxBasicDetailId(string ServiceNo)
        {
            const string query = @"SELECT MAX(BasicDetailId) AS MaxBasicDetailId FROM BasicDetails  WHERE ServiceNo = @ServiceNo";

            try
            {
                using var connection = _contextDP.CreateConnection();

                int? result = await connection.QueryFirstOrDefaultAsync<int?>(query, new { ServiceNo = ServiceNo });

                return result;
            }
            catch (Exception ex)
            {
                // Log and return null if exception occurs
                _logger.LogError(1001, ex, "BasicDetailDB->MaxBasicDetailId");
                return null;
            }
        }


        /// <summary>
        /// Searches for service numbers based on the provided criteria and returns a list of matching records.
        /// The query differs based on the request type (e.g., posting out, faulty cards, lost cards, etc.).
        /// </summary>
        /// <param name="dto">The data transfer object containing search criteria such as service number, type ID, and map unit ID.</param>
        /// <returns>A list of DTOSmartSearch objects matching the search criteria, or null if no records are found or an error occurs.</returns>
        public async Task<List<DTOSmartSearch>?> SearchAllServiceNo(DTOSearchArmyNoRequest dto)
        {
            string query = "";
            if (dto.TypeId == KeyConstants.ApplicantPostingOut || dto.TypeId == KeyConstants.ApplicantClose)
            {
                query = @"Select Distinct TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,req.CardSerialNo,req.ChipNo
                            from BasicDetails basi
                            inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId=1
                            inner join TrnDomainMapping map on map.Id = req.TrnDomainMappingId and map.UnitId=@MapUnitId
                            inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                            where ServiceNo like @ServiceNo ";
            }
            else if (dto.TypeId == KeyConstants.FaultyCardRequest)
            {
                if (dto.Claim == 1)
                {
                    query = @$"Select TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId,req.CardSerialNo,req.ChipNo
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId=1
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId=6
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                LEFT JOIN TrnFwds fwd ON fwd.RequestId = req.RequestId
                                where ServiceNo like @ServiceNo
                                Group by basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath,req.RequestId,req.CardSerialNo,req.ChipNo";
                }
                //else if (dto.Claim == 2)
                //{

                //}
                //else if (dto.Claim == 3)
                //{

                //}
                else
                {
                    query = @$"Select TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId,req.CardSerialNo,req.ChipNo
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId=1
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId=14
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId and tdm.UnitId=@MapUnitId
                                LEFT JOIN TrnFwds fwd ON fwd.RequestId = req.RequestId
                                where ServiceNo like @ServiceNo
                                Group by basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath,req.RequestId,req.CardSerialNo,req.ChipNo";
                }

            }
            else if (dto.TypeId == KeyConstants.HoltlistCardRequest)
            {
                query = @$"Select TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId,req.CardSerialNo,req.ChipNo
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId = 2
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId = 15
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                LEFT JOIN TrnFwds fwd ON fwd.RequestId = req.RequestId
                                Left join TrnHotlistCards thc on req.RequestId = thc.RequestId
                                where thc.RequestId is null and ServiceNo like @ServiceNo
                                Group by basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath,req.RequestId,req.CardSerialNo,req.ChipNo";
            }
            else if (dto.TypeId == KeyConstants.LostCardRequest)
            {
                query = @$"Select TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId,req.CardSerialNo,req.ChipNo
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId in (1,2)
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId in (6,11,12,13,14,15)
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId and tdm.UnitId=@MapUnitId
                                LEFT JOIN TrnFwds fwd ON fwd.RequestId = req.RequestId
                                Left join TrnLostCards tlc on req.RequestId = tlc.RequestId
                                Left join TrnDestructionCards tld on req.RequestId = tld.RequestId
                                where tlc.RequestId is null and tld.RequestId is null and ServiceNo like @ServiceNo
                                Group by basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath,req.RequestId,req.CardSerialNo,req.ChipNo";
            }
            else if (dto.TypeId == KeyConstants.DistributeCardRequest)
            {
                query = @$"Select TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,0 AS MaxTrnFwdId,req.CardSerialNo,req.ChipNo
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId=1
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId=14
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId and tdm.UnitId=@MapUnitId
                                Left join TrnDistributeCards tdc on req.RequestId = tdc.RequestId
                                Left join TrnHotlistCards thc on req.RequestId = thc.RequestId
                                where tdc.RequestId is null and thc.RequestId is null and ServiceNo like @ServiceNo
                                Group by basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath,req.RequestId,req.CardSerialNo,req.ChipNo";
            }
            else if (dto.TypeId == KeyConstants.DestructionCardRequest)
            {
                query = @$"Select TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId,req.CardSerialNo,req.ChipNo
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId = 2
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId in (15)
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId and tdm.UnitId=@MapUnitId
                                LEFT JOIN TrnFwds fwd ON fwd.RequestId = req.RequestId
                                Left join TrnDestructionCards tlc on req.RequestId = tlc.RequestId
                                where tlc.RequestId is null and ServiceNo like @ServiceNo
                                Group by basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath,req.RequestId,req.CardSerialNo,req.ChipNo";
            }

            try
            {
                //ServiceNo = "%" + ServiceNo.Replace("[", "[[]").Replace("%", "[%]") + "%";
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@AspNetUsersId", dto.AspNetUsersId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@MapUnitId", dto.MapUnitId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ServiceNo", $"%{dto.ArmyNo}%", DbType.String, ParameterDirection.Input);

                    var basicDetail = await connection.QueryAsync<DTOSmartSearch>(query, parameters);
                    if (basicDetail != null)
                    {
                        return basicDetail.ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->SearchAllServiceNo");
                return null;
            }
        }


        /// <summary>
        /// Retrieves basic detail information for a specific request based on the provided RequestId.
        /// This includes personal details, address, identity information, card request details, and more.
        /// </summary>
        /// <param name="RequestId">The unique identifier for the card request to fetch associated basic details.</param>
        /// <returns>A DTOBasicDetailForParitalViewResponse object containing the basic details for the specified RequestId, or an empty response if not found or an error occurs.</returns>
        public async Task<DTOBasicDetailForParitalViewResponse?> GetBasicDetailForParitalViewByRequestId(int RequestId)
        {
            try
            {

                string query = @"SELECT bas.PaperIcardNo,bas.NameAsPerRecord,bas.FName,bas.LName,bas.ServiceNo,bas.DOB,bas.DateOfIssue,bas.DateOfCommissioning,bas.PlaceOfIssue,
                                issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode,
                                IdenMark1,AadhaarNo,Height,bld.BloodGroup,regi.Abbreviation RegimentalName,Muni.UnitName,
                                ranks.RankAbbreviation RankName,arm.Abbreviation ArmedName,
                                icardreq.RequestId,icardreq.UpdatedOn RequestDate,appl.Name ApplyFor,uplod.PhotoImagePath,uplod.SignatureImagePath,
                                CASE
                                WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN
                                CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2))
                                ELSE
                                bas.ServiceNo
                                END AS ModifiedServiceNo,icardreq.CardSerialNo,icardreq.ChipNo
                                from BasicDetails bas
                                inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId
                                inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId
                                inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId
                                inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId
                                inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId
                                inner join MRank ranks on ranks.RankId=bas.RankId
                                inner join MArmedType arm on arm.ArmedId=bas.ArmedId
                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                                left join MRegimental regi on regi.RegId=bas.RegimentalId
                                inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId
                                inner join TrnStepCounter stepcount on icardreq.RequestId=stepcount.RequestId
                                where icardreq.RequestId=@RequestId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOBasicDetailForParitalViewResponse>(query, new { RequestId });

                    if (ret != null)
                    {
                        return ret.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                    

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailForParitalViewByRequestId");
                return null;
            }
        }


        /// <summary>
        /// Retrieves a list of all ICard types available in the database.
        /// </summary>
        /// <returns>A list of DTOICardTypeRequest containing all available ICard types.</returns>
        public async Task<List<DTOICardTypeRequest>> GetAllICardType()
        {
            string query = "Select * from MICardType";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ICardTypeList = await connection.QueryAsync<DTOICardTypeRequest>(query);
                    var allrecord = (from e in ICardTypeList
                                     select new DTOICardTypeRequest()
                                     {
                                         TypeId = e.TypeId,
                                         EncryptedId = protector.Protect(e.TypeId.ToString()),
                                         Name = e.Name,
                                     }).ToList();
                    return await Task.FromResult(allrecord);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllICardType");
                return new List<DTOICardTypeRequest>();
            }

        }


        /// <summary>
        /// Retrieves a paginated list of BasicDetails with their ICard status, based on search parameters.
        /// The results include details such as service number, request ID, apply-for status, and ICard type.
        /// </summary>
        /// <param name="dTO">The DTODataTablesRequestFor_BasicDetails_Index object containing the search and pagination parameters.</param>
        /// <returns>A DTODataTablesResponse containing the paginated list of DTOBasicDetailIndexResponse, with total records and filtered records count.</returns>
        public async Task<DTODataTablesResponse<DTOBasicDetailIndexResponse>> GetALLForIcardSttaus(DTODataTablesRequestFor_BasicDetails_Index dTO)
        {
            int? applyfor = 0;
            if (dTO.applyForId == 0) applyfor = null; else applyfor = dTO.applyForId;

            string query = "";
            string wherequery = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ServiceNo"] = "ServiceNo",
                ["RequestId"] = "RequestId", 
                ["ApplId"] = "RequestId",
                ["ApplyFor"] = "ApplyFor"
            };

            var sortOrder = dTO.sortDirection;

            if (dTO.stepcount == 0)//////For all record
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.Name AS ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId,Afor.Name AS ApplyFor,Afor.ApplyForId ,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
                        INNER JOIN BasicDetails B ON B.BasicDetailId = trnicrd.BasicDetailId
                        inner join MRank ran on ran.RankId=B.RankId
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId AND (@applyfor IS NULL OR Afor.ApplyForId = @applyfor)
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                        inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId AND map.AspNetUsersId = @UserId
                        inner join UserProfile pr on pr.UserId = map.UserId
                        left join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId
                        left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId
                        left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1";
                
                wherequery = @"WHERE ( (@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";
                
            }
            else if (dTO.stepcount == 1)//////For Draft
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.Name AS ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId,Afor.Name AS ApplyFor,Afor.ApplyForId ,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId AND trnicrd.StatusId = 1
                        inner join MRank ran on ran.RankId=B.RankId
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId AND (@applyfor IS NULL OR Afor.ApplyForId = @applyfor)
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId = @stepcount
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                        inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId and map.AspNetUsersId = @UserId
                        inner join UserProfile pr on pr.UserId = map.UserId
                        left join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId
                        left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId
                        left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1";
                
                wherequery = @"WHERE ( (@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";

            }

            else if (dTO.stepcount == 777)//////For Completed   
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.Name AS ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId,Afor.Name AS ApplyFor,Afor.ApplyForId ,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd 
                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId AND trnicrd.StatusId = 2
                        inner join MRank ran on ran.RankId=B.RankId 
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId AND (@applyfor IS NULL OR Afor.ApplyForId = @applyfor)
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId 
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId 
                        inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId AND map.AspNetUsersId = @UserId  
                        inner join UserProfile pr on pr.UserId = map.UserId 
                        left join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=1 and fwd.RequestId=trnicrd.RequestId 
                        left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId 
                        left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1";

                wherequery = @"WHERE ( (@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 888)//////For Submitted
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.Name AS ICardType,trnicrd.RequestId,Afor.Name AS ApplyFor,Afor.ApplyForId ,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId
                        inner join MRank ran on ran.RankId=B.RankId 
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId  AND (@applyfor IS NULL OR Afor.ApplyForId = @applyfor)
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId > 1
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId 
                        inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId AND map.AspNetUsersId = @UserId
                        inner join UserProfile pr on pr.UserId = map.UserId 
                        left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId";

                wherequery = @"WHERE  ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 5)
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.TypeId,ty.name AS ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId, ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId, Afor.Name AS ApplyFor,Afor.ApplyForId ,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId AND trnicrd.StatusId = 2
                        inner join MRank ran on ran.RankId=B.RankId
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId AND (@applyfor IS NULL OR Afor.ApplyForId = @applyfor)
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId = @stepcount 
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                        left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=2 
                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId  
                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";

                wherequery = @"where ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 2 || dTO.stepcount == 3 || dTO.stepcount == 4 || dTO.stepcount == 6)//IO
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.TypeId,ty.name AS ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId, ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId ,Afor.Name AS ApplyFor,Afor.ApplyForId ,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId AND trnicrd.StatusId = 1
                        inner join MRank ran on ran.RankId=B.RankId
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId AND (@applyfor IS NULL OR Afor.ApplyForId = @applyfor)
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId = @stepcount 
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                        left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 
                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId  and fwd.IsComplete=0 
                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";

                wherequery = @"where (@SearchTerm IS NULL OR B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm)";

            }
            else if (dTO.stepcount == 7 || dTO.stepcount == 8 || dTO.stepcount == 9 || dTO.stepcount == 10)//Reject From IO
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.TypeId,ty.name AS ICardType,trnicrd.RequestId, ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId,ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId, Afor.Name AS ApplyFor,Afor.ApplyForId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId AND trnicrd.StatusId = 1                        
                        inner join MRank ran on ran.RankId=B.RankId
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                        inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId AND (@applyfor IS NULL OR Afor.ApplyForId = @applyfor)
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                        left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 
                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId  and fwd.FwdStatusId=3 
                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                
                wherequery = @"where (@SearchTerm IS NULL OR B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm)";

            }
            else if (dTO.stepcount == 999)//Reject From IO,MI11 and HQ 54
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.TypeId,ty.name AS ICardType,trnicrd.RequestId, ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId ,Afor.Name AS ApplyFor,Afor.ApplyForId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId AND trnicrd.StatusId = 1
                        inner join MRank ran on ran.RankId=B.RankId
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId AND (@applyfor IS NULL OR Afor.ApplyForId = @applyfor)
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId in (7,8,9,10)
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                        left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 
                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId  and fwd.FwdStatusId=3  
                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";

                wherequery = @"where (@SearchTerm IS NULL OR B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm)";

            }
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "") ? allowedSortColumns[dTO.sortColumn!] : "ServiceNo";
                var multiQuery = query = $@"
                        WITH RecordCTE AS (
                            SELECT DISTINCT Count(*) over () as TotalFilteredRecords, {query} {wherequery} 
                        )
                        SELECT *
                        FROM (
                            SELECT 
                                ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum,
                                *
                            FROM RecordCTE
                        ) AS Numbered
                        WHERE RowNum BETWEEN @Offset AND @Limit;
                    ";
                //select Count(*) over () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query} {wherequery} 
                using (var connection = _contextDP.CreateConnection())
                {
                    var searchTerm = string.IsNullOrEmpty(dTO.searchValue) ? null : $"%{dTO.searchValue}%";

                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", dTO.UserId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@stepcount", dTO.stepcount, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TypeId", dTO.TypeId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@applyfor", applyfor, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", searchTerm, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTOBasicDetailIndexResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var allrecord = (from e in records
                                     select new DTOBasicDetailIndexResponse()
                                     {
                                         TotalFilteredRecords = e.TotalFilteredRecords,
                                         BasicDetailId = e.BasicDetailId,
                                         RegistrationApplyFor = e.RegistrationApplyFor,
                                         EncryptedId = protector.Protect(e.BasicDetailId.ToString()),
                                         EncryptedRequestId = protector.Protect(e.RequestId.ToString()),
                                         FName = e.FName,
                                         LName = e.LName,
                                         ServiceNo = e.ServiceNo,
                                         DOB = e.DOB,
                                         DateOfCommissioning = e.DateOfCommissioning,
                                         PermanentAddress = e.PermanentAddress,
                                         IsTrnFwdId = e.IsTrnFwdId,
                                         StepCounter = e.StepCounter,
                                         StepId = e.StepId,
                                         ICardType = e.ICardType,
                                         ApplyFor = e.ApplyFor,
                                         ApplyForId = e.ApplyForId,
                                         RequestId = e.RequestId,
                                         IsFwdStatusId = e.IsFwdStatusId,
                                         Remark = e.Remark,
                                         ApplId = e.RequestId,
                                         RankName = e.RankName,
                                         IsPosting = e.IsPosting,
                                         UnitName = e.UnitName,
                                         UnitId = e.UnitId
                                     }).ToList();
                    var responseData = new DTODataTablesResponse<DTOBasicDetailIndexResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),  
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = allrecord,
                    };
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetALLForIcardSttaus");
                List<DTOBasicDetailIndexResponse> detailVMs = new List<DTOBasicDetailIndexResponse>();
                var responseData = new DTODataTablesResponse<DTOBasicDetailIndexResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = detailVMs
                };
                return responseData;
            }
        }


        /// <summary>
        /// Retrieves a paginated list of Basic Details based on various filter conditions like step count, type, and search term.
        /// </summary>
        /// <param name="dTO">The data transfer object containing filtering parameters for the query, including step count, type, and search term.</param>
        /// <returns>A DTODataTablesResponse object containing the list of filtered records along with pagination information (total records, filtered records, etc.).</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the query execution.</exception>
        public async Task<DTODataTablesWithSelectedIdsResponse<DTOBasicDetailIndexResponse>> GetALLBasicDetail(DTODataTablesRequestFor_BasicDetails_Index dTO) //int UserId, int stepcount, int TypeId, int applyForId
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            string searchFilter = "";

            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection;
            
            if (dTO.stepcount == 0 || dTO.stepcount == 1)//////For Fwd Record
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ApplId"] = "RequestId",
                    ["ServiceNo"] = "ServiceNo",
                    ["RegimentalName"] = "RegimentalName",
                    ["ApplyFor"] = "ApplyFor",
                    ["ICardType"] = "ICardType"
                };
                selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName";
                fromJoinClause = @"FROM BasicDetails B 
                                    inner join MRank ran on ran.RankId=B.RankId 
                                    inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                    inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                    inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId and trnicrd.StatusId=1
                                    inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId)
                                    inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId 
                                    inner join MICardType ty on ty.TypeId = trnicrd.TypeId 
                                    inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId 
                                    inner join UserProfile pr on pr.UserId = map.UserId 
                                    left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0  and fwd.RequestId=trnicrd.RequestId 
                                    left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                whereClause = @"WHERE map.AspNetUsersId = @UserId";
                searchFilter = @"AND ( (@SearchTerm IS NULL) OR (ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 2 || dTO.stepcount == 3 || dTO.stepcount == 4 || dTO.stepcount == 5 || dTO.stepcount == 6)//IO
            {
                if (dTO.TypeId == 1)///For Icard Submit
                {
                    allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["ApplId"] = "RequestId",
                        ["ServiceNo"] = "ServiceNo",
                        ["RegimentalName"] = "RegimentalName",
                        ["ApplyFor"] = "ApplyFor",
                        ["ICardType"] = "ICardType"
                    };
                    selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName";
                    fromJoinClause = @"FROM BasicDetails B
                                        inner join MRank ran on ran.RankId=B.RankId 
                                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                        inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId and trnicrd.StatusId=1
                                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId)
                                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId = @stepcount
                                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId  and fwd.TypeId=@stepcount  
                                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";
                    whereClause = @"WHERE ";
                    searchFilter = @"( (@SearchTerm IS NULL) OR (ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";
                }
                else if (dTO.stepcount == 3 && dTO.TypeId == 2 && dTO.applyForId == 2) //// For For Action
                {
                    allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["ApplId"] = "RequestId",
                        ["ServiceNo"] = "ServiceNo",
                        ["RegimentalName"] = "RegimentalName",
                        ["ApplyFor"] = "ApplyFor",
                        ["ICardType"] = "ICardType"
                    };
                    selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName";
                    fromJoinClause = @"FROM BasicDetails B
                                        inner join MRank ran on ran.RankId=B.RankId
                                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                        inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId and trnicrd.StatusId=1
                                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId)
                                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId = @stepcount
                                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and fwd.IsComplete=0  and fwd.TypeId=@stepcount";
                    whereClause = @"WHERE ";
                    searchFilter = @"( (@SearchTerm IS NULL) OR (ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";
                }
                else if (dTO.TypeId == 2) //// For For Action
                {
                    allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["ApplId"] = "RequestId",
                        ["ServiceNo"] = "ServiceNo",
                        ["RegimentalName"] = "RegimentalName",
                        ["ApplyFor"] = "ApplyFor",
                        ["ICardType"] = "ICardType"
                    };
                    selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName";
                    fromJoinClause = @"FROM TrnICardRequest trnicrd
                                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId
                                        inner join MRank ran on ran.RankId=B.RankId
                                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId
                                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId = @stepcount
                                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId  and fwd.TypeId=@stepcount and fwd.IsComplete = 0
                                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId 
                                        left join MRegimental mreg on mreg.RegId = B.RegimentalId";
                    whereClause = @"WHERE trnicrd.StatusId = 1 AND (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";
                    searchFilter = @"AND ( (@SearchTerm IS NULL) OR (ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";

                }
                else if (dTO.TypeId == 3 && dTO.stepcount == 3)
                {
                    allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["ApplId"] = "RequestId",
                        ["ServiceNo"] = "ServiceNo",
                        ["RegimentalName"] = "RegimentalName",
                        ["ApplyFor"] = "ApplyFor",
                        ["ICardType"] = "ICardType"
                    };
                    selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName";
                    fromJoinClause = @"FROM BasicDetails B
                                        inner join MRank ran on ran.RankId=B.RankId
                                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                        inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId
                                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.FwdStatusId=2 and fwd.TypeId=3
                                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId 
                                        left join MRegimental mreg on mreg.RegId = B.RegimentalId";
                    whereClause = @"WHERE (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";
                    searchFilter = @"AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";

                }
                else if (dTO.TypeId == 3 && dTO.stepcount == 4)
                {
                    allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["ApplId"] = "RequestId",
                        ["ServiceNo"] = "ServiceNo",
                        ["RegimentalName"] = "RegimentalName",
                        ["ApplyFor"] = "ApplyFor",
                        ["ICardType"] = "ICardType"
                    };
                    selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName";
                    fromJoinClause = @"FROM BasicDetails B
                                        inner join MRank ran on ran.RankId=B.RankId
                                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                        inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId
                                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.FwdStatusId=2 and fwd.TypeId=4 
                                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";
                    whereClause = @"WHERE (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";
                    searchFilter = @"AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";

                }
                else if (dTO.stepcount == 5 || dTO.stepcount == 6)///for exported data
                {
                    allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["ApplId"] = "RequestId",
                        ["ServiceNo"] = "ServiceNo",
                        ["RegimentalName"] = "RegimentalName",
                        ["ApplyFor"] = "ApplyFor",
                        ["ICardType"] = "ICardType"
                    };
                    selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName";
                    fromJoinClause = @"FROM TrnICardRequest trnicrd
                                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId
                                        inner join MRank ran on ran.RankId=B.RankId
                                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and fwd.TypeId=4 and fwd.IsComplete=1
                                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId
                                        left join MRegimental mreg on mreg.RegId = B.RegimentalId";
                    whereClause = @"WHERE trnicrd.StatusId=1 AND (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";
                    searchFilter = @"AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";
                }
                else // For For Show
                {
                    allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["ApplId"] = "RequestId",
                        ["ServiceNo"] = "ServiceNo",
                        ["RegimentalName"] = "RegimentalName",
                        ["ApplyFor"] = "ApplyFor",
                        ["ICardType"] = "ICardType"
                    };
                    dTO.TypeId = dTO.stepcount - 1;
                    selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName";
                    fromJoinClause = @"FROM BasicDetails B
                                        inner join MRank ran on ran.RankId=B.RankId
                                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                        inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId
                                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.FwdStatusId=2
                                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";
                    whereClause = @"WHERE (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";
                    searchFilter = @"AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";
                }
            }
            else if (dTO.stepcount == 7 || dTO.stepcount == 8 || dTO.stepcount == 9 || dTO.stepcount == 10)//Reject From IO
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ApplId"] = "RequestId",
                    ["ServiceNo"] = "ServiceNo",
                    ["RegimentalName"] = "RegimentalName",
                    ["ApplyFor"] = "ApplyFor",
                    ["ICardType"] = "ICardType"
                };
                selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,fwd.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName";
                fromJoinClause = @"FROM BasicDetails B
                                    inner join MRank ran on ran.RankId=B.RankId
                                    inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                    inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                    inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId
                                    inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                                    inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                                    inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                    inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.StepId=@stepcount 
                                    inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId
                                    left join MRegimental mreg on mreg.RegId = B.RegimentalId";
                whereClause = @"WHERE (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";
                searchFilter = @"AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";
            }
            else if (dTO.stepcount == 11)
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ApplId"] = "RequestId",
                    ["ServiceNo"] = "ServiceNo",
                    ["RegimentalName"] = "RegimentalName",
                    ["ApplyFor"] = "ApplyFor",
                    ["ICardType"] = "ICardType"
                };
                selectFields = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,fwd.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName";
                fromJoinClause = @"FROM TrnICardRequest trnicrd
                                    INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId
                                    inner join MRank ran on ran.RankId=B.RankId 
                                    inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                                    inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                                    inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                                    inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                                    inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                                    inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.FwdStatusId=4
                                    inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";
                whereClause = @"WHERE trnicrd.StatusId=1 AND (@applyForId IS NULL OR Afor.ApplyForId = @applyForId) ";
                searchFilter = @"AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.RequestId LIKE @SearchTerm))";
            }
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "") ? allowedSortColumns[dTO.sortColumn!] : "ServiceNo";
                var multiQuery = $@"
                        WITH RecordCTE AS (
                            SELECT DISTINCT Count(*) over () as TotalFilteredRecords, {selectFields} {fromJoinClause} {whereClause} {searchFilter}
                        )
                        SELECT *
                        FROM (
                            SELECT 
                                ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum,
                                *
                            FROM RecordCTE
                        ) AS Numbered
                        WHERE RowNum BETWEEN @Offset AND @Limit;
                    ";
                string queryRequestIds = $@"SELECT DISTINCT trnicrd.RequestId {fromJoinClause} {whereClause} {searchFilter}";
                using (var connection = _contextDP.CreateConnection())
                {
                    var searchTerm = string.IsNullOrEmpty(dTO.searchValue) ? null : $"%{dTO.searchValue}%";
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", dTO.UserId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@stepcount", dTO.stepcount, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TypeId", dTO.TypeId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@applyForId", dTO.applyForId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", searchTerm, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTOBasicDetailIndexResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    List<int>? selectedIds = new List<int>();

                    if (dTO.AllChecked == true && (string.IsNullOrEmpty(dTO.searchValue) || dTO.SearchTextChanged == true))
                    {
                        var result = await connection.QueryMultipleAsync(queryRequestIds, parameters);
                        selectedIds = (await result.ReadAsync<int>()).ToList();
                    }
                    else
                    {
                        selectedIds = null;
                    }

                    var allrecord = (from e in records
                                     select new DTOBasicDetailIndexResponse()
                                     {
                                         TotalFilteredRecords = e.TotalFilteredRecords,
                                         BasicDetailId = e.BasicDetailId,
                                         RegistrationApplyFor = e.RegistrationApplyFor,
                                         EncryptedId = protector.Protect(e.BasicDetailId.ToString()),
                                         EncryptedRequestId = protector.Protect(e.RequestId.ToString()),
                                         FName = e.FName,
                                         LName = e.LName,
                                         ServiceNo = e.ServiceNo,
                                         DOB = e.DOB,
                                         DateOfCommissioning = e.DateOfCommissioning,
                                         PermanentAddress = e.PermanentAddress,
                                         IsTrnFwdId = e.IsTrnFwdId,
                                         StepCounter = e.StepCounter,
                                         StepId = e.StepId,
                                         ICardType = e.ICardType,
                                         ApplyFor = e.ApplyFor,
                                         ApplyForId = e.ApplyForId,
                                         RequestId = e.RequestId,
                                         IsFwdStatusId = e.IsFwdStatusId,
                                         ApplId = e.RequestId,
                                         RankName = e.RankName,
                                         UnitId = e.UnitId,
                                         UnitName = e.UnitName,
                                         RegimentalName = e.RegimentalName,

                                     }).ToList();

                    var responseData = new DTODataTablesWithSelectedIdsResponse<DTOBasicDetailIndexResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(), 
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        selectedIds = selectedIds,
                        data = allrecord,
                    };
                    return responseData;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetALLBasicDetail");
                List<DTOBasicDetailIndexResponse> detailVMs = new List<DTOBasicDetailIndexResponse>();
                var responseData = new DTODataTablesWithSelectedIdsResponse<DTOBasicDetailIndexResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    selectedIds = null,
                    data = detailVMs
                };
                return responseData;
            }
        }


        /// <summary>
        /// Retrieves the basic details of an individual based on their RequestId.
        /// </summary>
        /// <param name="RequestId">The unique identifier of the request for which the basic details are to be fetched.</param>
        /// <returns>
        /// A <see cref="BasicDetailCrtAndUpdVM"/> object containing the requested basic details if found, or null if no details are found or an error occurs.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is an error while executing the database query.</exception>
        public async Task<BasicDetailCrtAndUpdVM?> GetBasicDetailByRequestId(int RequestId)
        {
            string query = "select bas.NameAsPerRecord,bas.FName,bas.LName,bas.ServiceNo,bas.DOB,bas.DateOfIssue,bas.DateOfCommissioning,bas.PlaceOfIssue," +
                            " issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId," +
                            " CASE " +
                            " WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2)) " +
                            " ELSE" +
                            " bas.ServiceNo " +
                            " END AS ModifiedServiceNo " +
                            " from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.StatusId in (1,2,3)" +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where icardreq.RequestId=@RequestId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    BasicDetailCrtAndUpdVM? BasicDetailList = (await connection.QueryAsync<BasicDetailCrtAndUpdVM>(query, new { RequestId })).FirstOrDefault();
                    return BasicDetailList;
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailByRequestId");
                return null;
            }
        }


        /// <summary>
        /// Retrieves the basic details of an individual based on their BasicDetailId.
        /// </summary>
        /// <param name="BasicDetailId">The unique identifier of the individual whose basic details are to be fetched.</param>
        /// <returns>
        /// A <see cref="BasicDetailCrtAndUpdVM"/> object containing the requested basic details if found, or null if no details are found or an error occurs.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is an error while executing the database query.</exception>
        public async Task<BasicDetailCrtAndUpdVM?> GetBasicDetailById(int BasicDetailId)
        {
            string query = "select bas.*," +
                            " issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where bas.BasicDetailId=@BasicDetailId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailCrtAndUpdVM>(query, new { BasicDetailId });

                    return BasicDetailList.FirstOrDefault();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailById");
                return null;
            }
        }


        /// <summary>
        /// Retrieves the basic details of an individual for editing, based on their BasicDetailId.
        /// </summary>
        /// <param name="BasicDetailId">The unique identifier of the individual whose details are to be fetched for editing.</param>
        /// <returns>
        /// A <see cref="BasicDetailCrtAndUpdVM"/> object containing the requested basic details if found, or null if no details are found or an error occurs.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is an error while executing the database query.</exception>
        public async Task<BasicDetailCrtAndUpdVM?> GetBesicDetailForEditById(int BasicDetailId)
        {
            string query = "select bas.*," +
                            " issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " left join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.StatusId=1 " +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where bas.BasicDetailId=@BasicDetailId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailCrtAndUpdVM>(query, new { BasicDetailId });

                    return BasicDetailList.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBesicDetailForEditById");
                return null;
            }

        }


        /// <summary>
        /// Retrieves basic details for export based on the provided RequestIds and forwarding condition data.
        /// Updates the related records in the database before retrieving the export data.
        /// </summary>
        /// <param name="Data">An instance of <see cref="DTODataExportRequest"/> containing the list of RequestIds and other export-related parameters.</param>
        /// <param name="dTOApplFwdCondition">An instance of <see cref="DTOApplFwdConditionRequest"/> containing conditions to filter the forwarding data such as record office, armed forces, etc.</param>
        /// <returns>
        /// A list of <see cref="DTODataExportsResponse"/> objects that contain the requested basic details for the given RequestIds.
        /// If no data is found or an error occurs during execution, an empty list is returned.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error occurs during the database query execution, including issues in executing transactions or retrieving data.
        /// </exception>
        public async Task<List<DTODataExportsResponse>> GetBesicdetailsByRequestId(DTODataExportRequest Data, DTOApplFwdConditionRequest dTOApplFwdCondition)
        {
            #region Old Code 
            //var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            //int[] Ids = Data.Ids;
            //string query = "";
            //try
            //{
            //    string query1 = " update TrnFwds set IsComplete=1 where RequestId in @Ids ";
            //    await db.ExecuteAsync(query1, new { Ids }, transaction: transaction);

            //    string query2 = " update TrnStepCounter set StepId=5 where RequestId in @Ids ";
            //    await db.ExecuteAsync(query2, new { Ids }, transaction: transaction);

            //    string query3 = " update TrnICardRequest set StatusId=2 where  RequestId in @Ids ";
            //    await db.ExecuteAsync(query3, new { Ids }, transaction: transaction);

            //    // Commit the transaction if all operations succeed
            //    transaction.Commit();

            //    if (Data.IsJco == 0)
            //    {
            //        query = " select bas.*,issaut.Name IssuingAuth,mapl.Name ApplyFor, " +
            //                " trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
            //                " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
            //                " regi.Abbreviation RegimentalName,regi.Location RegimentalLocation,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
            //                " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId,MICardType.Name ICardType,reco.RecordOfficeId,reco.Name RecordOffice,icardreq.RequestId from BasicDetails bas" +
            //                " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
            //                " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
            //                " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
            //                " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
            //                " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
            //                " inner join MRank ran on ran.RankId=bas.RankId" +
            //                " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
            //                " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
            //                " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
            //                " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId " + //and icardreq.Status=0 
            //                " inner join TrnStepCounter scounter on scounter.RequestId=icardreq.RequestId " +
            //                " inner join MApplyFor mapl on mapl.ApplyForId=scounter.ApplyForId " +
            //                " inner join MRecordOffice reco on bas.ArmedId=reco.ArmedId" +
            //                " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId " +
            //                " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
            //                " where icardreq.RequestId in @Ids";
            //    }
            //    else
            //    {
            //        query = " select bas.*,issaut.Name IssuingAuth,mapl.Name ApplyFor, trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode, " +
            //                 " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId, " +
            //                 " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId, " +
            //                 " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId," +
            //                 " MICardType.Name ICardType," +
            //                 " CASE WHEN ran.orderby<=4 THEN '126' ELSE reco.RecordOfficeId END RecordOfficeId," +
            //                 " CASE WHEN ran.orderby<=4 THEN 'MP 6A' ELSE reco.Name END RecordOffice,icardreq.RequestId" +
            //                 " from BasicDetails bas " +
            //                 " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
            //                 " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId " +
            //                 " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId " +
            //                 " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId " +
            //                 " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId " +
            //                 " inner join MRank ran on ran.RankId=bas.RankId " +
            //                 " inner join MArmedType arm on arm.ArmedId=bas.ArmedId " +
            //                 " inner join MapUnit uni on uni.UnitMapId=bas.UnitId " +
            //                 " inner join MUnit Muni on Muni.UnitId=uni.UnitId " +
            //                 " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId  " +
            //                 " inner join TrnStepCounter scounter on scounter.RequestId=icardreq.RequestId " +
            //                 " inner join MApplyFor mapl on mapl.ApplyForId=scounter.ApplyForId " +
            //                 " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId  " +
            //                 " inner join MRecordOffice reco on reco.ArmedId=56" +
            //                 " inner join OROMapping OROMap on reco.RecordOfficeId=OROMap.RecordOfficeId" +
            //                 " left join MRegimental regi on regi.RegId=bas.RegimentalId where icardreq.RequestId in @Ids" +
            //                 " and bas.ArmedId in (select value from string_split(oromap.ArmedIdList,',')) " +
            //        " order by reco.RecordOfficeId";
            //    }

            //    var BasicDetailList = await db.QueryAsync<DTODataExportsResponse>(query, new { Ids });

            //    return BasicDetailList.ToList();

            //}
            //catch (Exception ex)
            //{
            //    // Rollback the transaction if any operation fails
            //    transaction.Rollback();
            //    _logger.LogError(1001, ex, "BasicDetailDB->GetBesicdetailsByRequestId");
            //    return new List<DTODataExportsResponse>();
            //}
            //finally
            //{
            //    // Dispose of the connection
            //    db.Dispose();
            //}
            #endregion Old Code 

            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            int[] Ids = Data.Ids;
            string query = "";
            try
            {
                string query1 = " update TrnFwds set IsComplete=1 where RequestId in @Ids ";
                await db.ExecuteAsync(query1, new { Ids }, transaction: transaction);

                string query2 = " update TrnStepCounter set StepId=5 where RequestId in @Ids ";
                await db.ExecuteAsync(query2, new { Ids }, transaction: transaction);

                //string query3 = " update TrnICardRequest set StatusId=2 where  RequestId in @Ids ";
                //await db.ExecuteAsync(query3, new { Ids }, transaction: transaction);

                // Commit the transaction if all operations succeed
                transaction.Commit();

                if (Data.IsJco == 0)
                {
                    query = " select bas.*,issaut.Name IssuingAuth,mapl.Name ApplyFor, " +
                            " trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,regi.Location RegimentalLocation,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId,MICardType.Name ICardType,reco.RecordOfficeId,reco.Name RecordOffice,icardreq.RequestId from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId " + //and icardreq.Status=0 
                            " inner join TrnStepCounter scounter on scounter.RequestId=icardreq.RequestId " +
                            " inner join MApplyFor mapl on mapl.ApplyForId=scounter.ApplyForId " +
                            " inner join MRecordOffice reco on bas.ArmedId=reco.ArmedId" +
                            " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId " +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where icardreq.RequestId as ApplId in @Ids";
                }
                else
                {
                    query = @"select bas.*,issaut.Name IssuingAuth,mapl.Name ApplyFor, trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode,
                                trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId,
                                regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId,
                                ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId,
                                MICardType.Name ICardType,
                                CASE
	                                WHEN arm.Abbreviation in @MPRSO_ArmedAbbreviation THEN @MPRSO_RecordOfficeId
	                                WHEN UPPER(LEFT(bas.ServiceNo,2)) = @MP6F_ArmyNoPrefix THEN @MP6F_RecordOfficeId
	                                WHEN ran.Orderby<=@MP6A_RankOrderby THEN @MP6A_RecordOfficeId 
	                                ELSE reco.RecordOfficeId 
	                                END AS RecordOfficeId,
                                CASE 
	                                WHEN arm.Abbreviation in @MPRSO_ArmedAbbreviation THEN @MPRSO_Name
	                                WHEN UPPER(LEFT(bas.ServiceNo,2)) = @MP6F_ArmyNoPrefix THEN @MP6F_Name
	                                WHEN ran.Orderby<=@MP6A_RankOrderby THEN @MP6A_Name 
	                                ELSE reco.Name 
	                                END AS RecordOffice
                                ,icardreq.RequestId as ApplId  from BasicDetails bas
                                inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId
                                inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId
                                inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId
                                inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId
                                inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId
                                inner join MRank ran on ran.RankId=bas.RankId
                                inner join MArmedType arm on arm.ArmedId=bas.ArmedId
                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId
                                inner join TrnStepCounter scounter on scounter.RequestId=icardreq.RequestId
                                inner join MApplyFor mapl on mapl.ApplyForId=scounter.ApplyForId
                                inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId
                                inner join MRecordOffice reco on reco.ArmedId=56
                                inner join OROMapping OROMap on reco.RecordOfficeId=OROMap.RecordOfficeId
                                left join MRegimental regi on regi.RegId=bas.RegimentalId where icardreq.RequestId in @Ids
                                and bas.ArmedId in (select value from string_split(oromap.ArmedIdList,','))
                                order by RecordOfficeId
                                ";
                }
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Ids);
                parameters.Add("@MPRSO_RecordOfficeId", dTOApplFwdCondition.MPRSO.RecordOfficeId);
                parameters.Add("@MPRSO_ArmedAbbreviation", dTOApplFwdCondition.MPRSO.ArmedAbbreviation);
                parameters.Add("@MPRSO_Name", dTOApplFwdCondition.MPRSO.Name);

                parameters.Add("@MP6F_RecordOfficeId", dTOApplFwdCondition.MP6F.RecordOfficeId);
                parameters.Add("@MP6F_ArmyNoPrefix", dTOApplFwdCondition.MP6F.ArmyNoPrefix);
                parameters.Add("@MP6F_Name", dTOApplFwdCondition.MP6F.Name);

                parameters.Add("@MP6A_RecordOfficeId", dTOApplFwdCondition.MP6A.RecordOfficeId);
                parameters.Add("@MP6A_RankOrderby", dTOApplFwdCondition.MP6A.RankOrderby);
                parameters.Add("@MP6A_Name", dTOApplFwdCondition.MP6A.Name);

                var BasicDetailList = await db.QueryAsync<DTODataExportsResponse>(query, parameters);

                return BasicDetailList.ToList();

            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "BasicDetailDB->GetBesicdetailsByRequestId");
                return new List<DTODataExportsResponse>();
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }


        /// <summary>
        /// Retrieves the necessary data for digital signing and prepares it in XML format.
        /// This method retrieves detailed information about a request based on the provided `RequestIds` 
        /// and processes the data for digital signing, including user profile and application details.
        /// </summary>
        /// <param name="Data">An instance of <see cref="DTODataExportRequest"/> containing the list of RequestIds to fetch the details for digital signing.</param>
        /// <returns>
        /// A <see cref="DTOXMLDigitalResponse"/> object that contains the digital sign response, including application details, profile details,
        /// and the last record for forwarding with its step ID for digital signature processing.
        /// If an error occurs or no data is found, an empty response will be returned.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is an error while executing the database query or processing the data.</exception>
        public async Task<DTOXMLDigitalResponse> GetDataDigitalXmlSign(DTODataExportRequest Data)
        {
            DTOXMLDigitalSignResponse dTOXMLDigitalSignResponse = new DTOXMLDigitalSignResponse();
            string query = "select bas.*,issaut.Name IssuingAuth ,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode, " +
                           " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId, " +
                           " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                           " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId," +
                           " trninfo.InfoId,MICardType.Name ICardType ,GETDATE() XmlCreatedOn," +
                           " App.Name ProApplyFor,reg.Name ProRegistraion,(select Name from MICardType where TypeId=icardreq.TypeId) ProType,users.DomainId ProDomainId,unit.UnitName ProUnitName,unit.Suffix ProSuffix,unit.Sus_no ProSUSNO,pro.Name ProName,ranks.RankAbbreviation ProRankName,pro.ArmyNo ProArmyName" +
                           " from BasicDetails bas " +
                           " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                           " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId " +
                           " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId " +
                           " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId " +
                           " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId " +
                           " inner join MRank ran on ran.RankId=bas.RankId " +
                           " inner join MArmedType arm on arm.ArmedId=bas.ArmedId " +
                           " inner join MapUnit uni on uni.UnitMapId=bas.UnitId " +
                           " inner join MUnit Muni on Muni.UnitId=uni.UnitId " +
                           " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.StatusId=1  " +
                           " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId  " +
                           " inner join TrnDomainMapping trn on trn.Id=icardreq.TrnDomainMappingId" +
                           " inner join AspNetUsers users on users.Id = trn.AspNetUsersId " +
                           " inner join MapUnit mapuni on mapuni.UnitMapId = trn.UnitId " +
                           " inner join MUnit unit on unit.UnitId = mapuni.UnitId " +
                           " left join UserProfile pro on pro.UserId = trn.UserId " +
                           " inner join MRank ranks on ranks.RankId = pro.RankId" +
                           " inner join MApplyFor App on App.ApplyForId=bas.ApplyForId" +
                           " inner join MRegistration reg on App.ApplyForId=reg.ApplyForId and App.ApplyForId=bas.ApplyForId and reg.RegistrationId= icardreq.RegistrationId" +
                           " left join MRegimental regi on regi.RegId=bas.RegimentalId where icardreq.RequestId in @Ids";
            int[] Ids = Data.Ids;
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailList = await connection.QueryFirstAsync<dynamic>(query, new { Ids });
                if (BasicDetailList != null)
                {
                    ApplicationDetails applicationDetails = new ApplicationDetails();
                    string FN = BasicDetailList.FName;
                    string LN = BasicDetailList.LName != null ? BasicDetailList.LName : "";

                    applicationDetails.Name = (FN + " " + LN).Trim();
                    applicationDetails.ServiceNo = BasicDetailList.ServiceNo;
                    applicationDetails.DOB = BasicDetailList.DOB;
                    applicationDetails.PlaceOfIssue = BasicDetailList.PlaceOfIssue;
                    applicationDetails.DateOfIssue = BasicDetailList.DateOfIssue;
                    applicationDetails.IssuingAuth = BasicDetailList.IssuingAuth;
                    applicationDetails.DateOfCommissioning = BasicDetailList.DateOfCommissioning;
                    applicationDetails.PaperIcardNo = BasicDetailList.PaperIcardNo;
                    applicationDetails.State = BasicDetailList.State;
                    applicationDetails.District = BasicDetailList.District;
                    applicationDetails.PS = BasicDetailList.PS;
                    applicationDetails.PO = BasicDetailList.PO;
                    applicationDetails.Tehsil = BasicDetailList.Tehsil;
                    applicationDetails.Village = BasicDetailList.Village;
                    applicationDetails.PinCode = BasicDetailList.PinCode;
                    applicationDetails.SignatureImagePath = BasicDetailList.SignatureImagePath;
                    applicationDetails.PhotoImagePath = BasicDetailList.PhotoImagePath;
                    applicationDetails.IdenMark1 = BasicDetailList.IdenMark1;
                    applicationDetails.IdenMark2 = BasicDetailList.IdenMark2;
                    applicationDetails.AadhaarNo = Convert.ToString(BasicDetailList.AadhaarNo);
                    applicationDetails.Height = Convert.ToString(BasicDetailList.Height);
                    applicationDetails.BloodGroup = BasicDetailList.BloodGroup;
                    applicationDetails.RegimentalName = BasicDetailList.RegimentalName;
                    applicationDetails.UnitName = BasicDetailList.UnitName;
                    applicationDetails.RankName = BasicDetailList.RankName;
                    applicationDetails.ArmedName = BasicDetailList.ArmedName;

                    applicationDetails.ICardType = BasicDetailList.ICardType;
                    applicationDetails.XmlCreatedOn = BasicDetailList.XmlCreatedOn;

                    Profiledtls profiledtls = new Profiledtls();
                    profiledtls.ProApplyFor = BasicDetailList.ProApplyFor;
                    profiledtls.ProRegistraion = BasicDetailList.ProRegistraion;
                    profiledtls.ProType = BasicDetailList.ProType;
                    profiledtls.ProDomainId = BasicDetailList.ProDomainId;
                    profiledtls.ProUnitName = BasicDetailList.ProUnitName;
                    profiledtls.ProSuffix = BasicDetailList.ProSuffix;
                    profiledtls.ProSUSNO = BasicDetailList.ProSUSNO;
                    profiledtls.ProName = BasicDetailList.ProName;
                    profiledtls.ProRankName = BasicDetailList.ProRankName;
                    profiledtls.ProArmyName = BasicDetailList.ProArmyName;

                    dTOXMLDigitalSignResponse.applicationDetails = applicationDetails;
                    dTOXMLDigitalSignResponse.profiledtls = profiledtls;
                }

                DTOFwdLastRecForDigitalSign dTOFwdLastRecForDigitalSign = new DTOFwdLastRecForDigitalSign();
                dTOFwdLastRecForDigitalSign = await ICardFwdLastRec(Ids[0]);
                dTOFwdLastRecForDigitalSign.StepId = Data.StepId;
                dTOXMLDigitalSignResponse.RecForDigitalSign = dTOFwdLastRecForDigitalSign;

                DTOXMLDigitalResponse dTOXMLDigitalResponse = new DTOXMLDigitalResponse();
                dTOXMLDigitalResponse.Header = dTOXMLDigitalSignResponse;
                return dTOXMLDigitalResponse;
            }
        }


        /// <summary>
        /// Retrieves detailed basic information based on RequestIds or TrnFwdIds and generates a CSV string for export.
        /// The method constructs a query depending on whether the provided IDs are `TrnFwdId` or `RequestId` and then processes the results into CSV format.
        /// </summary>
        /// <param name="Data">An instance of <see cref="DTOCSVExportRequest"/> containing the list of IDs (either `TrnFwdId` or `RequestId`) for which the data is fetched.</param>
        /// <returns>
        /// A CSV string representing the requested data. The CSV includes fields such as service number, name, date of birth, rank, and address information.
        /// If an error occurs or no data is found, the method returns <c>null</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if there is an error while executing the database query or processing the results into a CSV string.
        /// </exception>
        public async Task<string?> GetCSVString(DTOCSVExportRequest Data)
        {
            #region Old Code
            //string query = string.Empty;
            //if (Data.IdsTypeRequestIdOrTrnFwdId == true)
            //{
            //    //Ids is TrnFwdId.
            //    query = " Select B.ServiceNo,B.NameAsPerRecord,B.DOB,B.DateOfCommissioning,ran.RankAbbreviation,B.FName,B.LName,munit.UnitName,Afor.Name ApplyFor,ty.name ICardType,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode from BasicDetails B " +
            //            " inner join TrnAddress trnadd on trnadd.BasicDetailId = B.BasicDetailId " +
            //            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            " inner join MRank ran on ran.RankId=B.RankId " +
            //            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
            //            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
            //            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId " +
            //            " where fwd.TrnFwdId in @Ids";
            //}
            //else
            //{
            //    //Ids is RequestId.
            //    query = " Select B.ServiceNo,B.NameAsPerRecord,B.DOB,B.DateOfCommissioning,ran.RankAbbreviation,B.FName,B.LName,munit.UnitName,Afor.Name ApplyFor,ty.name ICardType,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode from BasicDetails B " +
            //            " inner join TrnAddress trnadd on trnadd.BasicDetailId = B.BasicDetailId " +
            //            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            " inner join MRank ran on ran.RankId=B.RankId " +
            //            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
            //            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
            //            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //            " where trnicrd.RequestId in @Ids";
            //}
            #endregion Old Code
            string query = string.Empty;
            
            query = " Select trnicrd.RequestId as ApplId,B.ServiceNo,B.NameAsPerRecord,B.DOB,B.DateOfCommissioning,ran.RankAbbreviation,B.FName,B.LName,munit.UnitName,Afor.Name ApplyFor,ty.name ICardType,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode from BasicDetails B " +
                    " inner join TrnAddress trnadd on trnadd.BasicDetailId = B.BasicDetailId " +
                    " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                    " inner join MRank ran on ran.RankId=B.RankId " +
                    " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                    " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                    " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                    " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                    " where trnicrd.RequestId in @Ids";

            int[] Ids = Data.Ids;
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOCSVExportResponseForSqlQuery>(query, new { Ids });
                    int sno = 1;
                    var allrecord = (from e in BasicDetailList
                                     select new DTOCSVExportResponse()
                                     {
                                         Sno = sno++,
                                         ApplId = e.ApplId,
                                         ServiceNo = e.ServiceNo,
                                         NameAsPerRecord = e.NameAsPerRecord,
                                         DOB = DateOnly.FromDateTime(e.DOB),
                                         DateOfCommissioning = DateOnly.FromDateTime(e.DateOfCommissioning),
                                         RankAbbreviation = e.RankAbbreviation,
                                         FName = e.FName,
                                         LName = e.LName,
                                         UnitName = e.UnitName,
                                         ApplyFor = e.ApplyFor,
                                         ICardType = e.ICardType,
                                         State = e.State,
                                         District = e.District,
                                         PS = e.PS,
                                         PO = e.PO,
                                         Tehsil = e.Tehsil,
                                         Village = e.Village,
                                         PinCode = e.PinCode,
                                         PermanentAddress = "Village - " + (e.Village ?? "") + ", Post Office - " + (e.PO ?? "") + ", Tehsil - " + (e.Tehsil ?? "") + ", District - " + (e.District ?? "") + ", State - " + (e.State ?? "") + ", Pin Code - " + e.PinCode,
                                     }).ToImmutableList();
                    CsvService csvService = new CsvService();
                    string csvData = csvService.GenerateCsv(allrecord);

                    return csvData;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetCSVString");
                return null;
            }
        }


        /// <summary>
        /// Checks the status of the card for the specified RequestId.
        /// Retrieves the card's status from the `TrnICardRequest` table based on the provided RequestId.
        /// </summary>
        /// <param name="RequestId">The unique identifier for the card request whose status is to be checked.</param>
        /// <returns>
        /// A <see cref="byte?"/> representing the status of the card. Returns `null` if the card is not found, otherwise returns the card's status.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if there is an error during database query execution. The exception is logged with an error message for debugging.
        /// </exception>
        public async Task<byte?> CheckCardStatus(int RequestId)
        {
            byte? cardStatus = 0;
            try
            {
                var card = await _context.TrnICardRequest.FindAsync(RequestId);
                cardStatus = card?.StatusId;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->CheckCardStatus");
            }
            return cardStatus;
        }


        /// <summary>
        /// Retrieves the completed card history for the specified RequestId.
        /// This method fetches the completed card request from the `CompletedICardRequests` table based on the provided `RequestId` and deserializes the associated card request history into an object.
        /// </summary>
        /// <param name="RequestId">The unique identifier for the completed card request whose history is to be retrieved.</param>
        /// <returns>
        /// An instance of <see cref="ICardHistoryResponseAll"/> containing the deserialized card request history. If the card request is not found or the history is empty, an empty response object is returned.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error occurs during database query execution or during the deserialization of the card history. The exception is logged for debugging purposes.
        /// </exception>
        public async Task<ICardHistoryResponseAll> ICardHistoryCompleted(int RequestId)
        {
            ICardHistoryResponseAll cardStatus = new ICardHistoryResponseAll();
            try
            {
                var card = await _context.CompletedICardRequests.FirstOrDefaultAsync(req => req.RequestId == RequestId);
                if (!string.IsNullOrEmpty(card?.CardRequestHistoryJson))
                    cardStatus = JsonConvert.DeserializeObject<ICardHistoryResponseAll>(card.CardRequestHistoryJson); 
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistoryCompleted");
            }
            return cardStatus;
        }



        /// <summary>
        /// Retrieves the complete history of a card request based on the provided RequestId.
        /// This includes data related to forwarding, posting, faulty card information, and application closure.
        /// The method queries multiple related tables and returns the data in a structured response object.
        /// </summary>
        /// <param name="RequestId">The unique identifier for the card request whose history is to be retrieved.</param>
        /// <returns>
        /// An instance of <see cref="ICardHistoryResponseAll"/> containing the full card history, including:
        /// - Forwarding details (<see cref="ICardHistoryResponse"/>)
        /// - Posting out information (<see cref="ICardHistoryPostingOutResponse"/>)
        /// - Faulty card details (<see cref="ICardHistoryFaultyCardResponse"/>)
        /// - Card closure information (<see cref="ICardApplCloseCardResponse"/>)
        /// If no history is found or an error occurs, the method returns <c>null</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error occurs while executing the database query or processing the results. 
        /// The exception is logged for debugging purposes.
        /// </exception>
        public async Task<ICardHistoryResponseAll> ICardHistory(int RequestId)
        {
            #region Old Code
            //string query = @"select usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank,
            //                usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ,
            //                CASE fwd.FwdStatusId WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved' WHEN 3 THEN 'Reject' WHEN 4 THEN 'Internal Forward' END Status,
            //                fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark,
            //                fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2,
            //                reason.Reason,postind.Authority,initres.UnitName
            //                from TrnFwds fwd
            //                inner join TrnStepCounter step on fwd.RequestId=step.RequestId
            //                inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=fwd.FromAspNetUsersId
            //                inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId
            //                inner join TrnDomainMapping mapto on mapto.AspNetUsersId=fwd.ToAspNetUsersId
            //                inner join AspNetUsers usersto on usersto.Id=mapto.AspNetUsersId
            //                left join UserProfile profrom on mapfrom.UserId=profrom.UserId
            //                inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId
            //                left join UserProfile proto on mapto.UserId=proto.UserId
            //                left join TrnPostingOut postind on postind.Id=fwd.PostingOutId
            //                left join MPostingReason reason on reason.Id=postind.ReasonId
            //                left join MapUnit Munitres on Munitres.UnitMapId=postind.ToUnitID
            //                left join MUnit initres on initres.UnitId=Munitres.UnitId
            //                inner join MRank ranlto on ranlto.RankId=proto.RankId where fwd.RequestId=@RequestId
            //                order by fwd.TrnFwdId asc";
            //try
            //{
            //    using (var connection = _contextDP.CreateConnection())
            //    {
            //        var BasicDetailList = await connection.QueryAsync<ICardHistoryResponse>(query, new { RequestId });

            //        return BasicDetailList.ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
            //    return null;
            //}
            #endregion
            string query = @"select fwd.TrnFwdId,usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank,
                            usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ,
                            CASE fwd.FwdStatusId WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved' WHEN 3 THEN 'Reject' WHEN 4 THEN 'Internal Forward' END Status,
                            fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark,
                            fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2
                            from TrnFwds fwd
                            inner join TrnStepCounter step on fwd.RequestId=step.RequestId
                            inner join AspNetUsers usersfrom on usersfrom.Id=fwd.FromAspNetUsersId
                            inner join AspNetUsers usersto on usersto.Id=fwd.ToAspNetUsersId
                            inner join UserProfile profrom on fwd.FromUserId=profrom.UserId
                            inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId
                            inner join UserProfile proto on fwd.ToUserId=proto.UserId
                            inner join MRank ranlto on ranlto.RankId=proto.RankId
                            where fwd.RequestId=@RequestId
                            order by fwd.TrnFwdId asc

	                        select reason.Reason,postind.Authority,initres.UnitName,initresfrom.UnitName FromUnit,ISNULL(postind.TrnFwdId,0) TrnFwdId from  
                            TrnPostingOut postind 
                            left join MPostingReason reason on reason.Id=postind.ReasonId
                            left join MapUnit Munitres on Munitres.UnitMapId=postind.ToUnitID
                            left join MUnit initres on initres.UnitId=Munitres.UnitId
                              left join MapUnit Munitresfrom on Munitresfrom.UnitMapId=postind.FromUnitID
                              left join MUnit initresfrom on initresfrom.UnitId=Munitresfrom.UnitId
							where postind.RequestId=@RequestId

                            select mcat.Name FaultyStage,mcat.CategoryId,ISNULL(faulty.TrnFwdId,0) 
                            TrnFwdId,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(faulty.RemarksIds,','))) RemarksNameList
							from TrnFaultyCard faulty
							inner join MCategory mcat on mcat.CategoryId = faulty.CategoryId where faulty.RequestId=@RequestId


                            select trnclose.Authority,trnclose.Remarks,res.Reasons from TrnApplClose trnclose
                            inner join MReasons res on trnclose.ReasonId=res.ReasonId where trnclose.RequestId=@RequestId

";
            try
            {
                ICardHistoryResponseAll cardHistoryResponseAll = new ICardHistoryResponseAll();
                using (var connection = _contextDP.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(query, new { RequestId }))
                    {
                        // var ICardHistory = await multi.ReadFirstOrDefaultAsync<ICardHistoryResponse>();
                        var ICardHistory = (await multi.ReadAsync<ICardHistoryResponse>()).ToList();
                        var PostingOut = (await multi.ReadAsync<ICardHistoryPostingOutResponse>()).ToList();
                        var FaultyCard = (await multi.ReadAsync<ICardHistoryFaultyCardResponse>()).ToList();
                        var CloseCard = await multi.ReadFirstOrDefaultAsync<ICardApplCloseCardResponse>();

                        cardHistoryResponseAll.ICardHistory = ICardHistory;
                        cardHistoryResponseAll.PostingOut = PostingOut;
                        cardHistoryResponseAll.FaultyCard = FaultyCard;
                        cardHistoryResponseAll.CloseCard = CloseCard;

                    }

                    // var BasicDetailList = await connection.QueryAsync<ICardHistoryResponseAll>(query, new { RequestId });

                    // return BasicDetailList.ToList();
                    return cardHistoryResponseAll;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
                return null;
            }

        }


        /// <summary>
        /// Retrieves the last record of a forwarded card based on the provided RequestId for digital signing.
        /// This method checks if the card has passed step 2, and if so, it fetches the corresponding record from the `TrnStepCounter` table. 
        /// If the card has not passed step 2, it fetches the most recent forwarding record from the `TrnFwds` table.
        /// </summary>
        /// <param name="RequestId">The unique identifier for the card request whose last forwarded record is to be retrieved.</param>
        /// <returns>
        /// An instance of <see cref="DTOFwdLastRecForDigitalSign"/> representing the last forwarded record details, including ArmyNo, DomainId, Rank, and the step ID.
        /// If no record is found or an error occurs, an empty <see cref="DTOFwdLastRecForDigitalSign"/> object is returned.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error occurs during the execution of the SQL query or if there's an issue with the database connection. The exception is logged for debugging purposes.
        /// </exception>
        public async Task<DTOFwdLastRecForDigitalSign> ICardFwdLastRec(int RequestId)
        {
            string query = " if exists (select StepId from TrnStepCounter where RequestId=@RequestId and StepId=2)" +
                           " begin" +
                           " select profrom.ArmyNo FromArmyNo,usersfrom.DomainId FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank," +
                           " Getdate() FromDate,trnste.StepId from BasicDetails basi" +
                           " inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=basi.Updatedby " +
                           " inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId " +
                           " left join UserProfile profrom on profrom.UserId=mapfrom.UserId " +
                           " inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId " +
                           " inner join TrnICardRequest req on  req.BasicDetailId=basi.BasicDetailId and req.StatusId=1" +
                           " inner join TrnStepCounter trnste on trnste.RequestId=req.RequestId" +
                           " where trnste.RequestId=@RequestId" +
                           " end" +
                           " else" +
                           " begin" +
                           " select top 1 profrom.ArmyNo FromArmyNo,usersfrom.DomainId FromDomain,profrom.Name FromProfile, " +
                           " ranlfrom.RankAbbreviation FromRank,Getdate() FromDate,step.StepId from TrnFwds fwd  " +
                           " inner join TrnStepCounter step on fwd.RequestId=step.RequestId " +
                           " inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=fwd.FromAspNetUsersId " +
                           " inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId " +
                           " left join UserProfile profrom on mapfrom.UserId=profrom.UserId " +
                           " inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId " +
                           " where fwd.RequestId=@RequestId order by fwd.TrnFwdId desc" +
                           " end";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOFwdLastRecForDigitalSign>(query, new { RequestId });

                    return BasicDetailList.FirstOrDefault() ?? new DTOFwdLastRecForDigitalSign();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
                return new DTOFwdLastRecForDigitalSign();
            }

        }


        /// <summary>
        /// Gets the I-Card forwarding and approval history for the given request ID.
        /// </summary>
        /// <param name="RequestId">The request ID to fetch history for.</param>
        /// <returns>
        /// A list of <see cref="ICardHistoryResponse"/> records if found; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Logged and returns <c>null</c> if any database error occurs.
        /// </exception>
        public async Task<List<ICardHistoryResponse>?> ICardHistoryByRequestId(int RequestId)
        {
            string query = @"select usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank, 
                            usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ,
                            CASE fwd.FwdStatusId WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved' WHEN 3 THEN 'Reject' WHEN 4 THEN 'Internal Forward' END Status,
                            fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark, 
                            fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2, 
                            reason.Reason,postind.Authority,initres.UnitName,req.RequestId 
                            from TrnFwds fwd 
                            inner join TrnICardRequest req on req.RequestId=fwd.RequestId 
                            inner join TrnStepCounter step
                            on fwd.RequestId=step.RequestId
                            inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=fwd.FromAspNetUsersId
                            inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId
                            inner join TrnDomainMapping mapto on mapto.AspNetUsersId=fwd.ToAspNetUsersId
                            inner join AspNetUsers usersto on usersto.Id=mapto.AspNetUsersId
                            left join UserProfile profrom
                            on mapfrom.UserId=profrom.UserId
                            inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId
                            left join UserProfile proto
                            on mapto.UserId=proto.UserId
                            left join TrnPostingOut postind on postind.TrnFwdId=fwd.TrnFwdId
                            left join MPostingReason reason on reason.Id=postind.ReasonId
                            left join MapUnit Munitres on Munitres.UnitMapId=postind.ToUnitID
                            left join MUnit initres on initres.UnitId=Munitres.UnitId
                            inner join MRank ranlto on ranlto.RankId=proto.RankId where req.RequestId=@RequestId
                            order by fwd.TrnFwdId asc";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<ICardHistoryResponse>(query, new { RequestId });

                    return BasicDetailList.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
                return null;
            }

        }


        /// <summary>
        /// Retrieves the task count for the card requests based on the provided UserId, Type, and ApplyForId.
        /// This method queries the database for different task counts depending on the provided Type (1 for Submitted, 2 for Pending).
        /// It counts the number of requests in various statuses such as Drafted, Submitted, Completed, and Rejected, as well as counts for each level of approval and export status.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user for whom the task count is being fetched.</param>
        /// <param name="Type">The type of task count to fetch: 
        /// 1 for Submitted, 2 for Pending (including multiple levels of pending, approved, rejected tasks).</param>
        /// <param name="applyForId">The identifier for the specific application type (e.g., ICard request type).</param>
        /// <returns>
        /// An instance of <see cref="DTOICardTaskCountResponse"/> containing the counts of tasks in various categories (Drafted, Submitted, Completed, Rejected, etc.).
        /// Returns <c>null</c> if an error occurs or no data is found.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if there is an error executing the database query or processing the results.
        /// The exception is logged for debugging purposes.
        /// </exception>
        public async Task<DTOICardTaskCountResponse?> GetTaskCountICardRequest(int UserId, int Type, int applyForId)
        {
            string query = "";
            if (Type == 1) // Submitted
            {
                query = "declare @ToDrafted int=0 declare @ToSubmitted int=0 declare @ToCompleted int=0 declare @ToRejected int=0" +
                        " select @ToDrafted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToSubmitted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>1 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToCompleted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and req.StatusId=2 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToRejected=COUNT(distinct fwd.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId" +
                        " inner join TrnFwds fwd on fwd.RequestId= trnstepcout.RequestId where fwd.ToAspNetUsersId=@UserId and req.StatusId=1 and trnstepcout.StepId in(7,8,9,10) and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToDrafted ToDrafted,@ToSubmitted ToSubmitted,@ToCompleted ToCompleted,@ToRejected ToRejected";
            }
            else if (Type == 2) // Pending
            {
                query = @"declare @_2ndLevelPending int declare @_2ndLevelApproved int declare @_2ndLevelReject int
                        declare @_3rdLevelPending int declare @_3rdLevelApproved int declare @_3rdLevelReject int
                        declare @_4thLevelPending int declare @_4thLevelApproved int declare @_4thLevelReject int
                        declare @ExportPending int declare @ExportApproved int declare @ExportReject int declare @ToInternalForward int declare @CsvUploadCount int


                        select @_2ndLevelPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId 
                        where ToAspNetUsersId=@UserId and IsComplete=0 and fwd.TypeId=2 and  trncard.StatusId=1

                        select @_2ndLevelApproved=COUNT(distinct fwd.RequestId) from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        where FromAspNetUsersId=@UserId and fwd.FwdStatusId=2 and TypeId=3

                        select @_2ndLevelReject=COUNT(distinct fwd.RequestId) from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        where FromAspNetUsersId=@UserId and fwd.StepId=7 and fwd.TypeId=1

                        select @_3rdLevelPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId 
                        where ToAspNetUsersId=@UserId and IsComplete=0 and fwd.TypeId=3 and  trncard.StatusId=1

                        select @_3rdLevelApproved=COUNT(distinct fwd.RequestId) from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        where FromAspNetUsersId=@UserId and fwd.FwdStatusId=2 and fwd.TypeId=4

                        select @_3rdLevelReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        where FromAspNetUsersId=@UserId and fwd.StepId=8 and fwd.TypeId=1

                        select @_4thLevelPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId 
                        where ToAspNetUsersId=@UserId and IsComplete=0 and cou.StepId=4 and  trncard.StatusId=1

                        select @_4thLevelApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId 
                        where ToAspNetUsersId=@UserId and IsComplete=1 and fwd.TypeId=4 and  trncard.StatusId=1

                        select @_4thLevelReject=COUNT(distinct fwd.RequestId) from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        where FromAspNetUsersId=@UserId and fwd.StepId=9 and fwd.TypeId=1

                        select @ExportPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId 
                        where ToAspNetUsersId=@UserId and IsComplete=0 and trncard.StatusId=1

                        select @ExportApproved=COUNT(distinct fwd.RequestId) from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId 
                        where ToAspNetUsersId=@UserId and  trncard.StatusId=1

                        select @ExportReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        where FromAspNetUsersId=@UserId and fwd.StepId=10 and fwd.TypeId=1

                        select @ToInternalForward=COUNT(distinct fwd.RequestId)  from TrnFwds fwd 
                        inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId 
                        inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId 
                        where FromAspNetUsersId=@UserId and FwdStatusId=4 and trncard.StatusId=1

                        select @CsvUploadCount=COUNT(Id) from CSVImports

                        select @_2ndLevelPending _2ndLevelPending,@_2ndLevelApproved _2ndLevelApproved,@_2ndLevelReject _2ndLevelReject, 
                        @_3rdLevelPending _3rdLevelPending,@_3rdLevelApproved _3rdLevelApproved,@_3rdLevelReject _3rdLevelReject, 
                        @_4thLevelPending _4thLevelPending,@_4thLevelApproved _4thLevelApproved,@_4thLevelReject _4thLevelReject, 
                        @ExportPending ExportPending,@ExportApproved ExportApproved,@ExportReject ExportReject,@ToInternalForward ToInternalForward,@CsvUploadCount CsvUploadCount";

            }

            using (var connection = _contextDP.CreateConnection())
            {
                try
                {
                    var ret = await connection.QueryAsync<DTOICardTaskCountResponse>(query, new { UserId, applyForId });
                    return ret.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, "BasicDetailDB->GetTaskCountICardRequest");
                    return null;
                }

            }
        }


        /// <summary>
        /// Retrieves unread notifications for a specified user based on their UserId, Notification Type, and ApplyForId.
        /// This method fetches notifications from the `TrnNotification` table that have not been marked as read and are related to the specified parameters.
        /// The result includes details such as the rank, name, service number, tracking ID, photo image, and URL associated with the notification.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user whose notifications are to be fetched.</param>
        /// <param name="Type">The type of the notification (e.g., the category or purpose of the notification).</param>
        /// <param name="applyForId">The application ID that the notification is associated with.</param>
        /// <returns>
        /// A list of <see cref="DTONotificationResponse"/> objects representing the unread notifications for the user.
        /// Each notification contains the display ID, message, rank, name, service number, tracking ID, photo, and URL related to the notification.
        /// Returns <c>null</c> if an error occurs or if no notifications are found.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error occurs during the execution of the SQL query or when processing the results. 
        /// The exception is logged for debugging purposes.
        /// </exception>
        public async Task<List<DTONotificationResponse>?> GetNotification(int UserId, int Type, int applyForId)
        {
            string query = "select dis.DisplayId,Spanname,Message,ranks.RankAbbreviation,bas.Name,bas.ServiceNo,uplod.PhotoImagePath,dis.Url  from TrnNotification noti" +
                            " inner join TrnNotificationDisplay dis on noti.DisplayId=dis.DisplayId" +
                            " inner join AspNetUsers users on users.Id=noti.SentAspNetUsersId" +
                            " inner join TrnStepCounter stepc on stepc.RequestId=noti.RequestId " +
                            " inner join TrnICardRequest tre on tre.RequestId = noti.RequestId " +
                            " inner join BasicDetails bas on bas.BasicDetailId=tre.BasicDetailId" +
                            " inner join MRank ranks on ranks.RankId=bas.RankId" +
                            " inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId" +
                            " where noti.ReciverAspNetUsersId=@UserId and NotificationTypeId=@Type and stepc.applyforId=@applyForId and [Read]=0 and ReciverAspNetUsersId!=SentAspNetUsersId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTONotificationResponse>(query, new { UserId, Type, applyForId });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetNotification");
                return null;
            }
        }


        /// <summary>
        /// Retrieves unread notifications related to a specific card request (RequestId) based on the provided UserId, Notification Type, and ApplyForId.
        /// This method fetches notifications from the `TrnNotification` table and returns the associated details such as the request ID, display ID, span name, message, rank, name, service number, tracking ID, and image paths. 
        /// Additionally, it checks the URL based on specific display IDs.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user whose notifications are being fetched.</param>
        /// <param name="Type">The type of the notification (e.g., the category or purpose of the notification).</param>
        /// <param name="applyForId">The application ID associated with the request.</param>
        /// <returns>
        /// A list of <see cref="DTONotificationResponse"/> objects representing the unread notifications for the specified user. 
        /// Returns <c>null</c> if no notifications are found or if an error occurs during the process.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error occurs during the execution of the SQL query or while processing the results. 
        /// The exception is logged for debugging purposes.
        /// </exception>
        public async Task<List<DTONotificationResponse>?> GetNotificationRequestId(int UserId, int Type, int applyForId)
        {
            string query = "select Distinct tre.RequestId, dis.DisplayId,Spanname + 'self' Spanname,Message,ranks.RankAbbreviation,bas.Name,bas.ServiceNo,uplod.PhotoImagePath,CASE WHEN dis.DisplayId in (7,8,9,10,17,18,19,20) THEN dis.Url ELSE '' END AS Url  from TrnNotification noti " +
                            " inner join TrnNotificationDisplay dis on noti.DisplayId = dis.DisplayId" +
                            " inner join AspNetUsers users on users.Id = noti.SentAspNetUsersId" +
                            " inner join TrnICardRequest tre on tre.RequestId = noti.RequestId" +
                            " inner join TrnDomainMapping dmap on dmap.Id = tre.TrnDomainMappingId" +
                            " inner join TrnStepCounter cou on cou.RequestId=tre.RequestId" +
                            " inner join BasicDetails bas on bas.BasicDetailId=tre.BasicDetailId" +
                            " inner join MRank ranks on ranks.RankId=bas.RankId" +
                             " inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId" +
                            " where NotificationTypeId = @Type and dmap.AspNetUsersId = @UserId and [Read]=0 and cou.applyforId=@applyForId and ReciverAspNetUsersId=SentAspNetUsersId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTONotificationResponse>(query, new { UserId, Type, applyForId });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetNotificationRequestId");
                return null;
            }

        }


        /// <summary>
        /// Retrieves a list of record offices (ROs) filtered by the provided ArmedId.
        /// This method fetches all the `MRecordOffice` records where the `ArmedId` matches the specified value.
        /// It returns a list of `MRecordOffice` objects containing details of the record offices.
        /// </summary>
        /// <param name="ArmedId">The unique identifier of the armed forces for which the record office list is to be fetched.</param>
        /// <returns>
        /// A list of <see cref="MRecordOffice"/> objects that match the specified `ArmedId`.
        /// Returns <c>null</c> if an error occurs during the database query or if no records are found.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if there is an error executing the database query or processing the results.
        /// The exception is logged for debugging purposes.
        /// </exception>
        public async Task<List<MRecordOffice>?> GetROListByArmedId(byte ArmedId)
        {
            try
            {
                return await _context.MRecordOffice.Where(x => x.ArmedId == ArmedId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetROListByArmedId");
                return null;
            }
        }


        /// <summary>
        /// Retrieves the history of an application based on the provided RequestId.
        /// This method fetches two sets of data:
        /// 1. General application details such as rank, name, service number, and unit.
        /// 2. Application RequestId history, including forwarding status, step ID, and remarks associated with the application.
        /// </summary>
        /// <param name="ApplicationHistory">The RequestId of the application whose history is to be fetched.</param>
        /// <returns>
        /// A <see cref="DTOApplicationTrack"/> object containing:
        /// - <see cref="dTOApplicationDetails"/>: General details about the application (e.g., rank, name, service number, unit).
        /// - <see cref="dTOTrackHistory"/>: The history of the application's progression through different steps, including forwarding status, remarks, and completion status.
        /// Returns <c>null</c> if an error occurs while fetching or processing the data.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if there is an error executing the SQL query or processing the results.
        /// The exception is logged for debugging purposes.
        /// </exception>
        public async Task<DTOApplicationTrack?> ApplicationHistory(int RequestId)
        {
            DTOApplicationTrack lst = new DTOApplicationTrack();
            try
            {
                string query = " select ran.RankAbbreviation RankName,bas.Name,bas.ServiceNo ArmyNo,unit.UnitName,uplod.PhotoImagePath," +
               " ranfrom.RankAbbreviation FromRank,pr.Name FromName,pr.ArmyNo FromArmyNo,users.DomainId" +
               " from BasicDetails bas " +
               " inner join TrnICardRequest req on bas.BasicDetailId=req.BasicDetailId" +
               " inner join TrnUpload uplod on bas.BasicDetailId=uplod.BasicDetailId" +
               " inner join MRank ran on bas.RankId=ran.RankId" +
               " inner join MapUnit muni on bas.UnitId=muni.UnitMapId" +
               " inner join MUnit unit on  muni.UnitId=unit.UnitId" +
               " inner join TrnDomainMapping map on map.Id= req.TrnDomainMappingId" +
               " inner join AspNetUsers users on map.AspNetUsersId=users.Id" +
               " inner join UserProfile pr on pr.UserId = map.UserId" +
               " inner join MRank ranfrom on pr.RankId=ranfrom.RankId" +
               " where req.StatusId=1 and req.RequestId=@RequestId";

                string query2 = " select fwd.FwdStatusId,fwd.stepId,fwd.UpdatedOn,step.Name,fwd.IsComplete," +
                        " isnull(fwd.Remark,'') Remark," +
                        " (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remark2" +
                        " from TrnFwds fwd " +
                        " inner join TrnICardRequest req on fwd.RequestId=req.RequestId" +
                        " inner join MStepCounterStep step on fwd.StepId=step.StepId" +
                        " where req.StatusId=1 and req.RequestId=@RequestId" +
                        " order by fwd.TrnFwdId asc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOApplicationDetails>(query, new { RequestId });
                    lst.dTOApplicationDetails = ret.FirstOrDefault() ?? new DTOApplicationDetails();

                    var ret1 = await connection.QueryAsync<DTOTrackHistory>(query2, new { RequestId });
                    lst.dTOTrackHistory = ret1.ToList();

                    return lst;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ApplicationHistory");
                return null;
            }
        }


        /// <summary>
        /// Checks the validity of card printing requests by validating various attributes such as Application ID, Card Serial Number, Chip Number, 
        /// Step Status, and Service Number. It processes the requests in chunks to ensure efficient handling of large datasets.
        /// The method checks if the Application ID exists, if the Card Serial Number and Chip Number are unique, if the request is eligible for printing,
        /// and if the associated Service Number is valid for the given application. It returns a list of <see cref="DTOCardPriningRequest"/> 
        /// objects with the validity status and remarks for each request.
        /// </summary>
        /// <param name="requests">A list of <see cref="DTOCardPriningRequest"/> objects containing the card printing request details.</param>
        /// <returns>
        /// A list of <see cref="DTOCardPriningRequest"/> objects, each with a validity status and detailed remarks for each request.
        /// The status indicates whether the request is valid or invalid, and the remarks explain the reason for invalidity if applicable.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error occurs during the processing of the requests or while interacting with the database. 
        /// The exception is logged for debugging purposes.
        /// </exception>
        public async Task<List<DTOCardPriningRequest>> CardPrintingCSVCheck(List<DTOCardPriningRequest> requests)
        {
            byte StepId = 5;
            var response = new List<DTOCardPriningRequest>();
            foreach (var batchRecords in requests.Chunk(5000))
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var resultInChunks = (from record in batchRecords
                                          join dbrecord in _context.TrnICardRequest on record.ApplId equals dbrecord.RequestId.ToString() into dbRecordJoin
                                          from matchRecord in dbRecordJoin.DefaultIfEmpty()
                                          join cardNoMatch in _context.TrnICardRequest on record.CardSerialNo equals cardNoMatch.CardSerialNo into cardNoJoin
                                          from cardNoExists in cardNoJoin.DefaultIfEmpty()
                                          join chipNoMatch in _context.TrnICardRequest on record.ChipNo equals chipNoMatch.ChipNo into chipNoJoin
                                          from chipNoExists in chipNoJoin.DefaultIfEmpty()
                                          join stepStatus in _context.TrnStepCounter on new { RequestId = (matchRecord == null ? 0 : matchRecord.RequestId), StepId } equals new { stepStatus.RequestId, stepStatus.StepId } into stepStatusJoin
                                          from stepStatus in stepStatusJoin.DefaultIfEmpty()
                                          join armyNoCheck in _context.BasicDetails on new { BasicDetailId = (matchRecord == null ? 0 : matchRecord.BasicDetailId), ServiceNo = record.ServiceNo } equals new { armyNoCheck.BasicDetailId, armyNoCheck.ServiceNo } into basicDetailJoin
                                          from armyNoCheck in basicDetailJoin.DefaultIfEmpty()
                                          select new DTOCardPriningRequest
                                          {
                                              ApplId = record.ApplId,
                                              ServiceNo = record.ServiceNo,
                                              ChipNo = record.ChipNo,
                                              CardSerialNo = record.CardSerialNo,
                                              IsValid = matchRecord != null && cardNoExists == null && chipNoExists == null && stepStatus != null && armyNoCheck != null,
                                              Status = matchRecord != null && cardNoExists == null && chipNoExists == null && stepStatus != null ? "Valid" : "DbInvalid",
                                              Remarks = (matchRecord == null ? "ApplId not exists; " : "") +
                                                            (cardNoExists != null ? "CardSerialNo already exists; " : "") +
                                                            (chipNoExists != null ? "ChipNo already exists; " : "") +
                                                            (matchRecord != null && stepStatus == null ? "Card application is not available for printing; " : "") +
                                                            (matchRecord != null && armyNoCheck == null ? "Service no. is invalid for this card application; " : "")
                                          }
                        ).ToList();

                    response.AddRange(resultInChunks);
                }
            }

            return response;
        }


        /// <summary>
        /// Uploads a list of card printing requests by converting them into a DataTable and passing them to a stored procedure for processing.
        /// This method processes the requests in chunks of 5000 and executes the `CardPriningCSVImport` stored procedure to import the data.
        /// It returns a response indicating the result of the import operation, including any relevant messages or statuses.
        /// </summary>
        /// <param name="requests">A list of <see cref="DTOCardPriningRequest"/> objects representing the card printing requests to be uploaded.</param>
        /// <returns>
        /// A <see cref="DTOUploadChipAndSerialResponse"/> object containing the result of the upload operation.
        /// This includes a message indicating success or failure, and any additional information related to the process.
        /// Returns <c>null</c> if the upload fails or an error occurs during processing.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if there is an error during the chunking of requests, the creation of the DataTable, 
        /// or the execution of the stored procedure. Any error is captured and the exception message is included in the response.
        /// </exception>
        public async Task<DTOUploadChipAndSerialResponse> CardPrintingCSVUpload(List<DTOCardPriningRequest> requests)
        {
            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();
            try
            {
                foreach (var batchRecords in requests.Chunk(5000))
                {
                    using (var connection = _contextDP.CreateConnection())
                    {
                        DataTable cardDistribution = DataTableHelper.ToDataTable(batchRecords, "Remarks", "IsValid", "Status");
                        var parameters = new DynamicParameters();
                        parameters.Add("@data", cardDistribution.AsTableValuedParameter("UT_CardPriningCSV"));

                        response = (await connection.QueryAsync<DTOUploadChipAndSerialResponse>("CardPriningCSVImport",
                                                                                                parameters,
                                                                                                commandType: CommandType.StoredProcedure
                                   )).FirstOrDefault();
                    }
                }
            }
            catch (Exception ee)
            {
                response.Message = ee.Message;
            }
            return response;
        }


        /// <summary>
        /// Uploads a list of card dispatch requests in CSV format and processes them in batches. 
        /// This method inserts dispatch card information into the database, processes the data in chunks of 5000 records, 
        /// and executes the `CardDispatchCSVImport` stored procedure to import the data into the system.
        /// The method returns a response indicating the success or failure of the upload operation.
        /// </summary>
        /// <param name="requests">A list of <see cref="DTOCardDispatchCheckRequest"/> objects representing the card dispatch requests to be uploaded.</param>
        /// <param name="dTODispatch">A <see cref="DTODispatchOutRequest"/> object containing the dispatch details such as step, apply for ID, 
        /// record office ID, dispatch dates, and other relevant information for the dispatch operation.</param>
        /// <returns>
        /// A <see cref="DTOGenericResponse{string}"/> object containing the result of the upload operation. The `Value` property is set to "Success" 
        /// on successful import, and `Message` contains the dispatch card ID. In case of failure, the `Result` is set to false, and the error message 
        /// is logged and returned in the `Message` property.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if any error occurs during the upload process or while interacting with the database. 
        /// The exception is logged, and the response `Message` is set accordingly.
        /// </exception>
        public async Task<DTOGenericResponse<string>> CardDispatchCSVUpload(List<DTOCardDispatchCheckRequest> requests, DTODispatchOutRequest dTODispatch)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            try
            {
                byte StepId;
                if (dTODispatch.Step == 1)
                {
                    StepId = 11;
                }
                else
                {
                    StepId = 13;
                }
                string insert = "";
                insert = @"INSERT INTO TrnDispatchCard(Step,ApplyForId,RegId,RecordOfficeId,OutDate,ReceiptDate,DispatchDate,RefOfDispatch,NameOfCourierIncharge,UploadFilePath,FromRemark,ToRemark,FromUnitId,ToUnitId,ToUserId,FromUserId,FromAspNetUsersId,ToAspNetUsersId,IsComplete,IsActive,Updatedby,UpdatedOn,DispatchModeId)
                                OUTPUT INSERTED.DispatchCardId
                                VALUES(@Step,@ApplyForId,@RegId,@RecordOfficeId,@OutDate,@ReceiptDate,@DispatchDate,@RefOfDispatch,@NameOfCourierIncharge,@UploadFilePath,@FromRemark,@ToRemark,@FromUnitId,@ToUnitId,@ToUserId,@FromUserId,@FromAspNetUsersId,@ToAspNetUsersId,@IsComplete,@IsActive,@Updatedby,@UpdatedOn,@DispatchModeId)";
                var parameters = new DynamicParameters();
                parameters.Add("@DispatchCardId", dTODispatch.DispatchCardId, DbType.Int32, ParameterDirection.Output);
                parameters.Add("@Step", dTODispatch.Step, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@ApplyForId", dTODispatch.ApplyForId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@RegId", dTODispatch.RegId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@RecordOfficeId", dTODispatch.RecordOfficeId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@OutDate", dTODispatch.OutDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@ReceiptDate", dTODispatch.ReceiptDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@DispatchDate", dTODispatch.DispatchDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@RefOfDispatch", dTODispatch.RefOfDispatch, DbType.String, ParameterDirection.Input, 50);
                parameters.Add("@NameOfCourierIncharge", dTODispatch.NameOfCourierIncharge, DbType.String, ParameterDirection.Input, 50);
                parameters.Add("@UploadFilePath", dTODispatch.UploadFilePath, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@FromRemark", dTODispatch.FromRemark, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@ToRemark", dTODispatch.ToRemark, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@FromUnitId", dTODispatch.FromUnitId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToUnitId", dTODispatch.ToUnitId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToUserId", dTODispatch.ToUserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FromUserId", dTODispatch.FromUserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FromAspNetUsersId", dTODispatch.FromAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToAspNetUsersId", dTODispatch.ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@IsComplete", dTODispatch.IsComplete, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@IsActive", dTODispatch.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", dTODispatch.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", dTODispatch.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@DispatchModeId", dTODispatch.DispatchModeId, DbType.Byte, ParameterDirection.Input);

                using (var connection = _contextDP.CreateConnection())
                {
                    var Id = await connection.QuerySingleAsync<int>(insert, parameters);

                    foreach (var batchRecords in requests.Chunk(5000))
                    {

                        DataTable cardDistribution = DataTableHelper.ToDataTable(batchRecords, "Remarks", "IsValid", "Status");
                        var parameters2 = new DynamicParameters();
                        parameters2.Add("@data", cardDistribution.AsTableValuedParameter("UT_CardDispatchCSV"));
                        parameters2.Add("@DispatchCardId", Id, DbType.Int32, ParameterDirection.Input);
                        parameters2.Add("@StepId", StepId, DbType.Byte, ParameterDirection.Input);
                        response = (await connection.QueryAsync<DTOGenericResponse<string>>("CardDispatchCSVImport",
                                                                                                parameters2,
                                                                                                commandType: CommandType.StoredProcedure
                                   )).FirstOrDefault();
                    }
                    response.Value = "Success";
                    response.Message = Id.ToString();
                }
                response.Result = true;


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetCardMovementHistory");
                response.Message = "";
                response.Result = false;
            }
            return response;
        }


        /// <summary>
        /// Handles the process of dispatching cards into the system. It accepts a list of dispatch card requests, processes them in batches of 5000 records, 
        /// and calls the stored procedure `CardDispatchIn` to insert the data into the database. The method also updates the status of the dispatch card 
        /// by using the provided `StepId` and `ToRemark`.
        /// </summary>
        /// <param name="dTODispatch">A list of <see cref="DTODispatchCardInRequest"/> objects containing dispatch card information to be processed.</param>
        /// <param name="StepId">The ID of the step in the dispatch process that the card is currently at.</param>
        /// <param name="DispatchCardId">The ID of the dispatch card being updated.</param>
        /// <param name="ToRemark">A string containing remarks that will be associated with the dispatch card during the update process.</param>
        /// <returns>
        /// A <see cref="DTOGenericResponse{string}"/> object that indicates the result of the dispatch card processing. 
        /// The `Value` property will be set to "Success" on successful processing. If an error occurs, the `Result` will be set to `false`, 
        /// and the `Message` property will contain the error details.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if any error occurs during the execution of the stored procedure or if there is an issue with the database connection.
        /// The exception will be logged, and the response will indicate a failure.
        /// </exception>
        public async Task<DTOGenericResponse<string>> DispatchCardIn(List<DTODispatchCardInRequest> dTODispatch, byte StepId, int DispatchCardId,string ToRemark)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {

                    foreach (var batchRecords in dTODispatch.Chunk(5000))
                    {

                        DataTable dataTable = DataTableHelper.ToDataTable(batchRecords);
                        var parameters = new DynamicParameters();
                        parameters.Add("@data", dataTable.AsTableValuedParameter("UT_CardDispatchIn"));
                        parameters.Add("@DispatchCardId", DispatchCardId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@StepId", StepId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@ToRemark", ToRemark, DbType.String, ParameterDirection.Input,100);
                        response = (await connection.QueryAsync<DTOGenericResponse<string>>("CardDispatchIn",
                                                                                                parameters,
                                                                                                commandType: CommandType.StoredProcedure
                                   )).FirstOrDefault();
                    }
                    response.Value = "Success";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetCardMovementHistory");
                response.Message = "";
                response.Result = false;
            }
            return response;
        }


        /// <summary>
        /// Retrieves the complete movement history of an I-Card based on the provided request ID. 
        /// This includes steps like card export, printing, loss, distribution, hotlisting, and destruction.
        /// It fetches the movement history data from multiple sources and returns a consolidated list sorted by the reported date.
        /// </summary>
        /// <param name="requestId">The unique identifier of the I-Card request for which the movement history is being fetched.</param>
        /// <returns>
        /// A list of <see cref="DTOCardMovementHistoryResponse"/> objects representing the history of the card's movements. 
        /// The list includes details such as step name, reported by, reported date, and any relevant remarks for each movement step.
        /// The list is sorted by the date the movement was reported.
        /// </returns>
        /// <exception cref="Exception">
        /// Logs an error if any issues occur while fetching the data from the database or processing the movement history.
        /// The error will be logged with the message "BasicDetailDB->GetCardMovementHistory".
        /// </exception>
        public async Task<List<DTOCardMovementHistoryResponse>> GetCardMovementHistory(int requestId)
        {
            var responseList = new List<DTOCardMovementHistoryResponse>();
            try
            {
                var cardStep = _context.TrnStepCounter.Where(step => step.RequestId == requestId).FirstOrDefaultAsync().Result.StepId;

                if (cardStep == (byte)CardStepEnum.Exported || cardStep == (byte)CardStepEnum.Printed || cardStep == (byte)CardStepEnum.CardDistributed)
                {
                    var exported = await (from request in _context.TrnICardRequest.AsNoTracking()
                                          where request.RequestId == requestId && request.CardExportedOn.HasValue
                                          select new DTOCardMovementHistoryResponse
                                          {
                                              StepName = "I-Card Exported",
                                              ReportedBy = "afsac_cell",
                                              ReportedOn = request.CardExportedOn.Value,
                                              Remark = "Card Exported"
                                          }).ToListAsync();
                    var printed = new List<DTOCardMovementHistoryResponse>();
                    var losted = new List<DTOCardMovementHistoryResponse>();
                    var distributed = new List<DTOCardMovementHistoryResponse>();
                    var hotlisted = new List<DTOCardMovementHistoryResponse>();
                    var destroyed = new List<DTOCardMovementHistoryResponse>();

                    if (cardStep > (byte)CardStepEnum.Exported)
                    {
                        printed = await (from request in _context.TrnICardRequest.AsNoTracking()
                                         where request.RequestId == requestId && request.CardPrintedOn.HasValue
                                         select new DTOCardMovementHistoryResponse
                                         {
                                             StepName = "I-Card Printed",
                                             ReportedBy = "afsac_cell",
                                             ReportedOn = request.CardPrintedOn.Value,
                                             Remark = "Card Printed"
                                         }).ToListAsync();
                        losted = await (from lost in _context.TrnLostCards.AsNoTracking()
                                            //join dist in _context.TrnDistributeCards.AsNoTracking()
                                            //    on lost.RequestId equals dist.RequestId into distGroup
                                            //from dist in distGroup.DefaultIfEmpty()
                                        join user in _context.Users.AsNoTracking()
                                            on lost.Updatedby equals user.Id
                                        join profile in _context.UserProfile.AsNoTracking()
                                            on lost.UpdatedbyUserId equals profile.UserId
                                        join rank in _context.MRank.AsNoTracking()
                                           on profile.RankId equals rank.RankId
                                        //where lost.UpdatedOn != null && (dist == null || lost.UpdatedOn < dist.UpdatedOn) && lost.RequestId == requestId
                                        where lost.RequestId == requestId
                                        select new DTOCardMovementHistoryResponse
                                        {
                                            StepName = "I-Card Lost",
                                            ReportedBy = $"({user.DomainId}) {rank.RankAbbreviation} {profile.Name}",
                                            ReportedOn = lost.LostOn.Value,
                                            Remark = lost.Remark
                                        }).ToListAsync();
                    }

                    if (cardStep == (byte)CardStepEnum.CardDistributed)
                    {
                        distributed = await (from dist in _context.TrnDistributeCards.AsNoTracking()
                                             join user in _context.Users.AsNoTracking()
                                                on dist.Updatedby equals user.Id
                                             join profile in _context.UserProfile.AsNoTracking()
                                                on dist.UpdatedbyUserId equals profile.UserId
                                             join rank in _context.MRank.AsNoTracking()
                                                on profile.RankId equals rank.RankId
                                             where dist.RequestId == requestId
                                             select new DTOCardMovementHistoryResponse
                                             {
                                                 StepName = "I-Card Distributed",
                                                 ReportedBy = $"({user.DomainId}) {rank.RankAbbreviation} {profile.Name}",
                                                 ReportedOn = dist.DistributedOn.Value,
                                                 Remark = dist.Remark
                                             }).ToListAsync();
                        hotlisted = await (from hotlist in _context.TrnHotlistCards.AsNoTracking()
                                           join user in _context.Users.AsNoTracking()
                                              on hotlist.Updatedby equals user.Id
                                           join profile in _context.UserProfile.AsNoTracking()
                                              on hotlist.UpdatedbyUserId equals profile.UserId
                                           join rank in _context.MRank.AsNoTracking()
                                              on profile.RankId equals rank.RankId
                                           where hotlist.RequestId == requestId
                                           select new DTOCardMovementHistoryResponse
                                           {
                                               StepName = "I-Card Holtist",
                                               ReportedBy = $"({user.DomainId}) {rank.RankAbbreviation} {profile.Name}",
                                               ReportedOn = hotlist.UpdatedOn.Value,
                                               Remark = hotlist.Remark
                                           }).ToListAsync();
                    }
                    destroyed = await (from dest in _context.TrnDestructionCards.AsNoTracking()
                                       join user in _context.Users.AsNoTracking()
                                          on dest.Updatedby equals user.Id
                                       join profile in _context.UserProfile.AsNoTracking()
                                          on dest.UpdatedbyUserId equals profile.UserId
                                       join rank in _context.MRank.AsNoTracking()
                                          on profile.RankId equals rank.RankId
                                       where dest.RequestId == requestId
                                       select new DTOCardMovementHistoryResponse
                                       {
                                           StepName = "I-Card Destruction",
                                           ReportedBy = $"({user.DomainId}) {rank.RankAbbreviation} {profile.Name}",
                                           ReportedOn = dest.DestructedOn.Value,
                                           Remark = dest.Remark
                                       }).ToListAsync();

                    responseList = exported
                            .Concat(printed)
                            .Concat(losted)
                            .Concat(distributed)
                            .Concat(hotlisted)
                            .Concat(destroyed)
                            .OrderBy(x => x.ReportedOn)
                            .ToList();

                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "BasicDetailDB->GetCardMovementHistory");
            }

            return responseList;
        }


        /// <summary>
        /// Updates the status of an I-Card request in the database based on the provided request ID and status.
        /// This method executes an UPDATE SQL query to change the `StatusId` for a specific `RequestId` in the `TrnICardRequest` table.
        /// </summary>
        /// <param name="requestId">The unique identifier of the I-Card request that needs to be updated.</param>
        /// <param name="status">The new status value to be assigned to the I-Card request. This value corresponds to the `StatusId` field in the database.</param>
        /// <exception cref="Exception">
        /// Logs any errors that occur during the update process. The exception is captured and logged with the message "BasicDetailDB->UpdateCardStatus".
        /// </exception>
        public async Task UpdateCardStatus(int requestId, byte status)
        {
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    string query = "UPDATE TrnICardRequest SET StatusId = @Status WHERE RequestId = @RequestId";

                    await connection.ExecuteAsync(query, new { Status = status, RequestId = requestId });
                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "BasicDetailDB->UpdateCardStatus");
            }
        }

        /// <summary>
        /// Checks the status of a card before distribution, based on the given request ID. This method determines whether the card
        /// is eligible for distribution by checking its type and associated status in the database. It also handles specific cases
        /// such as "Lost" or "Destruction" for different card types and checks the related records in the `TrnLostCards` and 
        /// `TrnDestructionCards` tables.
        /// </summary>
        /// <param name="requestId">The unique identifier for the I-Card request to check.</param>
        /// <returns>
        /// A <see cref="DTOUploadChipAndSerialResponse"/> containing the result and message. The result indicates whether the 
        /// card is eligible for distribution (1 for eligible, 0 for not eligible), and the message provides additional context 
        /// (such as "Lost" or "Destruction") based on the card type.
        /// </returns>
        /// <exception cref="Exception">
        /// Logs any exceptions encountered during the execution of the method, such as database query issues.
        /// </exception>
        public async Task<DTOUploadChipAndSerialResponse> CheckBeforeDistribution(int requestId)
        {
            #region Old Code
            //DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();
            //try
            //{
            //    string query = @$"SELECT CASE
            //                WHEN currentReq.TypeId = 1 THEN 1

            //                WHEN currentReq.TypeId = 5 AND EXISTS (
            //                    SELECT 1 FROM TrnLostCards lc
            //                    WHERE lc.RequestId = (
            //                        SELECT TOP 1 prevReq.RequestId
            //                        FROM TrnICardRequest prevReq
            //                        WHERE prevReq.BasicDetailId = currentReq.BasicDetailId
            //                          AND prevReq.RequestId < currentReq.RequestId
            //                          AND prevReq.StatusId != 1
            //                        ORDER BY prevReq.RequestId DESC
            //                    ) AND lc.IsActive = 1
            //                ) THEN 1

            //                WHEN currentReq.TypeId IN (2, 3, 4) AND EXISTS (
            //                    SELECT 1 FROM TrnDestructionCards dc
            //                    WHERE dc.RequestId = (
            //                        SELECT TOP 1 prevReq.RequestId
            //                        FROM TrnICardRequest prevReq
            //                        WHERE prevReq.BasicDetailId = currentReq.BasicDetailId
            //                          AND prevReq.RequestId < currentReq.RequestId
            //                          AND prevReq.StatusId != 1
            //                        ORDER BY prevReq.RequestId DESC
            //                    ) AND dc.IsActive = 1
            //                ) THEN 1

            //                ELSE 0
            //            END AS Result,case currentReq.TypeId when 1 then '' when 5 then 'Lost' else 'Destruction' end as Message
            //            FROM TrnICardRequest currentReq
            //            WHERE currentReq.RequestId = @RequestId";

            //    using (var connection = _contextDP.CreateConnection())
            //    {
            //        var list = await connection.QueryAsync<DTOUploadChipAndSerialResponse>(query, new { RequestId = requestId });
            //        response = list.FirstOrDefault();
            //    }
            //}
            //catch (Exception ee)
            //{
            //    _logger.LogError(1001, ee, "BasicDetailDB->UpdateCardStatus");
            //}
            //return response;
            #endregion
            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();
            try
            {
                string query = @"SELECT CASE
                                WHEN currentReq.TypeId = 1 THEN 1

                                WHEN currentReq.TypeId = 5 AND EXISTS (
                                    SELECT 1 FROM TrnLostCards lc
                                    WHERE lc.RequestId = (
                                        SELECT TIR1.RequestId
									    FROM TrnICardRequest TIR1
									    JOIN BasicDetails BD ON TIR1.BasicDetailId = BD.PreviousBasicDetailId
									    JOIN TrnICardRequest TIR2 ON BD.BasicDetailId = TIR2.BasicDetailId
									    WHERE TIR2.RequestId = currentReq.RequestId
                                    ) AND lc.IsActive = 1
                                ) THEN 1

                                WHEN currentReq.TypeId IN (2, 3, 4) AND EXISTS (
                                    SELECT 1 FROM TrnDestructionCards dc
                                    WHERE dc.RequestId = (
                                        SELECT TIR1.RequestId
									    FROM TrnICardRequest TIR1
									    JOIN BasicDetails BD ON TIR1.BasicDetailId = BD.PreviousBasicDetailId
									    JOIN TrnICardRequest TIR2 ON BD.BasicDetailId = TIR2.BasicDetailId
									    WHERE TIR2.RequestId = currentReq.RequestId
                                    ) AND dc.IsActive = 1
                                ) THEN 1

                                ELSE 0
                            END AS Result,
						    case 
						    WHEN currentReq.TypeId = 1 then '' 
						    WHEN currentReq.TypeId IN (2, 3, 4) THEN 'Destruction' 
						    WHEN currentReq.TypeId = 5 THEN 'Lost'
						    ELSE ''
						    END as Message
                            FROM TrnICardRequest currentReq
                            WHERE currentReq.RequestId = @RequestId;";

                using (var connection = _contextDP.CreateConnection())
                {
                   var list = await connection.QueryAsync<DTOUploadChipAndSerialResponse>(query, new {RequestId = requestId });
                    response = list.FirstOrDefault();
                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "BasicDetailDB->UpdateCardStatus");
            }
            return response;
        }
    }
}
