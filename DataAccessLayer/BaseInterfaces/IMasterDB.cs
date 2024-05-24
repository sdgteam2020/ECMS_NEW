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

namespace DataAccessLayer.BaseInterfaces
{
    public interface IMasterDB 
    {
        public Task<List<DTORemarksResponse>> GetRemarksByTypeId(DTORemarksRequest Data);
        public Task<List<DTOMasterResponse>> GetMFmnBranches();
        public Task<List<DTOMasterResponse>> GetMPSO();
        public Task<List<DTOMasterResponse>> GetMSubDte();
        public Task<List<DTOMasterResponse>> GetPostingReason(int TypeId);
        public Task<DTODashboardMasterCountResponse> GetDashboardMasterCount();
        public Task<List<DTOGetMappedForRecordResponse>> GetMappedForRecord(int TypeId, string SearchName);
        public Task<DTOGetDomainIdByTDMIdResponse?> GetDomainIdByTDMId(int TDMId);
        public Task<List<DTOArmsListResponse>> GetArmsList();
    }
}
