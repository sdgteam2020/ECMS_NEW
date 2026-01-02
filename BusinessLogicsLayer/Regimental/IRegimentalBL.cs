using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Master
{
    public interface IRegimentalBL : IGenericRepositoryDL<MRegimental>
    {
        public Task<bool> GetByName(MRegimental DTo);
        public Task<List<DTORegimentalResponse>> GetByArmedId(byte ArmedId);
        public Task<List<DTORegimentalResponse>> GetAllData();
        public Task<DTODataTablesResponse<DTORegimentalResponse>> GetAllRegimental_Pagination(DTODataTablesRequest dTO);
    }
}
