using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;

namespace BusinessLogicsLayer.BasicDet
{
    public interface IBasicDetailBL: IGenericRepositoryDL<BasicDetail>
    {
        public Task<List<DTODispatchCardForCSVResponse>> ExportCsvFileForDispatchCard(int[] RequestIds);
        public Task<DTODataTablesWithSelectedIdsResponse<DTODispatchCardStatusResponse>> GetDispatchCardStatusListForDialog(DTODataTablesRequestForCardStatusList dTO, byte ClaimValue);
        public Task<DTODataTablesResponse<DTODispatchCardStatusResponse>> GetDispatchCardStatusListForExport(byte ClaimValue, DTOExportDispatch Data);
        public Task<DTODataTablesWithSelectedIdsResponse<DTOCardDispatchDialogResponse>> GetDispatchCardDataForDialog(DTODataTablesRequestForCardDispatchDialog dTO);
        public Task<DTODataTablesResponse<DTODispatchCardListResponse>> GetAllDispatchCard(DTODataTablesRequestForCardDispatch dTO);
        public Task<DTOGenericResponse<string>> CardDispatchCSVUpload(List<DTOCardDispatchCheckRequest> requests, DTODispatchOutRequest dTODispatch);
        public Task<List<DTOCardDispatchCheckRequest>> CardDispatchCSVCheck(int[] RequestIds, byte ClaimValue, DTODispatchOutRequest dTO);
        public Task<DTOGenericResponse<DTODispatchToResponse?>> GetUserIdWithName(int AspNetUsersId);
        public Task<DTOGenericResponse<DTODispatchToResponse?>> GetDispatchToData(byte CategeryId, int Id);
        public Task<DTOGenericResponse<DTOOROWithRegimentAndUnitResponse>> GetddlRecordRegiment(byte ClaimValue, int TDMId, int UnitId, int ToUnitId);
        public Task<DTORecordRegimentIdResponse?> GetRecordRegimentId(byte ClaimValue, int TDMId, int UnitId);
        public Task<byte?> GetRecordOfficeId(byte ApplyForId, string ServiceNo, byte ArmedId, short RankId, DTOApplFwdConditionRequest dTOApplFwdCondition);
        public Task<bool> CheckArmyNO(string ArmyNo);
        public Task<string?> GetCSVString(DTOCSVExportRequest Data);
        public Task<List<DTOTopArmyNoFromICardRequestResponse>?> GetTopArmyNoFromICardRequest(string ArmyNo);
        public Task<DTOBDetailByRequestIdResponse?> GetBDetailByRequestId(int RequestId);
        public Task<DTODataTablesResponse<DTOICardRequestHoldResponse>> GetAllICardRequestHold(DTODataTablesRequest dTO);
        public Task<DTODataTablesWithSelectedIdsResponse<DTOBasicDetailIndexResponse>> GetALLBasicDetail(DTODataTablesRequestFor_BasicDetails_Index dTO);
        public Task<DTODataTablesResponse<DTOBasicDetailIndexResponse>> GetALLForIcardSttaus(DTODataTablesRequestFor_BasicDetails_Index dTO);
        public Task<List<DTOICardTypeRequest>> GetAllICardType();
        public Task<int?> MaxBasicDetailId(string ServiceNo);
        public Task<List<DTOSmartSearch>?> SearchAllServiceNo(DTOSearchArmyNoRequest dto);
        public Task<DTOGenericResponse<DTOBasicDetailForParitalViewResponse>> GetBasicDetailForParitalViewByRequestId(int RequestId);
        public Task<DTOBasicDetailByRequestIdResponse?> GetBasicDetailByRequestId(int RequestId);
        public Task<BasicDetailCrtAndUpdVM?> GetBesicDetailForEditById(int BasicDetailId);
        public Task<ICardHistoryResponseAll> ICardHistory(int RequestId);
        public Task<DTOFwdLastRecForDigitalSign> ICardFwdLastRec(int RequestId);
        public Task<List<ICardHistoryResponse>?> ICardHistoryByRequestId(int RequestId);
        public Task<DTOBasicDetailsSaveResponse> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address,MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter);
        public Task<DTOGenericResponse<DTOICardTaskCountResponse>> GetTaskCountICardRequest(DTOGetTaskCountICardRequest dTOGetTaskCount);
        public Task<DTONotificationResult> GetNotification(int UserId);
        public Task<List<DTONotificationResponse>?> GetNotificationRequestId(int UserId, int Type,int applyForId);
        public Task<List<DTODataExportsResponse>> GetDataForExportAndUpdateRequestAndStep(DTODataExportRequest Data, DTOApplFwdConditionRequest dTOApplFwdCondition);
        public Task<DTOXMLDigitalResponse> GetDataDigitalXmlSign(DTODataDigitalXmlSignRequest Data);
        public Task<List<MRecordOffice>?> GetROListByArmedId(byte ArmedId);
        Task<List<DTOCardPriningRequest>> ValidateCardPrinitng(List<DTOCardPriningRequest> request, int CardPrintedByAspNetUserId, int CardPrintedByUserId);
        public Task<List<DTOCardDispatchCheckRequest>> ValidateCardDispatchData(int[] RequestIds, byte ClaimValue, DTODispatchOutRequest dTO);
        Task<DTOUploadChipAndSerialResponse?> CardPrinitngCSVUpload(List<DTOCardPriningRequest> request);
        Task<byte?> CheckCardStatus(int RequestId);
        Task<ICardHistoryResponseAll> ICardHistoryCompleted(int RequestId);
        Task<ICardHistoryResponseAll> ICardHistoryClosed(int RequestId);
        Task<List<DTOCardMovementHistoryResponse>> GetCardMovementHistory(int requestId);
        Task UpdateCardStatus(int requestId, byte status);
        public Task<DTOGenericResponse<string>> CheckBeforeDistribution(int requestId, int UnitId);
        public Task<DTOGenericResponse<string>> DispatchCardIn(List<DTODispatchCardInRequest> dTODispatch, byte StepId, int DispatchCardId, string ToRemark);
        public Task<DTOPreventBasicDetailEditResponse?> GetPreventBasicDetailEdit(int BasicDetailId);
        public Task<DTOGenericResponse<string>> CheckBeforeBesicDetailPost(BasicDetailCrtAndUpdVM basicDetail);
        public Task<DTODataTablesResponse<DTOCompletedHistoryResponse>> GetAllCompletedHistory(DTODataTablesRequestFor_CompletedHistory dTO);
        public Task<DTODataTablesResponse<DTOClosedHistoryResponse>> GetAllClosedHistory(DTODataTableRequestForAppClosedHistory dTO);
        public Task<DTOGetMappingDetailsForClosedHistoryResponse> GetMappingDetailsForClosedHistory(DTODataTableRequestForAppClosedHistory dTO);
        public Task<ICardHistoryResponseAll> GetCompletedHistoryByRequestId(int RequestId);
        public Task<ICardHistoryResponseAll?> GetClosedHistoryByRequestId(int RequestId);
        public Task<DTOGenericResponse<DTOGetICardPrintPreviewByRequestIdResponse>> GetICardPrintPreviewByRequestId(int RequestId);
        public Task<DTOGenericResponse<DTOGetBasicDetailForPdfDigitalSignature>> GetBasicDetailForPdfDigitalSignature(int RequestId);
        public Task<DTOGetHistoryForPopupResponse> GetHistoryForPopup(string ServiceNo);
        public string MaskAadhaar(string? aadhaarNumber);
        public string FormatServiceNo(string? serviceNo);
    }
}
