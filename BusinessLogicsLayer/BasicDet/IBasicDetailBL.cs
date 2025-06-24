using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDet
{
    public interface IBasicDetailBL:IGenericRepository<BasicDetail>
    {
        public Task<bool> CheckArmyNO(string ArmyNo);
        public Task<DTOUploadChipAndSerialResponse> UploadChipAndSerial(List<DTOUploadChipAndSerialRequest> Data);
        public Task<string?> GetCSVString(DTOCSVExportRequest Data);
        public Task<List<DTOTopArmyNoFromICardRequestResponse>?> GetTopArmyNoFromICardRequest(string ArmyNo);
        public Task<DTOBDetailByRequestIdResponse?> GetBDetailByRequestId(int RequestId);
        public Task<List<DTOICardRequestHoldResponse>?> GetAllICardRequestHold();
        public Task<DTODataTablesResponse<DTOBasicDetailIndexResponse>> GetALLBasicDetail(DTODataTablesRequestFor_BasicDetails_Index dTO);
        public Task<DTODataTablesResponse<DTOBasicDetailIndexResponse>> GetALLForIcardSttaus(DTODataTablesRequestFor_BasicDetails_Index dTO);
        public Task<List<DTOICardTypeRequest>> GetAllICardType();
        public Task<BasicDetail?> FindServiceNo(string ServiceNo);
        public Task<int?> MaxBasicDetailId(string ServiceNo);
        public Task<List<DTOSmartSearch>?> SearchAllServiceNo(DTOSearchArmyNoRequest dto);
        public Task<DTOBasicDetailForParitalViewResponse> GetBasicDetailForParitalViewByRequestId(int RequestId);
        public Task<BasicDetailCrtAndUpdVM?> GetBasicDetailByRequestId(int RequestId);
        public Task<BasicDetailCrtAndUpdVM?> GetBasicDetailById(int BasicDetailId);
        public Task<BasicDetailCrtAndUpdVM?> GetBesicDetailForEditById(int BasicDetailId);
        public Task<ICardHistoryResponseAll?> ICardHistory(int RequestId);
        public Task<DTOFwdLastRecForDigitalSign> ICardFwdLastRec(int RequestId);
        public Task<List<ICardHistoryResponse>?> ICardHistoryByTrackingId(string TrackingId);
        public Task<DTOBasicDetailsSaveResponse> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address,MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter);
        public Task<DTOICardTaskCountResponse?> GetTaskCountICardRequest(int UserId, int Type,int applyForId);
        public Task<List<DTONotificationResponse>?> GetNotification(int UserId, int Type, int applyForId);
        public Task<List<DTONotificationResponse>?> GetNotificationRequestId(int UserId, int Type,int applyForId);
        public Task<List<DTODataExportsResponse>> GetBesicdetailsByRequestId(DTODataExportRequest Data, DTOApplFwdConditionRequest dTOApplFwdCondition);
        public Task<DTOXMLDigitalResponse> GetDataDigitalXmlSign(DTODataExportRequest Data);
        public Task<List<MRecordOffice>?> GetROListByArmedId(byte ArmedId);
        public Task<DTOApplicationTrack?> ApplicationHistory(string RequestId);
        Task<List<DTOCardPriningRequest>> ValidateCardPrinitng(List<DTOCardPriningRequest> request);
        Task<DTOUploadChipAndSerialResponse?> CardPrinitngCSVUpload(List<DTOCardPriningRequest> request);
        Task<byte?> CheckCardStatus(int RequestId);
        Task<ICardHistoryResponseAll> ICardHistoryCompleted(int RequestId);
        Task<List<DTOCardMovementHistoryResponse>> GetCardMovementHistory(int requestId);
        Task UpdateCardStatus(int requestId, byte status);
        Task<DTOUploadChipAndSerialResponse> CheckBeforeDistribution(int requestId);
    }
}
