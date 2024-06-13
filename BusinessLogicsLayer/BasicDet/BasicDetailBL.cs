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
        public BasicDetailBL(ApplicationDbContext context,IBasicDetailDB BasicDetail) : base(context)
        {
                _iBasicDetailDB = BasicDetail;
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
        public async Task<List<DTOSmartSearch>> SearchAllServiceNo(string ServiceNo, int AspNetUsersId)
        {
            return await _iBasicDetailDB.SearchAllServiceNo(ServiceNo, AspNetUsersId);
        }

        public Task<BasicDetailCrtAndUpdVM> GetByBasicDetailsId(int BasicDetailId)
        {
            return _iBasicDetailDB.GetByBasicDetailsId(BasicDetailId);
        }
        public Task<BasicDetailCrtAndUpdVM> GetByRequestIdBesicDetails(int RequestId)
        {
           
            return _iBasicDetailDB.GetByRequestIdBesicDetails(RequestId);
        }

        public Task<List<ICardHistoryResponse>> ICardHistory(int RequestId)
        {
            
            return _iBasicDetailDB.ICardHistory(RequestId);
        }

        public Task<List<BasicDetailVM>> GetALLForIcardSttaus(int UserId, int step, int type, int applyfor)
        {
            return _iBasicDetailDB.GetALLForIcardSttaus(UserId, step, type,applyfor);
        }

        public Task<bool> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address, MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter)
        {
            return _iBasicDetailDB.SaveBasicDetailsWithAll(Data, address, trnUpload, mTrnIdentityInfo, mTrnICardRequest, mStepCounter);
        }

        public Task<DTOICardTaskCountResponse> GetTaskCountICardRequest(int UserId, int Type, int applyForId)
        {
            return _iBasicDetailDB.GetTaskCountICardRequest(UserId, Type, applyForId);
        }

        public Task<List<DTONotificationResponse>> GetNotification(int UserId, int Type, int applyForId)
        {
            return _iBasicDetailDB.GetNotification(UserId, Type, applyForId);
        }

        public Task<List<DTONotificationResponse>> GetNotificationRequestId(int UserId, int Type, int applyForId)
        {
            return _iBasicDetailDB.GetNotificationRequestId(UserId, Type, applyForId);
        }

        public Task<List<DTODataExportsResponse>> GetBesicdetailsByRequestId(DTODataExportRequest Data)
        {
            
            var data = _iBasicDetailDB.GetBesicdetailsByRequestId(Data);

            return data;
        }
        public Task<DTOXMLDigitalResponse> GetDataDigitalXmlSign(DTODataExportRequest Data)
        {
            var data = _iBasicDetailDB.GetDataDigitalXmlSign(Data);
            return data;
        }
        public async Task<List<MRecordOffice>> GetROListByArmedId(byte ArmedId)
        {
            var data = await _iBasicDetailDB.GetROListByArmedId(ArmedId);
            return data;
        }
        public async Task<IEnumerable<SelectListItem>> GetRODDLIdSelected(byte ArmedId)
        {
            var data = await _iBasicDetailDB.GetRODDLIdSelected(ArmedId);
            return data;
        }

        public async Task<DTOApplicationTrack> ApplicationHistory(string TrackingId)
        {
            var data = await _iBasicDetailDB.ApplicationHistory(TrackingId);
            return data;
        }
    }
}
