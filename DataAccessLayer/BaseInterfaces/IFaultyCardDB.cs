using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IFaultyCardDB : IGenericRepositoryDL<TrnFaultyCard>
    {
        public Task<string> GetRemarksData(int[] RemarksIds);
        public Task<bool> FindRequestId(int RequestId);
        public Task<DTOFaultyCardListResponse?> GetTrnFaultyCardDetail(int TrnFaultyCardId);
        public Task<List<DTOFaultyCardListResponse>?> GetAllFaulty(bool Claim, int MapUnitId);
        public Task<DTODataTablesResponse<DTOFaultyCardListResponse>> GetAllFaulty(DTODataTablesRequestForFaultyCard request);
        public Task<DTOCommonSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO, MTrnFwd? mTrnFwd);
        public Task<DTOBeforeFaultyCardReportResponse> CheckBeforeFaultyCardReport(DTOFaultyCardRequest dTOFaulty);
    }
}
