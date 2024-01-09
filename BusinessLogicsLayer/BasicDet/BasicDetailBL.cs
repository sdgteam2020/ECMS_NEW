using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDet
{
    public class BasicDetailBL : GenericRepositoryDL<BasicDetail>, IBasicDetailBL
    {
        private readonly IBasicDetailDB _iBasicDetailDB;
        public BasicDetailBL(ApplicationDbContext context,IBasicDetailDB BasicDetail) : base(context)
        {
                _iBasicDetailDB = BasicDetail;
        }
        public Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId, int step, int type)
        {
            return _iBasicDetailDB.GetALLBasicDetail(UserId ,step, type);
        }
        public async Task<List<DTOICardTypeRequest>> GetAllICardType()
        {
            return await _iBasicDetailDB.GetAllICardType();
        }
        public async Task<BasicDetail?> FindServiceNo(string ServiceNo)
        {
            return await _iBasicDetailDB.FindServiceNo(ServiceNo);
        }

        public Task<BasicDetailCrtAndUpdVM> GetByBasicDetailsId(int BasicDetailId)
        {
            return _iBasicDetailDB.GetByBasicDetailsId(BasicDetailId);
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

        public Task<List<DTONotificationResponse>> GetNotification(int UserId, int Type)
        {
            return _iBasicDetailDB.GetNotification(UserId, Type);
        }

        public Task<List<DTONotificationResponse>> GetNotificationRequestId(int UserId, int Type)
        {
            return _iBasicDetailDB.GetNotificationRequestId(UserId, Type);
        }

        public Task<List<DTODataExportsResponse>> GetBesicdetailsByRequestId(DTODataExportRequest Data)
        {
            
            var data = _iBasicDetailDB.GetBesicdetailsByRequestId(Data);

            return data;
        }
       
    }
}
