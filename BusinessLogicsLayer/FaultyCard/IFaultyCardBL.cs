using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.FaultyCard
{
    public interface IFaultyCardBL : IGenericRepositoryDL<TrnFaultyCard>
    {
        public Task<string> GetRemarksData(int[] RemarksIds);
        public Task<bool> FindRequestId(int RequestId);
        public Task<DTOGenericResponse<DTOFaultyCardListResponse?>> GetTrnFaultyCardDetail(int TrnFaultyCardId);
        public Task<DTODataTablesResponse<DTOFaultyCardListResponse>> GetAllFaulty(DTODataTablesRequestForFaultyCard request);
        public Task<DTOCommonSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO, MTrnFwd? mTrnFwd);
        public Task<DTOBeforeFaultyCardReportResponse> CheckBeforeFaultyCardReport(DTOFaultyCardRequest dTOFaulty);
    }
}
