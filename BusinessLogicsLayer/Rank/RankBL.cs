using BusinessLogicsLayer.Bde;
using DapperRepo.Core.Constants;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public Task<int> GetByMaxOrder()
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
            int ComdIdnext = await _iRankDB.GetRankIdbyOrderby(Dto.Orderby + 1);
            if (ComdIdnext > 0)
            {

                ///
                /////Subtraction order no Next Comd
                var datanext = await Get(ComdIdnext);
                datanext.Orderby = Dto.Orderby;
                await Update(datanext);

                ////////Change Order No For Click
                MRank data = new MRank();
                data = await Get(Dto.RankId);
                data.Orderby = Convert.ToInt16(Dto.Orderby + 1);
                await Update(data);
                /////////////////////////
            }
            return KeyConstants.Success;
        }

    }
}
