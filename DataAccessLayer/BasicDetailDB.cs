using Azure;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Healpers;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using EntityFramework.Exceptions.Common;  
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Data;
using Azure.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DataTransferObject.Constants;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using static Dapper.SqlMapper;
using DataTransferObject.Response.User;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DataTransferObject.Domain.Identitytable;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public class BasicDetailDB : GenericRepositoryDL<BasicDetail>, IBasicDetailDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<BasicDetailDB> _logger;
        private readonly IServiceProvider _serviceProvider;
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
        public async Task<DTODataTablesResponse<DTODispatchCardStatusResponse>> GetDispatchCardStatusListForDialog(DTODataTablesRequestForCardStatusList dTO, byte ClaimValue)
        {
            string query = "";
            string wherequery = "";
            string searchFilter = "";
            byte PendingStepId = 0;
            byte DispatchStepId = 0;
            byte finalValue=0;
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection;

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
            if (ClaimValue == 1)
            {
                PendingStepId = 6; // Pending Step for AFSAC
                DispatchStepId = 11; // Dispatch Step for AFSAC
                query = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,mrec.Abbreviation as RecordOfficeName,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo,
                        CASE 
                            WHEN stepc.StepId = 6 THEN 'Pending' 
                            WHEN stepc.StepId >= 11 THEN 'Dispatch Out'
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
                            (stepc.StepId=6 OR stepc.StepId>=11)";
            }
            else if (ClaimValue == 2 || ClaimValue == 3)
            {
                PendingStepId = 12; // Pending Step for RO / Regiment
                DispatchStepId = 13; // Dispatch Step for RO / Regiment
                query = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,mrec.Abbreviation as RecordOfficeName,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo,
                        CASE 
                            WHEN stepc.StepId = 12 THEN 'Pending' 
                            WHEN stepc.StepId >= 13 THEN 'Dispatch Out'
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
                            (stepc.StepId=12 OR stepc.StepId>=13)";
            }
            else
            {
                PendingStepId = 14; // Pending Step for Unit
                DispatchStepId = 15; // Dispatch Step for Unit
                query = @"req.RequestId,stepc.StepId,mappl.Name as ApplyFor,mappl.ApplyForId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,mrec.Abbreviation as RecordOfficeName,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo,
                        CASE 
                            WHEN stepc.StepId = 14 THEN 'Pending' 
                            WHEN stepc.StepId = 15 THEN 'Card Distribute'
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
                            (stepc.StepId=14 OR stepc.StepId=15)";
            }
            if (!string.IsNullOrWhiteSpace(dTO.SearchField) && !string.IsNullOrWhiteSpace(dTO.SearchText))
            {
                string safeField = dTO.SearchField.Trim().ToLower();
                switch (safeField)
                {
                    case "categery":
                        searchFilter = @"AND mappl.Name=@SearchText";
                        break;
                    case "requestid":
                        searchFilter = @"AND req.RequestId=@SearchText";
                        break;
                    case "serviceno":
                        searchFilter = @"AND basi.ServiceNo LIKE '%' + @SearchText + '%'";
                        break;
                    case "susno":
                        searchFilter = @"AND concat(munit.Sus_no, munit.Suffix) LIKE '%' + @SearchText + '%'";
                        break;
                    case "regimentalname":
                        searchFilter = @"AND regi.Abbreviation LIKE '%' + @SearchText + '%'";
                        break;
                    case "recordofficename":
                        searchFilter = @"AND mrec.Abbreviation LIKE '%' + @SearchText + '%'";
                        break;
                    case "chipno":
                        searchFilter = @"AND req.ChipNo LIKE '%' + @SearchText + '%'";
                        break;
                    case "cardserialno":
                        searchFilter = @"AND req.CardSerialNo LIKE '%' + @SearchText + '%'";
                        break;
                    case "status":
                        finalValue = ((dTO.searchValue?.Trim().ToLower() == "pending" || dTO.searchValue?.Trim().ToLower() == "card distribute") ? PendingStepId : DispatchStepId);
                        searchFilter = @"AND stepc.StepId = @FinalStepId";
                        break;
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
                var multiQuery = query = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query} {wherequery} {searchFilter}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    dTO.SearchText = string.IsNullOrEmpty(dTO.SearchText) ? string.Empty : dTO.SearchText.Trim();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchText", dTO.SearchText, DbType.String, ParameterDirection.Input);
                    parameters.Add("@FinalStepId", finalValue, DbType.Byte, ParameterDirection.Input);
                    //parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTODispatchCardStatusResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTODispatchCardStatusResponse>
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

        public async Task<DTODataTablesResponse<DTODispatchCardStatusResponse>> GetDispatchCardStatusListForExport(byte ClaimValue, DTOExportDispatch Data)
        {
            string query = "";
            string wherequery = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            

           
            if (ClaimValue == 1)
            {
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

        public async Task<DTODataTablesResponse<DTOCardDispatchDialogResponse>> GetDispatchCardDataForDialog(DTODataTablesRequestForCardDispatchDialog dTO)
        {
            string query = "";
            string wherequery = "";
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
            query = @"dcm.DispatchCardMappingId,req.RequestId,basi.NameAsPerRecord,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation as RegimentalName,mrec.Abbreviation as RecordOfficeName,req.ChipNo,req.CardSerialNo,munit.Abbreviation as UnitAbbreviation,concat(munit.Sus_no,munit.Suffix) as SUSNo from TrnDispatchCardMapping dcm
                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId
                    INNER JOIN TrnICardRequest req on dcm.ChipNo=req.ChipNo
                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                    INNER JOIN MUnit munit on unit.UnitId = munit.UnitId
                    LEFT JOIN MRegimental regi on regi.RegId=basi.RegimentalId
                    LEFT JOIN MRecordOffice mrec on dcard.RecordOfficeId = mrec.RecordOfficeId";
            wherequery = @"WHERE
                            dcm.DispatchCardId=@DispatchCardId
                            AND (
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
                var multiQuery = query = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query} {wherequery}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    dTO.searchValue = string.IsNullOrEmpty(dTO.searchValue) ? string.Empty : dTO.searchValue.Trim();
                    var parameters = new DynamicParameters();
                    parameters.Add("@DispatchCardId", dTO.DispatchCardId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTOCardDispatchDialogResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTOCardDispatchDialogResponse>
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
                List<DTOCardDispatchDialogResponse> dTOCards = new List<DTOCardDispatchDialogResponse>();
                var responseData = new DTODataTablesResponse<DTOCardDispatchDialogResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOCards
                };
                return responseData;
            }
        }
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
                    ["LotNo"] = "dcard.LotNo",
                    ["NameOfCourierIncharge"] = "dcard.NameOfCourierIncharge",
                    ["ToServiceNo"] = "toUp.ArmyNo",
                    ["DispatchDate"] = "dcard.DispatchDate",
                    ["FromRemark"] = "dcard.FromRemark",
                    ["ReceiptDate"] = "dcard.ReceiptDate",
                    ["ToRemark"] = "dcard.ToRemark"
                };
                selectFields = @"dcard.DispatchCardId,dcard.Step,mappl.Name as ApplyFor,mappl.ApplyForId,regi.Abbreviation RegimentalName,mrec.Name as RecordOfficeName,dcard.OutDate,dcard.ReceiptDate,dcard.DispatchDate,mdis.Description as DispatchMode,dcard.RefOfDispatch,dcard.LotNo,dcard.NameOfCourierIncharge,dcard.UploadFilePath,dcard.FromRemark,dcard.ToRemark,fromMuni.Abbreviation as FromUnit,toMuni.Abbreviation as ToUnit,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID,dcard.IsComplete,dcard.IsActive,dcard.UpdatedOn";
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
                                toUp.ArmyNo LIKE '%' + @SearchTerm + '%' OR
                                dcard.LotNo LIKE '%' + @SearchTerm + '%' OR
                                dcard.NameOfCourierIncharge LIKE '%' + @SearchTerm + '%' OR
                                mappl.Name LIKE '%' + @SearchTerm + '%'
                                )";
                if (!string.IsNullOrEmpty(dTO.FilterApplyFor))
                    whereClause += " AND mappl.Name = @FilterApplyFor";
            }
            else if (dTO.ClaimValue == 2)
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ApplyFor"] = "mappl.Name",
                    ["LotNo"] = "dcard.LotNo",
                    ["NameOfCourierIncharge"] = "dcard.NameOfCourierIncharge",
                    ["ToServiceNo"] = "toUp.ArmyNo",
                    ["DispatchDate"] = "dcard.DispatchDate",
                    ["FromRemark"] = "dcard.FromRemark",
                    ["ReceiptDate"] = "dcard.ReceiptDate",
                    ["ToRemark"] = "dcard.ToRemark"
                };
                selectFields = @"dcard.DispatchCardId,dcard.Step,mappl.Name as ApplyFor,mappl.ApplyForId,mrec.Name as RecordOfficeName,dcard.OutDate,dcard.ReceiptDate,dcard.DispatchDate,mdis.Description as DispatchMode,dcard.RefOfDispatch,dcard.LotNo,dcard.NameOfCourierIncharge,dcard.UploadFilePath,dcard.FromRemark,dcard.ToRemark,fromMuni.Abbreviation as FromUnit,toMuni.Abbreviation as ToUnit,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID,dcard.IsComplete,dcard.IsActive,dcard.UpdatedOn";
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
                                    toUp.ArmyNo LIKE '%' + @SearchTerm + '%' OR
                                    dcard.LotNo LIKE '%' + @SearchTerm + '%' OR
                                    dcard.NameOfCourierIncharge LIKE '%' + @SearchTerm + '%' OR
                                    mappl.Name LIKE '%' + @SearchTerm + '%'
                                )";
                if (!string.IsNullOrEmpty(dTO.FilterApplyFor))
                    whereClause += " AND mappl.Name = @FilterApplyFor";
            }
            else if (dTO.ClaimValue == 3)
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ApplyFor"] = "mappl.Name",
                    ["LotNo"] = "dcard.LotNo",
                    ["NameOfCourierIncharge"] = "dcard.NameOfCourierIncharge",
                    ["ToServiceNo"] = "toUp.ArmyNo",
                    ["DispatchDate"] = "dcard.DispatchDate",
                    ["FromRemark"] = "dcard.FromRemark",
                    ["ReceiptDate"] = "dcard.ReceiptDate",
                    ["ToRemark"] = "dcard.ToRemark"
                };
                selectFields = @"dcard.DispatchCardId,dcard.Step,mappl.Name as ApplyFor,mappl.ApplyForId,regi.Abbreviation RegimentalName,dcard.OutDate,dcard.ReceiptDate,dcard.DispatchDate,mdis.Description as DispatchMode,dcard.RefOfDispatch,dcard.LotNo,dcard.NameOfCourierIncharge,dcard.UploadFilePath,dcard.FromRemark,dcard.ToRemark,fromMuni.Abbreviation as FromUnit,toMuni.Abbreviation as ToUnit,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID,dcard.IsComplete,dcard.IsActive,dcard.UpdatedOn";
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
                                toUp.ArmyNo LIKE '%' + @SearchTerm + '%' OR
                                dcard.LotNo LIKE '%' + @SearchTerm + '%' OR
                                dcard.NameOfCourierIncharge LIKE '%' + @SearchTerm + '%' OR
                                mappl.Name LIKE '%' + @SearchTerm + '%'
                                )";
                if (!string.IsNullOrEmpty(dTO.FilterApplyFor))
                    whereClause += " AND mappl.Name = @FilterApplyFor";
            }
            else
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ApplyFor"] = "mappl.Name",
                    ["LotNo"] = "dcard.LotNo",
                    ["NameOfCourierIncharge"] = "dcard.NameOfCourierIncharge",
                    ["ToServiceNo"] = "toUp.ArmyNo",
                    ["DispatchDate"] = "dcard.DispatchDate",
                    ["FromRemark"] = "dcard.FromRemark",
                    ["ReceiptDate"] = "dcard.ReceiptDate",
                    ["ToRemark"] = "dcard.ToRemark"
                };
                selectFields = @"dcard.DispatchCardId,dcard.Step,mappl.Name as ApplyFor,mappl.ApplyForId,regi.Abbreviation RegimentalName,mrec.Name as RecordOfficeName,dcard.OutDate,dcard.ReceiptDate,dcard.DispatchDate,mdis.Description as DispatchMode,dcard.RefOfDispatch,dcard.LotNo,dcard.NameOfCourierIncharge,dcard.UploadFilePath,dcard.FromRemark,dcard.ToRemark,fromMuni.Abbreviation as FromUnit,toMuni.Abbreviation as ToUnit,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID,dcard.IsComplete,dcard.IsActive,dcard.UpdatedOn";
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
                                    toUp.ArmyNo LIKE '%' + @SearchTerm + '%' OR
                                    dcard.LotNo LIKE '%' + @SearchTerm + '%' OR
                                    dcard.NameOfCourierIncharge LIKE '%' + @SearchTerm + '%' OR
                                    mappl.Name LIKE '%' + @SearchTerm + '%' OR
                                    mrec.Name LIKE '%' + @SearchTerm + '%'
                                )";
                if (!string.IsNullOrEmpty(dTO.FilterApplyFor))
                    whereClause += " AND mappl.Name = @FilterApplyFor";
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
                    parameters.Add("@FilterApplyFor", dTO.FilterApplyFor ?? "", DbType.String);

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
        public async Task<List<DTOCardDispatchCheckRequest>> CardDispatchCSVCheck(List<DTOCardDispatchCheckRequest> requests, byte ClaimValue, DTODispatchOutRequest dTO)
        {
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

                    foreach (var batchRecords in requests.Chunk(5000))
                    {
                        var resultInChunks = await Task.Run(() =>
                        {
                            if (ClaimValue == 1)
                            {
                                if (dTO.ApplyForId == 1)
                                {
                                    return (from record in batchRecords
                                            join chipNoMatch in context.TrnICardRequest on record.ChipNo equals chipNoMatch.ChipNo into chipNoJoin
                                            from chipNoExists in chipNoJoin.DefaultIfEmpty()
                                            join stepStatus in context.TrnStepCounter on new { RequestId = chipNoExists?.RequestId ?? 0, StepId } equals new { stepStatus.RequestId, stepStatus.StepId } into stepStatusJoin
                                            from stepStatus in stepStatusJoin.DefaultIfEmpty()
                                            select new DTOCardDispatchCheckRequest
                                            {
                                                ChipNo = record.ChipNo,
                                                RequestId = chipNoExists?.RequestId ?? 0,
                                                IsValid = chipNoExists != null && chipNoExists.RecordOfficeId == dTO.RecordOfficeId && stepStatus != null,
                                                Status = chipNoExists != null && chipNoExists.RecordOfficeId == dTO.RecordOfficeId && stepStatus != null ? "Valid" : "DbInvalid",
                                                Remarks = (chipNoExists == null ? "ChipNo not exists; " : "") +
                                                          (chipNoExists != null && chipNoExists.RecordOfficeId != dTO.RecordOfficeId ? "ChipNo not Valid match to RecordOffice; " : "") +
                                                          (chipNoExists != null && stepStatus == null ? Remarks : "")
                                            }).ToList();
                                }
                                else
                                {
                                    return (from record in batchRecords
                                            join chipNoMatch in context.TrnICardRequest on record.ChipNo equals chipNoMatch.ChipNo into chipNoJoin
                                            from chipNoExists in chipNoJoin.DefaultIfEmpty()
                                            join bdMatch in context.BasicDetails on new { BasicDetailId = chipNoExists?.BasicDetailId ?? 0, RegimentalId = dTO.RegId } equals new { bdMatch.BasicDetailId, bdMatch.RegimentalId } into bdMatchJoin
                                            from bdMatch in bdMatchJoin.DefaultIfEmpty()
                                            join stepStatus in context.TrnStepCounter on new { RequestId = chipNoExists?.RequestId ?? 0, StepId } equals new { stepStatus.RequestId, stepStatus.StepId } into stepStatusJoin
                                            from stepStatus in stepStatusJoin.DefaultIfEmpty()
                                            select new DTOCardDispatchCheckRequest
                                            {
                                                ChipNo = record.ChipNo,
                                                RequestId = chipNoExists?.RequestId ?? 0,
                                                IsValid = chipNoExists != null && bdMatch != null && stepStatus != null,
                                                Status = chipNoExists != null && bdMatch != null && stepStatus != null ? "Valid" : "DbInvalid",
                                                Remarks = (chipNoExists == null ? "ChipNo not exists; " : "") +
                                                          (chipNoExists != null && bdMatch == null ? "ChipNo not Valid match to Regiment; " : "") +
                                                          (chipNoExists != null && stepStatus == null ? Remarks : "")
                                            }).ToList();
                                }
                            }
                            else
                            {
                                return (from record in batchRecords
                                        join chipNoMatch in context.TrnICardRequest on record.ChipNo equals chipNoMatch.ChipNo into chipNoJoin
                                        from chipNoExists in chipNoJoin.DefaultIfEmpty()
                                        join bdMatch in context.BasicDetails on new { BasicDetailId = chipNoExists?.BasicDetailId ?? 0, UnitId = dTO.ToUnitId } equals new { bdMatch.BasicDetailId, bdMatch.UnitId } into bdMatchJoin
                                        from bdMatch in bdMatchJoin.DefaultIfEmpty()
                                        join stepStatus in context.TrnStepCounter on new { RequestId = chipNoExists?.RequestId ?? 0, StepId } equals new { stepStatus.RequestId, stepStatus.StepId } into stepStatusJoin
                                        from stepStatus in stepStatusJoin.DefaultIfEmpty()
                                        select new DTOCardDispatchCheckRequest
                                        {
                                            ChipNo = record.ChipNo,
                                            RequestId = chipNoExists?.RequestId ?? 0,
                                            IsValid = chipNoExists != null && bdMatch != null && stepStatus != null,
                                            Status = chipNoExists != null && bdMatch != null && stepStatus != null ? "Valid" : "DbInvalid",
                                            Remarks = (chipNoExists == null ? "ChipNo not exists; " : "") +
                                                      (chipNoExists != null && bdMatch == null ? "ChipNo not Valid match to Unit; " : "") +
                                                      (chipNoExists != null && stepStatus == null ? Remarks : "")
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
        public async Task<DTOGenericResponse<List<DTOMasterResponse>>> GetddlRecordRegiment(byte CategeryId, byte ClaimValue,int TDMId,int UnitId)
        {
            List<DTOMasterResponse> ret = new List<DTOMasterResponse>();
            DTOGenericResponse<List<DTOMasterResponse>> response = new DTOGenericResponse<List<DTOMasterResponse>>();
            string query=string.Empty;
            if (ClaimValue == 1)
            {
                if(CategeryId == 1)
                {
                    query = @"Select oro.RecordOfficeId as Id,mrec.Name as Name from OROMapping oro
                            inner join MRecordOffice mrec on oro.RecordOfficeId = mrec.RecordOfficeId";
                }
                else if(CategeryId == 2)
                {
                    query = @"Select RegId as Id, Name  from MRegimental";
                }
            }
            else if (ClaimValue == 2) 
            {
                query = @"Select oro.RecordOfficeId as Id,mrec.Name from OROMapping oro
                        inner join MRecordOffice mrec on oro.RecordOfficeId = mrec.RecordOfficeId WHERE oro.TDMId=@TDMId";
            }
            else if (ClaimValue == 3)
            {
                query = @"Select RegId as Id, Name  from MRegimental WHERE UnitId=@UnitId";
            }
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                     ret = (await connection.QueryAsync<DTOMasterResponse>(query,new { TDMId , UnitId })).ToList();
                }
                response.Result = true;  // Operation successful
                response.Message = "Data retrieved successfully.";
                response.Value = ret;
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
        public async Task<bool> CheckArmyNO(string ArmyNo)
        {
            return await _context.BasicDetails.AnyAsync(x => x.ServiceNo == ArmyNo);
        }
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
        public async Task<List<DTOICardRequestHoldResponse>?> GetAllICardRequestHold()
        {
            string query = "";
            query = " SELECT munit.UnitName,B.FName,B.LName,B.ServiceNo,trnicrd.RequestId,Afor.Name ApplyFor,ran.RankAbbreviation RankName,thold.ICardHoldId,thold.HoldReason,thold.UnHoldReason,thold.IsHold,u.DomainId,u.UpdatedOn " +
                    " FROM MTrnICardHold thold " +
                    " inner join AspNetUsers u on u.Id = thold.Updatedby " +
                    " inner join TrnICardRequest trnicrd on trnicrd.RequestId = thold.RequestId " +
                    " inner join BasicDetails B on B.BasicDetailId = trnicrd.BasicDetailId " +
                    " inner join MRank ran on ran.RankId=B.RankId " +
                    " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                    " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                    " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId ";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOICardRequestHoldResponse>(query);
                    return await Task.FromResult(allrecord.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllICardRequestHold");
                return null;
            }
        }
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

                        var insertTrnICardRequest = " INSERT INTO TrnICardRequest (BasicDetailId, TypeId, RegistrationId, TrnDomainMappingId, TrackingId, IsActive, Updatedby, UpdatedOn, StatusId, CardSerialNo, ChipNo)" +
                                                    " OUTPUT INSERTED.RequestId " +
                                                    " VALUES (@BasicDetailId, @TypeId, @RegistrationId, @TrnDomainMappingId, @TrackingId, @IsActive, @Updatedby, @UpdatedOn, @StatusId, @CardSerialNo, @ChipNo);";
                        var parametersTrnICardRequest = new DynamicParameters();
                        //parametersTrnICardRequest.Add("@RequestId", mTrnICardRequest.RequestId, DbType.Int32, ParameterDirection.Output);
                        parametersTrnICardRequest.Add("@BasicDetailId", mTrnICardRequest.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TypeId", mTrnICardRequest.TypeId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@RegistrationId", mTrnICardRequest.RegistrationId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TrnDomainMappingId", mTrnICardRequest.TrnDomainMappingId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TrackingId", mTrnICardRequest.TrackingId, DbType.Int64, ParameterDirection.Input);
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

                        var updateTrnICardRequest = " UPDATE TrnICardRequest SET BasicDetailId=@BasicDetailId, TypeId=@TypeId, RegistrationId=@RegistrationId, TrnDomainMappingId=@TrnDomainMappingId, TrackingId=@TrackingId, IsActive=@IsActive, Updatedby=@Updatedby, UpdatedOn=@UpdatedOn, StatusId=@StatusId, CardSerialNo=@CardSerialNo, ChipNo=@ChipNo,RecordOfficeId=@RecordOfficeId WHERE RequestId=@RequestId";
                        var parametersTrnICardRequest = new DynamicParameters();
                        parametersTrnICardRequest.Add("@RequestId", mTrnICardRequest.RequestId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@BasicDetailId", mTrnICardRequest.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TypeId", mTrnICardRequest.TypeId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@RegistrationId", mTrnICardRequest.RegistrationId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TrnDomainMappingId", mTrnICardRequest.TrnDomainMappingId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TrackingId", mTrnICardRequest.TrackingId, DbType.Int64, ParameterDirection.Input);
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
        public async Task<DTOBasicDetailForParitalViewResponse> GetBasicDetailForParitalViewByRequestId(int RequestId)
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

                    return ret.FirstOrDefault() ?? new DTOBasicDetailForParitalViewResponse();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailForParitalViewByRequestId");
                return new DTOBasicDetailForParitalViewResponse();
            }
        }
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
                ["TrackingId"] = "TrackingId",
                ["ApplyFor"] = "ApplyFor"
            };

            var sortOrder = dTO.sortDirection;

            if (dTO.stepcount == 0)//////For all record
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.Name AS ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId,Afor.Name AS ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
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
                
                wherequery = @"WHERE ( (@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm))";
                
            }
            else if (dTO.stepcount == 1)//////For Draft
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.Name AS ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId,Afor.Name AS ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
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
                
                wherequery = @"WHERE ( (@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm))";

            }

            else if (dTO.stepcount == 777)//////For Completed   
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.Name AS ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId,Afor.Name AS ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd 
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

                wherequery = @"WHERE ( (@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 888)//////For Submitted
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.Name AS ICardType,trnicrd.RequestId,Afor.Name AS ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
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

                wherequery = @"WHERE  ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 5)
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.TypeId,ty.name AS ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId, ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId, Afor.Name AS ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
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

                wherequery = @"where ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 2 || dTO.stepcount == 3 || dTO.stepcount == 4 || dTO.stepcount == 6)//IO
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.TypeId,ty.name AS ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId, ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId ,Afor.Name AS ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
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

                wherequery = @"where (@SearchTerm IS NULL OR B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm)";

            }
            else if (dTO.stepcount == 7 || dTO.stepcount == 8 || dTO.stepcount == 9 || dTO.stepcount == 10)//Reject From IO
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.TypeId,ty.name AS ICardType,trnicrd.RequestId, ISNULL(fwd.TrnFwdId,0) AS IsTrnFwdId,ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId, Afor.Name AS ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
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
                
                wherequery = @"where (@SearchTerm IS NULL OR B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm)";

            }
            else if (dTO.stepcount == 999)//Reject From IO,MI11 and HQ 54
            {
                query = @"trnicrd.RegistrationId AS RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId AS StepCounter,C.Id AS StepId,ty.TypeId,ty.name AS ICardType,trnicrd.RequestId, ISNULL(fwd.FwdStatusId,0) AS IsFwdStatusId ,Afor.Name AS ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation AS RankName,ISNULL(Postout.Id,0) AS IsPosting FROM TrnICardRequest trnicrd
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

                wherequery = @"where (@SearchTerm IS NULL OR B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm)";

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
                                         TrackingId = e.TrackingId,
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
        public async Task<DTODataTablesResponse<DTOBasicDetailIndexResponse>> GetALLBasicDetail(DTODataTablesRequestFor_BasicDetails_Index dTO) //int UserId, int stepcount, int TypeId, int applyForId
        {
            string query = "";
            string wherequery = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection;
            
            if (dTO.stepcount == 0 || dTO.stepcount == 1)//////For Fwd Record
            {
                query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName  FROM BasicDetails B 
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
                
                wherequery = @"WHERE map.AspNetUsersId = @UserId AND ( (@SearchTerm IS NULL) OR (ServiceNo LIKE @SearchTerm OR TrackingId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 2 || dTO.stepcount == 3 || dTO.stepcount == 4 || dTO.stepcount == 5 || dTO.stepcount == 6)//IO
            {
                if (dTO.TypeId == 1)///For Icard Submit
                {
                    query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName FROM BasicDetails B
                            inner join MRank ran on ran.RankId=B.RankId 
                            inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                            inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                            inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId and trnicrd.StatusId=1
                            inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId)
                            inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId = @stepcount
                            inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                            inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId  and fwd.TypeId=@stepcount  
                            inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";
                    
                    wherequery = @"WHERE ( (@SearchTerm IS NULL) OR (ServiceNo LIKE @SearchTerm OR TrackingId LIKE @SearchTerm))";

                }
                else if (dTO.stepcount == 3 && dTO.TypeId == 2 && dTO.applyForId == 2) //// For For Action
                {
                    query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName FROM BasicDetails B
                            inner join MRank ran on ran.RankId=B.RankId
                            inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                            inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                            inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId and trnicrd.StatusId=1
                            inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId)
                            inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId and C.StepId = @stepcount
                            inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                            inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and fwd.IsComplete=0  and fwd.TypeId=@stepcount";

                    wherequery = @"WHERE ( (@SearchTerm IS NULL) OR (ServiceNo LIKE @SearchTerm OR TrackingId LIKE @SearchTerm))";

                }
                else if (dTO.TypeId == 2) //// For For Action
                {
                    query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName FROM TrnICardRequest trnicrd
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

                    wherequery = @"WHERE trnicrd.StatusId = 1 AND( (@SearchTerm IS NULL) OR (ServiceNo LIKE @SearchTerm OR TrackingId LIKE @SearchTerm)) AND (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";

                }
                else if (dTO.TypeId == 3 && dTO.stepcount == 3)
                {
                    query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName FROM BasicDetails B
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

                    wherequery = @"WHERE ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm)) AND (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";

                }
                else if (dTO.TypeId == 3 && dTO.stepcount == 4)
                {
                    query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName FROM BasicDetails B
                            inner join MRank ran on ran.RankId=B.RankId
                            inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                            inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                            inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId
                            inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                            inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                            inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                            inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.FwdStatusId=2 and fwd.TypeId=4 
                            inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";
                    
                    wherequery = @"WHERE ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm)) AND (@applyForId IS NULL OR Afor.ApplyForId = @applyForId)";

                }
                else if (dTO.stepcount == 5 || dTO.stepcount == 6)///for exported data
                {
                    query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName FROM TrnICardRequest trnicrd
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

                    wherequery = @"WHERE trnicrd.StatusId=1 AND (@applyForId IS NULL OR Afor.ApplyForId = @applyForId) AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm))";
                }
                else // For For Show
                {
                    dTO.TypeId = dTO.stepcount - 1;
                    query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName FROM BasicDetails B
                            inner join MRank ran on ran.RankId=B.RankId
                            inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                            inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                            inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId
                            inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                            inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                            inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                            inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.FwdStatusId=2
                            inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";
                    
                    wherequery = @"WHERE (@applyForId IS NULL OR Afor.ApplyForId = @applyForId) AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm)) ";

                }
            }
            else if (dTO.stepcount == 7 || dTO.stepcount == 8 || dTO.stepcount == 9 || dTO.stepcount == 10)//Reject From IO
            {

                query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,fwd.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName FROM BasicDetails B
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

                wherequery = @"WHERE (@applyForId IS NULL OR Afor.ApplyForId = @applyForId) AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm))";

            }
            else if (dTO.stepcount == 11)
            {
                query = @"trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,fwd.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName FROM TrnICardRequest trnicrd
                        INNER JOIN BasicDetails B ON trnicrd.BasicDetailId = B.BasicDetailId
                        inner join MRank ran on ran.RankId=B.RankId 
                        inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId 
                        inner join MUnit munit on munit.UnitId=mapunit.UnitId 
                        inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId 
                        inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId
                        inner join MICardType ty on ty.TypeId = trnicrd.TypeId
                        inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.FwdStatusId=4
                        inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId";

                wherequery = @"WHERE trnicrd.StatusId=1 AND (@applyForId IS NULL OR Afor.ApplyForId = @applyForId) AND ((@SearchTerm IS NULL) OR (B.ServiceNo LIKE @SearchTerm OR trnicrd.TrackingId LIKE @SearchTerm)) ";
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

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTOBasicDetailIndexResponse>()).ToList();

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
                                         TrackingId = e.TrackingId,
                                         RankName = e.RankName,
                                         UnitId = e.UnitId,
                                         UnitName = e.UnitName,
                                         RegimentalName = e.RegimentalName,

                                     }).ToList();
                    var totalFilteredRecords = allrecord?.FirstOrDefault()?.TotalFilteredRecords;
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
                _logger.LogError(1001, ex, "BasicDetailDB->GetALLBasicDetail");
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
                            " where icardreq.RequestId in @Ids";
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
                                ,icardreq.RequestId from BasicDetails bas
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
        public async Task<string?> GetCSVString(DTOCSVExportRequest Data)
        {
            string query = string.Empty;
            if (Data.IdsTypeRequestIdOrTrnFwdId == true)
            {
                //Ids is TrnFwdId.
                query = " Select B.ServiceNo,B.NameAsPerRecord,B.DOB,B.DateOfCommissioning,ran.RankAbbreviation,B.FName,B.LName,munit.UnitName,trnicrd.TrackingId,Afor.Name ApplyFor,ty.name ICardType,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode from BasicDetails B " +
                        " inner join TrnAddress trnadd on trnadd.BasicDetailId = B.BasicDetailId " +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId " +
                        " where fwd.TrnFwdId in @Ids";
            }
            else
            {
                //Ids is RequestId.
                query = " Select B.ServiceNo,B.NameAsPerRecord,B.DOB,B.DateOfCommissioning,ran.RankAbbreviation,B.FName,B.LName,munit.UnitName,trnicrd.TrackingId,Afor.Name ApplyFor,ty.name ICardType,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode from BasicDetails B " +
                        " inner join TrnAddress trnadd on trnadd.BasicDetailId = B.BasicDetailId " +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        " where trnicrd.RequestId in @Ids";
            }

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
                                         ServiceNo = e.ServiceNo,
                                         NameAsPerRecord = e.NameAsPerRecord,
                                         DOB = DateOnly.FromDateTime(e.DOB),
                                         DateOfCommissioning = DateOnly.FromDateTime(e.DateOfCommissioning),
                                         RankAbbreviation = e.RankAbbreviation,
                                         FName = e.FName,
                                         LName = e.LName,
                                         UnitName = e.UnitName,
                                         TrackingId = e.TrackingId,
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
        public async Task<List<ICardHistoryResponse>?> ICardHistoryByTrackingId(string TrackingId)
        {
            string query = " select usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank, " +
                            " usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ," +
                            " CASE fwd.FwdStatusId WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved' WHEN 3 THEN 'Reject' WHEN 4 THEN 'Internal Forward' END Status," +
                            " fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark, " +
                            " fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2, " +
                            " reason.Reason,postind.Authority,initres.UnitName,req.RequestId " +
                            " from TrnFwds fwd " +
                            " inner join TrnICardRequest req on req.RequestId=fwd.RequestId " +
                            " inner join TrnStepCounter step" +
                            " on fwd.RequestId=step.RequestId" +
                            " inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=fwd.FromAspNetUsersId" +
                            " inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId" +
                            " inner join TrnDomainMapping mapto on mapto.AspNetUsersId=fwd.ToAspNetUsersId" +
                            " inner join AspNetUsers usersto on usersto.Id=mapto.AspNetUsersId" +
                            " left join UserProfile profrom" +
                            " on mapfrom.UserId=profrom.UserId" +
                            " inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId" +
                            " left join UserProfile proto" +
                            " on mapto.UserId=proto.UserId" +
                            " left join TrnPostingOut postind on postind.TrnFwdId=fwd.TrnFwdId" +
                            " left join MPostingReason reason on reason.Id=postind.ReasonId" +
                            " left join MapUnit Munitres on Munitres.UnitMapId=postind.ToUnitID" +
                            " left join MUnit initres on initres.UnitId=Munitres.UnitId" +
                            " inner join MRank ranlto on ranlto.RankId=proto.RankId where req.TrackingId=@TrackingId" +
                            " order by fwd.TrnFwdId asc";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<ICardHistoryResponse>(query, new { TrackingId });

                    return BasicDetailList.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
                return null;
            }

        }
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
        public async Task<List<DTONotificationResponse>?> GetNotification(int UserId, int Type, int applyForId)
        {
            string query = "select dis.DisplayId,Spanname,Message,ranks.RankAbbreviation,bas.Name,bas.ServiceNo,tre.TrackingId,uplod.PhotoImagePath,dis.Url  from TrnNotification noti" +
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
        public async Task<List<DTONotificationResponse>?> GetNotificationRequestId(int UserId, int Type, int applyForId)
        {
            string query = "select Distinct tre.RequestId, dis.DisplayId,Spanname + 'self' Spanname,Message,ranks.RankAbbreviation,bas.Name,bas.ServiceNo,tre.TrackingId,uplod.PhotoImagePath,CASE WHEN dis.DisplayId in (7,8,9,10,17,18,19,20) THEN dis.Url ELSE '' END AS Url  from TrnNotification noti " +
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
        public async Task<DTOApplicationTrack?> ApplicationHistory(string ApplicationHistory)
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
               " where req.StatusId=1 and req.TrackingId=@TrackingId";

                //" select fwd.FwdStatusId,fwd.stepId,fwd.UpdatedOn,step.Name,fwd.IsComplete" +
                //" from TrnFwds fwd " +
                //" inner join TrnICardRequest req on fwd.RequestId=req.RequestId" +
                //" inner join MStepCounterStep step on fwd.StepId=step.StepId" +
                //"  where fwd.RequestId=@RequestId" +
                //" order by fwd.TrnFwdId asc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOApplicationDetails>(query, new { ApplicationHistory });
                    lst.dTOApplicationDetails = ret.FirstOrDefault() ?? new DTOApplicationDetails();
                }
                query = " select fwd.FwdStatusId,fwd.stepId,fwd.UpdatedOn,step.Name,fwd.IsComplete," +
                        " isnull(fwd.Remark,'') Remark," +
                        " (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remark2" +
                        " from TrnFwds fwd " +
                        " inner join TrnICardRequest req on fwd.RequestId=req.RequestId" +
                        " inner join MStepCounterStep step on fwd.StepId=step.StepId" +
                        " where req.StatusId=1 and req.TrackingId=@TrackingId" +
                        " order by fwd.TrnFwdId asc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret1 = await connection.QueryAsync<DTOTrackHistory>(query, new { ApplicationHistory });
                    lst.dTOTrackHistory = ret1.ToList();
                }
                return lst;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ApplicationHistory");
                return null;
            }
        }

        public async Task<List<DTOCardPriningRequest>> CardPrintingCSVCheck(List<DTOCardPriningRequest> requests)
        {
            byte StepId = 5;
            var response = new List<DTOCardPriningRequest>();
            foreach (var batchRecords in requests.Chunk(5000))
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var resultInChunks = (from record in batchRecords
                                          join dbrecord in _context.TrnICardRequest on record.RequestId equals dbrecord.RequestId.ToString() into dbRecordJoin
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
                                              RequestId = record.RequestId,
                                              ServiceNo = record.ServiceNo,
                                              ChipNo = record.ChipNo,
                                              CardSerialNo = record.CardSerialNo,
                                              IsValid = matchRecord != null && cardNoExists == null && chipNoExists == null && stepStatus != null && armyNoCheck != null,
                                              Status = matchRecord != null && cardNoExists == null && chipNoExists == null && stepStatus != null ? "Valid" : "DbInvalid",
                                              Remarks = (matchRecord == null ? "RequestId not exists; " : "") +
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
        public async Task<DTOGenericResponse<string>> CardDispatchCSVUpload(List<DTOCardDispatchCheckRequest> requests, DTODispatchOutRequestWithoutIFormFile dTODispatch)
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
                insert = @"INSERT INTO TrnDispatchCard(Step,ApplyForId,RegId,RecordOfficeId,OutDate,ReceiptDate,DispatchDate,RefOfDispatch,LotNo,NameOfCourierIncharge,UploadFilePath,FromRemark,ToRemark,FromUnitId,ToUnitId,ToUserId,FromUserId,FromAspNetUsersId,ToAspNetUsersId,IsComplete,IsActive,Updatedby,UpdatedOn,DispatchModeId)
                                OUTPUT INSERTED.DispatchCardId
                                VALUES(@Step,@ApplyForId,@RegId,@RecordOfficeId,@OutDate,@ReceiptDate,@DispatchDate,@RefOfDispatch,@LotNo,@NameOfCourierIncharge,@UploadFilePath,@FromRemark,@ToRemark,@FromUnitId,@ToUnitId,@ToUserId,@FromUserId,@FromAspNetUsersId,@ToAspNetUsersId,@IsComplete,@IsActive,@Updatedby,@UpdatedOn,@DispatchModeId)";
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
                parameters.Add("@LotNo", dTODispatch.LotNo, DbType.String, ParameterDirection.Input,50);
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
