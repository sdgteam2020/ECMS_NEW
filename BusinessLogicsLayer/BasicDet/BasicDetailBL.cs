using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo;
using System.Reflection;


namespace BusinessLogicsLayer.BasicDet
{
    public class BasicDetailBL : GenericRepositoryDL<BasicDetail>, IBasicDetailBL
    {
        private readonly IBasicDetailDB _iBasicDetailDB;
        private readonly ILogger<BasicDetailBL> _logger;
        private readonly UserManager<ApplicationUser> userManager;// For Identity
        public BasicDetailBL(ApplicationDbContext context, IBasicDetailDB BasicDetail, ILogger<BasicDetailBL> logger, UserManager<ApplicationUser> userManager) : base(context)
        {
            _iBasicDetailDB = BasicDetail;
            _logger = logger;
            this.userManager = userManager;
        }
        public async Task<List<DTODispatchCardForCSVResponse>> ExportCsvFileForDispatchCard(int[] RequestIds)
        {
            return await _iBasicDetailDB.ExportCsvFileForDispatchCard(RequestIds);
        }
        public async Task<DTODataTablesWithSelectedIdsResponse<DTODispatchCardStatusResponse>> GetDispatchCardStatusListForDialog(DTODataTablesRequestForCardStatusList dTO, byte ClaimValue)
        {
            return await _iBasicDetailDB.GetDispatchCardStatusListForDialog(dTO, ClaimValue);
        }
        public async Task<DTODataTablesResponse<DTODispatchCardStatusResponse>> GetDispatchCardStatusListForExport(byte ClaimValue, DTOExportDispatch Data)
        {
            return await _iBasicDetailDB.GetDispatchCardStatusListForExport(ClaimValue, Data);
        }
        public async Task<DTODataTablesWithSelectedIdsResponse<DTOCardDispatchDialogResponse>> GetDispatchCardDataForDialog(DTODataTablesRequestForCardDispatchDialog dTO)
        {
            return await _iBasicDetailDB.GetDispatchCardDataForDialog(dTO);
        }
        public async Task<DTODataTablesResponse<DTODispatchCardListResponse>> GetAllDispatchCard(DTODataTablesRequestForCardDispatch dTO)
        {
            return await _iBasicDetailDB.GetAllDispatchCard(dTO);
        }
        public async Task<DTOGenericResponse<string>> CardDispatchCSVUpload(List<DTOCardDispatchCheckRequest> requests, DTODispatchOutRequest dTODispatch)
        {
            return await _iBasicDetailDB.CardDispatchCSVUpload(requests, dTODispatch);
        }
        public async Task<List<DTOCardDispatchCheckRequest>> CardDispatchCSVCheck(int[] RequestIds, byte ClaimValue, DTODispatchOutRequest dTO)
        {
            return await _iBasicDetailDB.CardDispatchCSVCheck(RequestIds, ClaimValue, dTO);
        }
        public async Task<DTOGenericResponse<DTODispatchToResponse?>> GetUserIdWithName(int AspNetUsersId)
        {
            return await _iBasicDetailDB.GetUserIdWithName(AspNetUsersId);
        }
        public async Task<DTOGenericResponse<DTODispatchToResponse?>> GetDispatchToData(byte CategeryId, int Id)
        {
            return await _iBasicDetailDB.GetDispatchToData(CategeryId, Id);
        }
        public async Task<DTOGenericResponse<DTOOROWithRegimentAndUnitResponse>> GetddlRecordRegiment(byte ClaimValue, int TDMId, int UnitId, int ToUnitId)
        {
            return await _iBasicDetailDB.GetddlRecordRegiment(ClaimValue, TDMId, UnitId, ToUnitId);
        }

        public async Task<DTORecordRegimentIdResponse?> GetRecordRegimentId(byte ClaimValue, int TDMId, int UnitId)
        {
            return await _iBasicDetailDB.GetRecordRegimentId(ClaimValue, TDMId, UnitId);
        }

        public async Task<byte?> GetRecordOfficeId(byte ApplyForId, string ServiceNo, byte ArmedId, short RankId, DTOApplFwdConditionRequest dTOApplFwdCondition)
        {
            return await _iBasicDetailDB.GetRecordOfficeId(ApplyForId, ServiceNo, ArmedId, RankId, dTOApplFwdCondition);
        }
        public async Task<bool> CheckArmyNO(string ArmyNo)
        {
            return await _iBasicDetailDB.CheckArmyNO(ArmyNo);
        }
        public async Task<string?> GetCSVString(DTOCSVExportRequest Data)
        {
            return await _iBasicDetailDB.GetCSVString(Data);
        }
        public async Task<List<DTOTopArmyNoFromICardRequestResponse>?> GetTopArmyNoFromICardRequest(string ArmyNo)
        {
            return await _iBasicDetailDB.GetTopArmyNoFromICardRequest(ArmyNo);
        }
        public async Task<DTOBDetailByRequestIdResponse?> GetBDetailByRequestId(int RequestId)
        {
            return await _iBasicDetailDB.GetBDetailByRequestId(RequestId);
        }
        public async Task<DTODataTablesResponse<DTOICardRequestHoldResponse>> GetAllICardRequestHold(DTODataTablesRequest dTO)
        {
            return await _iBasicDetailDB.GetAllICardRequestHold(dTO);
        }
        public async Task<DTODataTablesWithSelectedIdsResponse<DTOBasicDetailIndexResponse>> GetALLBasicDetail(DTODataTablesRequestFor_BasicDetails_Index dTO)
        {
            return await _iBasicDetailDB.GetALLBasicDetail(dTO);
        }
        public async Task<List<DTOICardTypeRequest>> GetAllICardType()
        {
            return await _iBasicDetailDB.GetAllICardType();
        }

        public async Task<int?> MaxBasicDetailId(string ServiceNo)
        {
            return await _iBasicDetailDB.MaxBasicDetailId(ServiceNo);
        }
        public async Task<List<DTOSmartSearch>?> SearchAllServiceNo(DTOSearchArmyNoRequest dto)
        {
            return await _iBasicDetailDB.SearchAllServiceNo(dto);
        }
        public async Task<DTOBasicDetailForParitalViewResponse?> GetBasicDetailForParitalViewByRequestId(int RequestId)
        {
            return await _iBasicDetailDB.GetBasicDetailForParitalViewByRequestId(RequestId);
        }

        public async Task<DTOBasicDetailByRequestIdResponse?> GetBasicDetailByRequestId(int RequestId)
        {
            return await _iBasicDetailDB.GetBasicDetailByRequestId(RequestId);
        }
        public Task<BasicDetailCrtAndUpdVM?> GetBesicDetailForEditById(int BasicDetailId)
        {
            return _iBasicDetailDB.GetBesicDetailForEditById(BasicDetailId);
        }

        public Task<ICardHistoryResponseAll> ICardHistory(int RequestId)
        {

            return _iBasicDetailDB.ICardHistory(RequestId);
        }
        public Task<DTOFwdLastRecForDigitalSign> ICardFwdLastRec(int RequestId)
        {

            return _iBasicDetailDB.ICardFwdLastRec(RequestId);
        }
        public Task<List<ICardHistoryResponse>?> ICardHistoryByRequestId(int RequestId)
        {

            return _iBasicDetailDB.ICardHistoryByRequestId(RequestId);
        }

        public async Task<DTODataTablesResponse<DTOBasicDetailIndexResponse>> GetALLForIcardSttaus(DTODataTablesRequestFor_BasicDetails_Index dTO)
        {
            return await _iBasicDetailDB.GetALLForIcardSttaus(dTO);
        }

        public Task<DTOBasicDetailsSaveResponse> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address, MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter)
        {
            return _iBasicDetailDB.SaveBasicDetailsWithAll(Data, address, trnUpload, mTrnIdentityInfo, mTrnICardRequest, mStepCounter);
        }

        public async Task<DTOGenericResponse<DTOICardTaskCountResponse>> GetTaskCountICardRequest(DTOGetTaskCountICardRequest dTOGetTaskCount)
        {
            DTOGenericResponse<DTOICardTaskCountResponse> response = new DTOGenericResponse<DTOICardTaskCountResponse>();

            if (dTOGetTaskCount.Type == 1) // Submitted
            {
                return await _iBasicDetailDB.GetTaskCountICardRequest(dTOGetTaskCount);
            }
            else if (dTOGetTaskCount.Type == 2) //Pendding
            {
                var user = await userManager.FindByIdAsync(dTOGetTaskCount.UserId.ToString());

                // Retrieve all claims associated with the user
                var UserClaims = await userManager.GetClaimsAsync(user);

                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                {
                    dTOGetTaskCount.CValue = 1; //AFSAC Cell
                }
                else if (UserClaims.Any(i => i.Value == "Appl Approver") && UserClaims.Any(i => i.Value == "Internal Wk Distr"))
                {
                    dTOGetTaskCount.CValue = 2; //RO
                }
                else if (UserClaims.Any(i => i.Value == "Appl Approver"))
                {
                    dTOGetTaskCount.CValue = 3; //ORO / RO_2
                }
                else
                {
                    dTOGetTaskCount.CValue = 0; //Verifier
                }

                return await _iBasicDetailDB.GetTaskCountICardRequest(dTOGetTaskCount);
            }
            else
            {
                response.Result = false;
                response.Message = "Invalid Type";
                response.Value = new DTOICardTaskCountResponse();
                return response;
            }

        }
        public async Task<DTONotificationResult> GetNotification(int UserId)
        {
            return await _iBasicDetailDB.GetNotification(UserId);
        }

        public Task<List<DTONotificationResponse>?> GetNotificationRequestId(int UserId, int Type, int applyForId)
        {
            return _iBasicDetailDB.GetNotificationRequestId(UserId, Type, applyForId);
        }

        public Task<List<DTODataExportsResponse>> GetBesicdetailsByRequestId(DTODataExportRequest Data, DTOApplFwdConditionRequest dTOApplFwdCondition)
        {

            var data = _iBasicDetailDB.GetBesicdetailsByRequestId(Data, dTOApplFwdCondition);

            return data;
        }
        public Task<DTOXMLDigitalResponse> GetDataDigitalXmlSign(DTODataExportRequest Data)
        {
            var data = _iBasicDetailDB.GetDataDigitalXmlSign(Data);
            return data;
        }
        public async Task<List<MRecordOffice>?> GetROListByArmedId(byte ArmedId)
        {
            var data = await _iBasicDetailDB.GetROListByArmedId(ArmedId);
            return data;
        }
        public async Task<DTOUploadChipAndSerialResponse?> CardPrinitngCSVUpload(List<DTOCardPriningRequest> request)
        {
            var data = await _iBasicDetailDB.CardPrintingCSVUpload(request);
            return data;
        }

        public async Task<byte?> CheckCardStatus(int RequestId)
        {
            var data = await _iBasicDetailDB.CheckCardStatus(RequestId);
            return data;
        }
        public async Task<ICardHistoryResponseAll> ICardHistoryCompleted(int RequestId)
        {
            var data = await _iBasicDetailDB.ICardHistoryCompleted(RequestId);
            return data;
        }
        public async Task<List<DTOCardDispatchCheckRequest>> ValidateCardDispatchData(int[] RequestIds, byte ClaimValue, DTODispatchOutRequest dTO)
        {
            List<DTOCardDispatchCheckRequest> checkDbRecords = new List<DTOCardDispatchCheckRequest>();
            try
            {
                checkDbRecords = await _iBasicDetailDB.CardDispatchCSVCheck(RequestIds, ClaimValue, dTO);
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "BasicDetailBL->ValidateCardDispatchData");
            }
            return checkDbRecords;
        }
        public async Task<List<DTOCardPriningRequest>> ValidateCardPrinitng(List<DTOCardPriningRequest> request)
        {
            try
            {
                // Get properties to check (excluding Remarks, IsValid, Status)
                var properties = GetPropertiesToCheck();

                // Find duplicate values in request
                var duplicateValuesDict = FindDuplicateValues(request, properties);

                // Mark records with remarks
                request = MarkRecordsWithRemarks(request, properties, duplicateValuesDict);

                // Separate valid and invalid records
                var (validRecords, invalidRecords) = SeparateRecordsByValidity(request);

                // Ensure validRecords is never null
                validRecords = validRecords ?? new List<DTOCardPriningRequest>();

                // Check valid records in the database
                if (validRecords.Any())
                {
                    var checkDbRecords = await _iBasicDetailDB.CardPrintingCSVCheck(validRecords);
                    validRecords = checkDbRecords.Where(r => r.IsValid).ToList();
                    invalidRecords = invalidRecords.Concat(checkDbRecords.Where(r => !r.IsValid)).ToList();
                }

                // Combine invalid and valid records
                request = invalidRecords.Concat(validRecords).ToList();
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "BasicDetailBL->ValidateCardPrinitng");
            }
            return request;
        }
        private List<PropertyInfo> GetPropertiesToCheck()
        {
            return typeof(DTOCardPriningRequest).GetProperties()
                .Where(p => p.Name != nameof(DTOCardPriningRequest.Remarks)
                            && p.Name != nameof(DTOCardPriningRequest.IsValid)
                            && p.Name != nameof(DTOCardPriningRequest.Status))
                .ToList();
        }
        private Dictionary<string, HashSet<string?>> FindDuplicateValues(List<DTOCardPriningRequest> request, List<PropertyInfo> properties)
        {
            return properties.ToDictionary(
                    prop => prop.Name,
                    prop =>
                    {
                        // Create a list of values for the current property
                        var values = request
                            .Select(r =>
                            {
                                // Get value and handle nullable values safely
                                var value = prop.GetValue(r) as string; // Safe casting to string
                                return value?.Trim(); // Using null-conditional operator to ensure value is not null
                            })
                            .Where(value => !string.IsNullOrWhiteSpace(value))  // Filter out null, empty, or whitespace values
                            .ToList();

                        // Group by value, find duplicates, and return them as a HashSet
                        return values
                            .GroupBy(v => v)
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToHashSet();
                    }
                );
        }

        private List<DTOCardPriningRequest> MarkRecordsWithRemarks(List<DTOCardPriningRequest> request, List<PropertyInfo> properties, Dictionary<string, HashSet<string>> duplicateValuesDict)
        {
            return request.Select(r =>
            {
                var remarks = ValidateRecordProperties(r, properties, duplicateValuesDict);

                if (remarks.Any())
                {
                    r.IsValid = false;
                    r.Status = "SheetInValid";
                    r.Remarks = string.Join("; ", remarks);
                }

                return r;
            }).ToList();
        }
        private List<string> ValidateRecordProperties(DTOCardPriningRequest r, List<PropertyInfo> properties, Dictionary<string, HashSet<string>> duplicateValuesDict)
        {
            var remarks = new List<string>();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(r)?.ToString()?.Trim();

                // Validate specific properties
                if (prop.Name == "RequestId" && !IsInteger(value))
                {
                    remarks.Add($"{prop.Name} is not valid");
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    remarks.Add($"{prop.Name} is blank");
                }
                else if (prop.Name == nameof(DTOCardPriningRequest.ServiceNo) && value.Length > 10)
                {
                    remarks.Add($"{prop.Name} is out of range");
                }
                else if ((prop.Name == "CardSerialNo" || prop.Name == "ChipNo") && value.Length > 30)
                {
                    remarks.Add($"{prop.Name} is out of range");
                }
                else if (duplicateValuesDict[prop.Name].Contains(value))
                {
                    remarks.Add($"{prop.Name} is duplicate");
                }
            }

            return remarks;
        }
        private bool IsInteger(string value)
        {
            return int.TryParse(value, out _);
        }
        private (List<DTOCardPriningRequest> validRecords, List<DTOCardPriningRequest> invalidRecords) SeparateRecordsByValidity(List<DTOCardPriningRequest> request)
        {
            var validRecords = request.Where(r => r.IsValid).ToList();
            var invalidRecords = request.Where(r => !r.IsValid).ToList();

            return (validRecords, invalidRecords);
        }

        public Task<List<DTOCardMovementHistoryResponse>> GetCardMovementHistory(int requestId)
        {
            return _iBasicDetailDB.GetCardMovementHistory(requestId);
        }

        public async Task UpdateCardStatus(int requestId, byte status)
        {
            await _iBasicDetailDB.UpdateCardStatus(requestId, status);
        }

        public async Task<DTOGenericResponse<string>> CheckBeforeDistribution(int requestId, int UnitId)
        {
            return await _iBasicDetailDB.CheckBeforeDistribution(requestId, UnitId);
        }
        public async Task<DTOGenericResponse<string>> DispatchCardIn(List<DTODispatchCardInRequest> dTODispatch, byte StepId, int DispatchCardId, string ToRemark)
        {
            return await _iBasicDetailDB.DispatchCardIn(dTODispatch, StepId, DispatchCardId, ToRemark);
        }
        public async Task<DTOPreventBasicDetailEditResponse?> GetPreventBasicDetailEdit(int BasicDetailId)
        {
            return await _iBasicDetailDB.GetPreventBasicDetailEdit(BasicDetailId);
        }
        public async Task<DTOGenericResponse<string>> CheckBeforeBesicDetailPost(BasicDetailCrtAndUpdVM basicDetail)
        {
            return await _iBasicDetailDB.CheckBeforeBesicDetailPost(basicDetail);
        }
        public async Task<DTODataTablesResponse<DTOCompletedHistoryResponse>> GetAllCompletedHistory(DTODataTablesRequestFor_CompletedHistory dTO)
        {
            List<DTOCompletedHistoryResponse> dTOCompleteds = new List<DTOCompletedHistoryResponse>();
            var responseData = new DTODataTablesResponse<DTOCompletedHistoryResponse>
            {
                draw = dTO.Draw,        // DataTables draw counter (0 since error)
                recordsTotal = 0,       // Total records (0 since error)
                recordsFiltered = 0,    // Filtered records (0 since error)
                data = dTOCompleteds    // Empty list of data
            };
            if (dTO.UserType == "Completed_IO")
            {
                return await _iBasicDetailDB.GetAllCompletedHistory(dTO);
            }
            else
            {
                var user = await userManager.FindByIdAsync(dTO.AspNetUsersId.ToString());

                // Retrieve all claims associated with the user
                var UserClaims = await userManager.GetClaimsAsync(user);

                if (dTO.UserType == "Completed_ADC")
                {
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        dTO.CValue = 1; //AFSAC Cell

                        DTOGetMappingDetailsForCompletedHistoryResponse dTOGet =await _iBasicDetailDB.GetMappingDetailsForCompletedHistory(dTO);

                        if (dTOGet.TDMId != 0)
                        {
                            if (dTO.TDMId == dTOGet.TDMId) // Match the TDMId for AFSAC Cell Table
                            {
                                return await _iBasicDetailDB.GetAllCompletedHistory(dTO);
                            }
                            else
                            {
                                responseData.Message = "Your mapping not bind.";
                                responseData.Result = false;
                                return responseData;
                            }
                        }
                        else
                        {
                            responseData.Message = "AFSAC Cell mapping missing.";
                            responseData.Result = false;
                            return responseData;
                        }

                    }
                    else
                    {
                        responseData.Message= "You are not authorized to access this data.";
                        responseData.Result = false;
                        return responseData;
                    }
                }
                else if (dTO.UserType == "Completed_ORO")
                {
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Appl Approver"))
                    {
                        dTO.CValue = 2; //ORO
                        DTOGetMappingDetailsForCompletedHistoryResponse dTOGet = await _iBasicDetailDB.GetMappingDetailsForCompletedHistory(dTO);

                        if (dTOGet.RecordOfficeId != null)
                        {
                            dTO.RecordOfficeId = dTOGet.RecordOfficeId;
                            return await _iBasicDetailDB.GetAllCompletedHistory(dTO);
                        }
                        else
                        {
                            responseData.Message = "You are not mapped.";
                            responseData.Result = false;
                            return responseData;
                        }
                    }
                    else
                    {
                        responseData.Message = "You are not authorized to access this data.";
                        responseData.Result = false;
                        return responseData;
                    }
                }
                else if(dTO.UserType == "Completed_RO") 
                {
                    if (UserClaims.Count > 0)
                    {
                        if (UserClaims.Any(i => i.Value == "Appl Approver") && UserClaims.Any(i => i.Value == "Internal Wk Distr"))
                        {
                            dTO.CValue = 3; //RO

                            DTOGetMappingDetailsForCompletedHistoryResponse dTOGet = await _iBasicDetailDB.GetMappingDetailsForCompletedHistory(dTO);

                            if (dTOGet.RecordOfficeId != null)
                            {
                                dTO.RecordOfficeId = dTOGet.RecordOfficeId;
                                return await _iBasicDetailDB.GetAllCompletedHistory(dTO);
                            }
                            else
                            {
                                responseData.Message = "You are not mapped.";
                                responseData.Result = false;
                                return responseData;
                            }
                        }
                        else if (UserClaims.Any(i => i.Value == "Appl Approver"))
                        {
                            dTO.CValue = 4; //RO_2
                            
                            DTOGetMappingDetailsForCompletedHistoryResponse dTOGet = await _iBasicDetailDB.GetMappingDetailsForCompletedHistory(dTO);

                            if (dTOGet.RecordOfficeId != null)
                            {
                                dTO.RecordOfficeId = dTOGet.RecordOfficeId;
                                return await _iBasicDetailDB.GetAllCompletedHistory(dTO);
                            }
                            else
                            {
                                responseData.Message = "You are not mapped.";
                                responseData.Result = false;
                                return responseData;
                            }
                        }
                        else
                        {
                            responseData.Message = "You are not authorized to access this data.";
                            responseData.Result = false;
                            return responseData;
                        }
                    }
                    else
                    {
                        responseData.Message = "You are not authorized to access this data.";
                        responseData.Result = false;
                        return responseData;
                    }
                }
                else
                {
                    responseData.Message = "You are not authorized to access this data.";
                    responseData.Result = false;
                    return responseData;
                }

            }

        }
        public Task<DTOGetMappingDetailsForClosedHistoryResponse> GetMappingDetailsForClosedHistory(DTODataTableRequestForAppClosedHistory dTO)
        {
            return _iBasicDetailDB.GetMappingDetailsForClosedHistory(dTO);
        }
        public async Task<DTODataTablesResponse<DTOClosedHistoryResponse>> GetAllClosedHistory(DTODataTableRequestForAppClosedHistory dTO)
        {
            List<DTOClosedHistoryResponse> dTOCompleteds = new List<DTOClosedHistoryResponse>();
            var responseData = new DTODataTablesResponse<DTOClosedHistoryResponse>
            {
                draw = dTO.Draw,        // DataTables draw counter (0 since error)
                recordsTotal = 0,       // Total records (0 since error)
                recordsFiltered = 0,    // Filtered records (0 since error)
                data = dTOCompleteds    // Empty list of data
            };
            if (dTO.UserType == "Closed_IO")
            {
                return await _iBasicDetailDB.GetAllClosedHistory(dTO);
            }
            else
            {
                var user = await userManager.FindByIdAsync(dTO.AspNetUsersId.ToString());

                // Retrieve all claims associated with the user
                var UserClaims = await userManager.GetClaimsAsync(user);

                if (dTO.UserType == "Closed_ADC")
                {
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        dTO.CValue = 1; //AFSAC Cell

                        DTOGetMappingDetailsForClosedHistoryResponse dTOGet = await _iBasicDetailDB.GetMappingDetailsForClosedHistory(dTO);

                        if (dTOGet.TDMId != 0)
                        {
                            if (dTO.TDMId == dTOGet.TDMId) // Match the TDMId for AFSAC Cell Table
                            {
                                return await _iBasicDetailDB.GetAllClosedHistory(dTO);
                            }
                            else
                            {
                                responseData.Message = "Your mapping not bind.";
                                responseData.Result = false;
                                return responseData;
                            }
                        }
                        else
                        {
                            responseData.Message = "AFSAC Cell mapping missing.";
                            responseData.Result = false;
                            return responseData;
                        }

                    }
                    else
                    {
                        responseData.Message = "You are not authorized to access this data.";
                        responseData.Result = false;
                        return responseData;
                    }
                }
                else if (dTO.UserType == "Closed_ORO")
                {
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Appl Approver"))
                    {
                        dTO.CValue = 2; //ORO
                        DTOGetMappingDetailsForClosedHistoryResponse dTOGet = await _iBasicDetailDB.GetMappingDetailsForClosedHistory(dTO);

                        if (dTOGet.RecordOfficeId != null)
                        {
                            dTO.RecordOfficeId = dTOGet.RecordOfficeId;
                            return await _iBasicDetailDB.GetAllClosedHistory(dTO);
                        }
                        else
                        {
                            responseData.Message = "You are not mapped.";
                            responseData.Result = false;
                            return responseData;
                        }
                    }
                    else
                    {
                        responseData.Message = "You are not authorized to access this data.";
                        responseData.Result = false;
                        return responseData;
                    }
                }
                else if (dTO.UserType == "Closed_RO")
                {
                    if (UserClaims.Count > 0)
                    {
                        if (UserClaims.Any(i => i.Value == "Appl Approver") && UserClaims.Any(i => i.Value == "Internal Wk Distr"))
                        {
                            dTO.CValue = 3; //RO

                            DTOGetMappingDetailsForClosedHistoryResponse dTOGet = await _iBasicDetailDB.GetMappingDetailsForClosedHistory(dTO);

                            if (dTOGet.RecordOfficeId != null)
                            {
                                dTO.RecordOfficeId = dTOGet.RecordOfficeId;
                                return await _iBasicDetailDB.GetAllClosedHistory(dTO);
                            }
                            else
                            {
                                responseData.Message = "You are not mapped.";
                                responseData.Result = false;
                                return responseData;
                            }
                        }
                        else if (UserClaims.Any(i => i.Value == "Appl Approver"))
                        {
                            dTO.CValue = 4; //RO_2

                            DTOGetMappingDetailsForClosedHistoryResponse dTOGet = await _iBasicDetailDB.GetMappingDetailsForClosedHistory(dTO);

                            if (dTOGet.RecordOfficeId != null)
                            {
                                dTO.RecordOfficeId = dTOGet.RecordOfficeId;
                                return await _iBasicDetailDB.GetAllClosedHistory(dTO);
                            }
                            else
                            {
                                responseData.Message = "You are not mapped.";
                                responseData.Result = false;
                                return responseData;
                            }
                        }
                        else
                        {
                            responseData.Message = "You are not authorized to access this data.";
                            responseData.Result = false;
                            return responseData;
                        }
                    }
                    else
                    {
                        responseData.Message = "You are not authorized to access this data.";
                        responseData.Result = false;
                        return responseData;
                    }
                }
                else
                {
                    responseData.Message = "You are not authorized to access this data.";
                    responseData.Result = false;
                    return responseData;
                }

            }

        }
        

    }
}
