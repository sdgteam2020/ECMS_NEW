using BusinessLogicsLayer.User;
using DapperRepo.Core.Constants;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public class Comd : GenericRepositoryDL<DataTransferObject.Domain.Master.Comd>, IComd
    {
        private readonly IComdDB _iComdDB;

        public Comd(ApplicationDbContext context, IComdDB comdDB) : base(context)
        {
            _iComdDB= comdDB;   
        }

        public Task<IEnumerable<DataTransferObject.Domain.Master.Comd>> GetAllByorder()
        {
            return _iComdDB.GetAllByorder();
        }

        public Task<int> GetByMaxOrder()
        {
            return _iComdDB.GetByMaxOrder();    
        }

        public Task<bool> GetByName(DataTransferObject.Domain.Master.Comd Dto)
        {
            Dto.ComdName = Dto.ComdName.Trim().TrimEnd().TrimStart();    
           return _iComdDB.GetByName(Dto);   
        }

        public async Task<int> OrderByChange(DataTransferObject.Domain.Master.Comd Dto)
        {
            ////Current Order
            int ComdIdnext =await _iComdDB.GetComdIdbyOrderby(Dto.Orderby+1);
            if (ComdIdnext > 0)
            {
             
                ///
                /////Subtraction order no Next Comd
                var datanext = await Get(ComdIdnext);
                datanext.Orderby = Dto.Orderby;
                await Update(datanext);

                ////////Change Order No For Click
                DataTransferObject.Domain.Master.Comd data = new DataTransferObject.Domain.Master.Comd();
                data = await Get(Dto.ComdId);
                data.Orderby = Dto.Orderby + 1;
                await Update(data);
                /////////////////////////
            }
            return KeyConstants.Success;
        }
    }
}
