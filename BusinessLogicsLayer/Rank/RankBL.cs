using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.BdeCate
{
    public class RankBL : GenericRepositoryDL<MRank>, IRankBL
    {


        private readonly IRankDB _iRankDB;

        public RankBL(ApplicationDbContext context, IRankDB iRankDB) : base(context)
        {
            _iRankDB = iRankDB;
        }

        public Task<IEnumerable<MRank>> GetAllByorder()
        {
            return _iRankDB.GetAllByorder();
        }

        public Task<IEnumerable<MRank>> GetAllByType(int Type)
        {
            return _iRankDB.GetAllByType(Type);
        }

        public Task<short> GetByMaxOrder()
        {
            return _iRankDB.GetByMaxOrder();
        }

        public Task<bool> GetByName(MRank Dto)
        {
            Dto.RankAbbreviation = Dto.RankAbbreviation.Trim().TrimEnd().TrimStart();
            return _iRankDB.GetByName(Dto);
        }

        public async Task<int> OrderByChange(MRank Dto)
        {
            ////Current Order
            short i = Dto.Orderby;
            increment:
            i++;
            short ComdIdnext = await _iRankDB.GetRankIdbyOrderby(i);
            if (ComdIdnext == 0)
            {
                goto increment;
            }
            else
            {
                /////Subtraction order no Next Comd
                var datanext = await GetByGen<short>(ComdIdnext);
                datanext.Orderby = Dto.Orderby;
                await Update(datanext);

                ////////Change Order No For Click
                MRank data = new MRank();
                data = await GetByGen<short>(Dto.RankId);
                data.Orderby = i;
                await Update(data);
                /////////////////////////
            }
            return KeyConstants.Success;
        }
        public async Task<DTORankIdCheckInFKTableResponse?> RankIdCheckInFKTable(short RankId)
        {
            return await _iRankDB.RankIdCheckInFKTable(RankId);
        }
        public async Task<DTODataTablesResponse<DTORankResponse>> GetAllRank_Pagination(DTODataTablesRequest dTO)
        {
            return await _iRankDB.GetAllRank_Pagination(dTO);
        }

    }
}
