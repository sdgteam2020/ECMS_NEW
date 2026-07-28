using Azure.Core;
using BusinessLogicsLayer.Unit;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.FaultyCard
{
    public class FaultyCardBL : GenericRepositoryDL<TrnFaultyCard>, IFaultyCardBL
    {
        private readonly IFaultyCardDB _iFaultyCardDB;
        public FaultyCardBL(ApplicationDbContext context, IFaultyCardDB iFaultyCardDB) : base(context)
        {
            _iFaultyCardDB=iFaultyCardDB;
        }
        public async Task<string> GetRemarksData(int[] RemarksIds) 
        {
            return await _iFaultyCardDB.GetRemarksData(RemarksIds);
        }
        public async Task<bool> FindRequestId(int RequestId)
        {
            return await _iFaultyCardDB.FindRequestId(RequestId);
        }
        public async Task<DTOGenericResponse<DTOFaultyCardListResponse?>> GetTrnFaultyCardDetail(int TrnFaultyCardId)
        {
            return await _iFaultyCardDB.GetTrnFaultyCardDetail(TrnFaultyCardId);
        }
        public async Task<DTODataTablesResponse<DTOFaultyCardListResponse>> GetAllFaulty(DTODataTablesRequestForFaultyCard request)
        {
            return await _iFaultyCardDB.GetAllFaulty(request);
        }
        public async Task<DTOCommonSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO, MTrnFwd? mTrnFwd) 
        {
            if (dTO.TrnFaultyCardId > 0)
            {
                dTO.IsComplete = true;
                dTO.IsEditAction = true;
                return await _iFaultyCardDB.SaveFaultyCard(dTO, mTrnFwd);
            }
            else
            {
                if (dTO.Claim)
                {
                    dTO.IsComplete = true;
                }
                else
                {
                    dTO.IsComplete = false;
                }

                //Accept
                if (dTO.Choice == 2)
                {
                    dTO.IsEditAction = true;
                    return await _iFaultyCardDB.SaveFaultyCard(dTO, mTrnFwd);
                }
                //Reject
                else if (dTO.Choice == 3)
                {
                    dTO.IsEditAction = true;
                    return await _iFaultyCardDB.SaveFaultyCard(dTO, mTrnFwd);
                }
                else
                {
                    dTO.CategoryId = 2; //request by Unit level
                    dTO.IsEditAction = false;
                    dTO.ToRemark = null;
                    return await _iFaultyCardDB.SaveFaultyCard(dTO, mTrnFwd);
                }
            }
        }

        public async Task<DTOBeforeFaultyCardReportResponse> CheckBeforeFaultyCardReport(DTOFaultyCardRequest dTOFaulty)
        {
            return await _iFaultyCardDB.CheckBeforeFaultyCardReport(dTOFaulty);
        }
    }
}
