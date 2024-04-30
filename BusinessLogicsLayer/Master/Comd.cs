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
    public class Comd : GenericRepositoryDL<DataTransferObject.Domain.Master.MComd>, IComd
    {
        private readonly IComdDB _iComdDB;

        public Comd(ApplicationDbContext context, IComdDB comdDB) : base(context)
        {
            _iComdDB= comdDB;   
        }

        public Task<IEnumerable<DataTransferObject.Domain.Master.MComd>> GetAllByorder()
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

        public Task<bool> GetByName(DataTransferObject.Domain.Master.MComd Dto)
        {
            Dto.ComdName = Dto.ComdName.Trim().TrimEnd().TrimStart();    
           return _iComdDB.GetByName(Dto);   
        }

        public async Task<byte> OrderByChange(DataTransferObject.Domain.Master.MComd Dto)
        {
            ////Current Order
            byte ComdIdnext =await _iComdDB.GetComdIdbyOrderby(Dto.Orderby+1);
            if (ComdIdnext > 0)
            {
                ///
                /////Subtraction order no Next Comd
                var datanext = await GetByByte(ComdIdnext);
                datanext.Orderby = Dto.Orderby;
                await Update(datanext);

                ////////Change Order No For Click
                DataTransferObject.Domain.Master.MComd data = new DataTransferObject.Domain.Master.MComd();
                data = await GetByByte(Dto.ComdId);
                data.Orderby = Dto.Orderby + 1;
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
