using BusinessLogicsLayer.User;
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

        public Task<bool> GetByName(DataTransferObject.Domain.Master.Comd Dto)
        {
            Dto.ComdName = Dto.ComdName.Trim().TrimEnd().TrimStart();    
           return _iComdDB.GetByName(Dto);   
        }
    }
}
