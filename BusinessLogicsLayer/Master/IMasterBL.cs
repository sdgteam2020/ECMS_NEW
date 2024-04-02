using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{
    public interface IMasterBL
    {
        public Task<List<DTORemarksResponse>> GetRemarksByTypeId(DTORemarksRequest Data);

        public Task<List<DTOMasterResponse>> GetMFmnBranches();
        public Task<List<DTOMasterResponse>> GetMPSO();
        public Task<List<DTOMasterResponse>> GetMSubDte();
        public Task<List<DTOMasterResponse>> GetPostingReason();
        public Task<DTODashboardFormationCountResponse> GetDashboardFormationCount();
        public Task<DTODashboardMasterCountResponse> GetDashboardMasterCount();
        public Task<DTODashboardUserConfigCountResponse> GetDashboardUserConfigCount();
    }
}
