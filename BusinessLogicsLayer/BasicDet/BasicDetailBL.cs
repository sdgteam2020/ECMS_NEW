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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.BasicDet
{
    public class BasicDetailBL : GenericRepositoryDL<BasicDetail>, IBasicDetailBL
    {
        private readonly IBasicDetailDB _iBasicDetailDB;
        public async Task<DTOUploadChipAndSerialResponse> UploadChipAndSerial(List<DTOUploadChipAndSerialRequest> Data)
        {
            return await _iBasicDetailDB.UploadChipAndSerial(Data);
        }
        public async Task<string?> GetCSVString(DTOCSVExportRequest Data) 
        {
            return await _iBasicDetailDB.GetCSVString(Data);
        }
        public BasicDetailBL(ApplicationDbContext context,IBasicDetailDB BasicDetail) : base(context)
        {
                _iBasicDetailDB = BasicDetail;
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
        public Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId, int step, int type, int applyForId)
        {
            return _iBasicDetailDB.GetALLBasicDetail(UserId ,step, type, applyForId);
        }
        public async Task<List<DTOICardTypeRequest>> GetAllICardType()
        {
            return await _iBasicDetailDB.GetAllICardType();
        }
        public async Task<BasicDetail?> FindServiceNo(string ServiceNo)
        {
            return await _iBasicDetailDB.FindServiceNo(ServiceNo);
        } 
        public async Task<List<DTOSmartSearch>?> SearchAllServiceNo(string ServiceNo, int AspNetUsersId)
        {
            return await _iBasicDetailDB.SearchAllServiceNo(ServiceNo, AspNetUsersId);
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

        public Task<List<ICardHistoryResponse>?> ICardHistory(int RequestId)
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

        public Task<List<BasicDetailVM>> GetALLForIcardSttaus(int UserId, int step, int type, int applyfor)
        {
            return _iBasicDetailDB.GetALLForIcardSttaus(UserId, step, type,applyfor);
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
        public async Task<DTOApplicationTrack?> ApplicationHistory(string TrackingId)
        {
            var data = await _iBasicDetailDB.ApplicationHistory(TrackingId);
            return data;
        }

        public async Task<DTOCardDistributionCheckRes?> ValidateCardDistribution(List<DTOCardDistributionRequest> request)
        {
            var data = new DTOCardDistributionCheckRes();
            try
            {
                data.Result = DTOCardDistributionUploadEnum.Rejected;
                //Get properties to check (excluding Remarks)
                var properties = typeof(DTOCardDistributionRequest).GetProperties()
                                                  .Where(p => p.Name != "Remarks" && p.Name != "IsValid")
                                                  .ToList();

                // For each property, find duplicate values
                var duplicateValuesDict = properties.ToDictionary(
                    prop => prop.Name,
                    prop => request
                        .Where(r => !string.IsNullOrWhiteSpace(prop.GetValue(r)?.ToString()))
                        .GroupBy(r => prop.GetValue(r)?.ToString().Trim())
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToHashSet()
                );

                //Mark records with remarks
                request = request.Select(r =>
                {
                    var remarks = new List<string>();

                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(r)?.ToString()?.Trim();

                        if (prop.Name == "RequestId")
                        {
                            int integerValue = 0;
                            var isInteger = int.TryParse(value,out integerValue);
                            if (!isInteger) {
                                remarks.Add($"{prop.Name} is not valid");
                            }
                        }

                        // Null or Blank Check
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            remarks.Add($"{prop.Name} is blank");
                        }
                        // Duplicate Check
                        else if (duplicateValuesDict[prop.Name].Contains(value))
                        {
                            remarks.Add($"{prop.Name} is duplicate");
                        }
                    }

                    if (remarks.Any())
                    {
                        r.IsValid = false;
                        r.Remarks = string.Join("; ", remarks);
                    }
                    return r;
                }).ToList();

                data.ValidRecords = request.Where(r => r.IsValid).ToList();
                data.InValidRecords = request.Where(r => !r.IsValid).ToList();

                if (data.ValidRecords?.Count() > 0 && data.InValidRecords?.Count() > 0)
                {
                    data.Result = DTOCardDistributionUploadEnum.PartiallyUpload;
                }

                if (data.InValidRecords?.Count() ==  0 && data.ValidRecords?.Count() > 0)
                {
                    data.Result = DTOCardDistributionUploadEnum.FullyUpload;
                }

                if (data.ValidRecords?.Count() == 0 && data.InValidRecords?.Count() > 0)
                {
                    data.Result = DTOCardDistributionUploadEnum.Rejected;
                }
            }
            catch(Exception ee)
            {
                data.Result = DTOCardDistributionUploadEnum.InternalError;
            }
            return data;
        }
    }
}
