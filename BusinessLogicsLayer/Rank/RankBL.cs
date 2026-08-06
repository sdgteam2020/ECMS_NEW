using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogicsLayer.Rank
{
    public class RankBL : GenericRepositoryDL<MRank>, IRankBL
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly ILogger<RankBL> _logger;
        private readonly IRankDB _iRankDB;

        public RankBL(ApplicationDbContext context, IRankDB iRankDB, ILogger<RankBL> logger) : base(context)
        {
            _context = context;
            _iRankDB = iRankDB;
            _logger = logger;
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

        public async Task<DTOGenericResponse<string>> OrderByChange(MRank dto)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            response.Result = false;
            response.Value = string.Empty;
            if (dto == null || dto.RankId <= 0)
            {
                response.Message = "Invalid Rank data.";
                return response;
            }
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                MRank? currentRank = await GetByGen<short>(dto.RankId);

                if (currentRank == null)
                {
                    await transaction.RollbackAsync();
                    response.Message = "Rank Not found.";
                    return response;
                }


                short currentOrder = currentRank.Orderby;
                int nextOrder = currentOrder;

                MRank? nextRank = null;

                while (nextOrder < short.MaxValue)
                {
                    nextOrder++;

                    short nextRankId = await _iRankDB.GetRankIdbyOrderby((short)nextOrder);

                    if (nextRankId <= 0)
                        continue;

                    nextRank = await GetByGen<short>(nextRankId);

                    if (nextRank != null)
                        break;
                }

                // No rank exists after the selected rank
                if (nextRank == null)
                {
                    await transaction.RollbackAsync();
                    response.Message = "No rank exists after the selected rank.";
                    return response;
                }


                // Swap both Orderby values
                nextRank.Orderby = currentOrder;
                currentRank.Orderby = (short)nextOrder;

                // Mark both records as modified
                _context.Entry(nextRank).State = EntityState.Modified;
                _context.Entry(currentRank).State = EntityState.Modified;

                // Save both updates together
                await SaveAsync();

                // Commit only after both records are successfully updated
                await transaction.CommitAsync();

                response.Message = "Rank order update sccessful";
                response.Result = true;
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(1001, ex, "RankBL->OrderByChange");
                response.Message = "Internal Server Error.";
                return response;
            }

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
