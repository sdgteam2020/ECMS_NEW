using BusinessLogicsLayer.User;
using DapperRepo.Core.Constants;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public class Comd : GenericRepositoryDL<MComd>, IComd
    {
        private readonly IComdDB _iComdDB;

        public Comd(ApplicationDbContext context, IComdDB comdDB) : base(context)
        {
            _iComdDB= comdDB;   
        }

        public Task<IEnumerable<MComd>> GetAllByorder()
        {
            return _iComdDB.GetAllByorder();
        }

        public Task<DTOTreeViewUnitResponse> GetBinaryTree(int Id)
        {
            return _iComdDB.GetBinaryTree(Id);
        }

        public Task<int> GetByMaxOrder()
        {
            return _iComdDB.GetByMaxOrder();    
        }

        public Task<bool> GetByName(MComd Dto)
        {
            Dto.ComdName = Dto.ComdName.Trim().TrimEnd().TrimStart();    
           return _iComdDB.GetByName(Dto);   
        }

        public async Task<byte> OrderByChange(MComd Dto)
        {
            ////Current Order
            int i = Dto.Orderby;
            increment:
            i++;
            byte ComdIdnext =await _iComdDB.GetComdIdbyOrderby(i);
            if (ComdIdnext == 0)
            {
                goto increment;
            }
            else 
            {
                ///
                /////Subtraction order no Next Comd
                var datanext = await GetByByte(ComdIdnext);
                datanext.Orderby = Dto.Orderby;
                await Update(datanext);

                ////////Change Order No For Click
                MComd data = new MComd();
                data = await GetByByte(Dto.ComdId);
                data.Orderby = i;
                await Update(data);
                /////////////////////////

            }
            return KeyConstants.Success;
        }
        public async Task<DTOComdIdCheckInFKTableResponse?> ComdIdCheckInFKTable(byte ComdId)
        {
            return await _iComdDB.ComdIdCheckInFKTable(ComdId);
        }
    }
}
