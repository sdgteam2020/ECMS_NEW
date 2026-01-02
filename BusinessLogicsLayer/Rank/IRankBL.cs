using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Bde
{
    public interface IRankBL : IGenericRepositoryDL<MRank>
    {

        public Task<bool> GetByName(MRank Dto);
        public Task<short> GetByMaxOrder();
        public Task<int> OrderByChange(MRank Dto);
        public Task<IEnumerable<MRank>> GetAllByorder();
        public Task<IEnumerable<MRank>> GetAllByType(int Type);
        public Task<DTORankIdCheckInFKTableResponse?> RankIdCheckInFKTable(short RankId);
        public Task<DTODataTablesResponse<DTORankResponse>> GetAllRank_Pagination(DTODataTablesRequest dTO);
    }
}
