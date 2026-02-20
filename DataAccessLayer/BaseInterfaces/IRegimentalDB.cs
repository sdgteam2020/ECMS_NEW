using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IRegimentalDB : IGenericRepositoryDL<MRegimental>
    {
        public Task<bool> GetByName(MRegimental Dto);
        public Task<List<DTORegimentalResponse>> GetByArmedId(byte ArmedId);
        public Task<List<DTORegimentalResponse>> GetAllData();
        public Task<DTODataTablesResponse<DTORegimentalResponse>> GetAllRegimental_Pagination(DTODataTablesRequest dTO);
        public Task<bool> ValidateUnitIdInRegimental(int UnitId);
    }
}
