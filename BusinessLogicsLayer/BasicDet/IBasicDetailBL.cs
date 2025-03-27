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
        public Task<DTOUploadChipAndSerialResponse> UploadChipAndSerial(List<DTOUploadChipAndSerialRequest> Data);
        public Task<string?> GetCSVString(DTOCSVExportRequest Data);
        public Task<List<DTOTopArmyNoFromICardRequestResponse>?> GetTopArmyNoFromICardRequest(string ArmyNo);
        public Task<DTOBDetailByRequestIdResponse?> GetBDetailByRequestId(int RequestId);
        public Task<List<DTOICardRequestHoldResponse>?> GetAllICardRequestHold();
        public Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId,int step,int type, int applyForId);
        public Task<List<BasicDetailVM>> GetALLForIcardSttaus(int UserId,int step,int type,int applyfor);
        public Task<List<DTOICardTypeRequest>> GetAllICardType();
        public Task<BasicDetail?> FindServiceNo(string ServiceNo);
        public Task<List<DTOSmartSearch>?> SearchAllServiceNo(string ServiceNo, int AspNetUsersId);
        public Task<BasicDetailCrtAndUpdVM?> GetBasicDetailByRequestId(int RequestId);
        public Task<BasicDetailCrtAndUpdVM?> GetBasicDetailById(int BasicDetailId);
        public Task<BasicDetailCrtAndUpdVM?> GetBesicDetailForEditById(int BasicDetailId);
        public Task<List<ICardHistoryResponse>?> ICardHistory(int RequestId);
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
        Task<List<DTOCardDistributionRequest>> ValidateCardDistribution(List<DTOCardDistributionRequest> request);
        Task<DTOUploadChipAndSerialResponse?> CardDistributionCSVUpload(List<DTOCardDistributionRequest> request);
    }
}
