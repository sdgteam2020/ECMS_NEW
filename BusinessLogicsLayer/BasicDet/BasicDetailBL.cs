using Azure;
using BusinessLogicsLayer.BdeCate;
using BusinessLogicsLayer.Master;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.BasicDet
{
    public class BasicDetailBL : GenericRepositoryDL<BasicDetail>, IBasicDetailBL
    {
        private readonly IBasicDetailDB _iBasicDetailDB;
        private readonly ILogger<BasicDetailBL> _logger;
        public BasicDetailBL(ApplicationDbContext context,IBasicDetailDB BasicDetail, ILogger<BasicDetailBL> logger) : base(context)
        {
            _iBasicDetailDB = BasicDetail;
            _logger = logger;
        }
        public async Task<DTOGenericResponse<List<DTOMasterResponse>>> GetddlRecordRegiment(byte CategeryId, byte ClaimValue, int TDMId, int UnitId)
        {
            return await _iBasicDetailDB.GetddlRecordRegiment(CategeryId, ClaimValue, TDMId, UnitId);
        }
        public async Task<byte?> GetRecordOfficeId(byte ApplyForId, string ServiceNo, byte ArmedId, short RankId, DTOApplFwdConditionRequest dTOApplFwdCondition)
        {
            return await _iBasicDetailDB.GetRecordOfficeId(ApplyForId, ServiceNo, ArmedId, RankId, dTOApplFwdCondition);
        }
        public async Task<bool> CheckArmyNO(string ArmyNo)
        {
            return await _iBasicDetailDB.CheckArmyNO(ArmyNo);
        }
        public async Task<DTOUploadChipAndSerialResponse> UploadChipAndSerial(List<DTOUploadChipAndSerialRequest> Data)
        {
            return await _iBasicDetailDB.UploadChipAndSerial(Data);
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
        public async Task<List<DTOICardRequestHoldResponse>?> GetAllICardRequestHold()
        {
            return await _iBasicDetailDB.GetAllICardRequestHold();
        }
        public async Task<DTODataTablesResponse<DTOBasicDetailIndexResponse>> GetALLBasicDetail(DTODataTablesRequestFor_BasicDetails_Index dTO)
        {
            return await _iBasicDetailDB.GetALLBasicDetail(dTO);
        }
        public async Task<List<DTOICardTypeRequest>> GetAllICardType()
        {
            return await _iBasicDetailDB.GetAllICardType();
        }
        public async Task<BasicDetail?> FindServiceNo(string ServiceNo)
        {
            return await _iBasicDetailDB.FindServiceNo(ServiceNo);
        }
        public async Task<int?> MaxBasicDetailId(string ServiceNo)
        {
            return await _iBasicDetailDB.MaxBasicDetailId(ServiceNo);
        }
        public async Task<List<DTOSmartSearch>?> SearchAllServiceNo(DTOSearchArmyNoRequest dto)
        {
            return await _iBasicDetailDB.SearchAllServiceNo(dto);
        }
        public async Task<DTOBasicDetailForParitalViewResponse> GetBasicDetailForParitalViewByRequestId(int RequestId)
        {
            return await _iBasicDetailDB.GetBasicDetailForParitalViewByRequestId(RequestId);
        }

        public async Task<BasicDetailCrtAndUpdVM?> GetBasicDetailByRequestId(int RequestId)
        {
            return await _iBasicDetailDB.GetBasicDetailByRequestId(RequestId);
        }
        public async Task<BasicDetailCrtAndUpdVM?> GetBasicDetailById(int BasicDetailId)
        {
            return await _iBasicDetailDB.GetBasicDetailById(BasicDetailId);
        }
        public Task<BasicDetailCrtAndUpdVM?> GetBesicDetailForEditById(int BasicDetailId)
        {
            return _iBasicDetailDB.GetBesicDetailForEditById(BasicDetailId);
        }

        public Task<ICardHistoryResponseAll?> ICardHistory(int RequestId)
        {
            
            return _iBasicDetailDB.ICardHistory(RequestId);
        }
        public Task<DTOFwdLastRecForDigitalSign> ICardFwdLastRec(int RequestId)
        {
            
            return _iBasicDetailDB.ICardFwdLastRec(RequestId);
        }
        public Task<List<ICardHistoryResponse>?> ICardHistoryByTrackingId(string TrackingId)
        {
            
            return _iBasicDetailDB.ICardHistoryByTrackingId(TrackingId);
        }

        public async Task<DTODataTablesResponse<DTOBasicDetailIndexResponse>> GetALLForIcardSttaus(DTODataTablesRequestFor_BasicDetails_Index dTO)
        {
            return await _iBasicDetailDB.GetALLForIcardSttaus(dTO);
        }

        public Task<DTOBasicDetailsSaveResponse> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address, MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter)
        {
            return _iBasicDetailDB.SaveBasicDetailsWithAll(Data, address, trnUpload, mTrnIdentityInfo, mTrnICardRequest, mStepCounter);
        }

        public Task<DTOICardTaskCountResponse?> GetTaskCountICardRequest(int UserId, int Type, int applyForId)
        {
            return _iBasicDetailDB.GetTaskCountICardRequest(UserId, Type, applyForId);
        }

        public Task<List<DTONotificationResponse>?> GetNotification(int UserId, int Type, int applyForId)
        {
            return _iBasicDetailDB.GetNotification(UserId, Type, applyForId);
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
        public async Task<DTOApplicationTrack?> ApplicationHistory(string RequestId)
        {
            var data = await _iBasicDetailDB.ApplicationHistory(RequestId);
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
            catch(Exception ee)
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

        private List<DTOCardPriningRequest> MarkRecordsWithRemarks(List<DTOCardPriningRequest> request,List<PropertyInfo> properties,Dictionary<string, HashSet<string>> duplicateValuesDict)
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

        public async Task<DTOUploadChipAndSerialResponse> CheckBeforeDistribution(int requestId)
        {
            return await _iBasicDetailDB.CheckBeforeDistribution(requestId);
        }
    }
}
