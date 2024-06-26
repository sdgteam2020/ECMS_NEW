﻿using BusinessLogicsLayer.User;
using DapperRepo.Core.Constants;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using DataTransferObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.Master
{ 
    public class MasterBL : IMasterBL
    {
        private readonly IMasterDB _iMasterDB;

        public MasterBL(IMasterDB iMasterDB)
        {
            _iMasterDB = iMasterDB;   
        }

       
        public async Task<List<DTOMasterResponse>> GetMFmnBranches()
        {
            return await _iMasterDB.GetMFmnBranches();
        }

        public async Task<List<DTOMasterResponse>> GetMPSO()
        {
            return await _iMasterDB.GetMPSO();
        }

        public async Task<List<DTOMasterResponse>> GetMSubDte()
        {
            return await _iMasterDB.GetMSubDte();
        }

        public async Task<List<DTOMasterResponse>> GetPostingReason(int TypeId)
        {
            return await _iMasterDB.GetPostingReason(TypeId);
        }

        public async Task<List<DTORemarksResponse>> GetRemarksByTypeId(DTORemarksRequest Data)
        {
          return  await _iMasterDB.GetRemarksByTypeId(Data);
        }
        public async Task<DTODashboardMasterCountResponse> GetDashboardMasterCount()
        {
            return await _iMasterDB.GetDashboardMasterCount();
        }
        public async Task<List<DTOGetMappedForRecordResponse>> GetMappedForRecord(int TypeId, string SearchName)
        {
            return await _iMasterDB.GetMappedForRecord(TypeId, SearchName);
        }
        public async Task<DTOGetDomainIdByTDMIdResponse?> GetDomainIdByTDMId(int TDMId)
        {
            return await _iMasterDB.GetDomainIdByTDMId(TDMId);
        }
        public async Task<List<DTOArmsListResponse>> GetArmsList()
        {
            return await _iMasterDB.GetArmsList();
        }
    }
}
