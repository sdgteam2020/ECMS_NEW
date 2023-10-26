using BusinessLogicsLayer.Bde;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Response;
using DataTransferObject.Requests;

namespace BusinessLogicsLayer.Unit
{
    public class UnitBL : GenericRepositoryDL<MUnit>, IUnitBL
    {
        private readonly IUnitDB _UnitDB;

        public UnitBL(ApplicationDbContext context, IUnitDB UnitDB) : base(context)
        {
            _UnitDB = UnitDB;
        }

      
        public Task<bool> GetByName(MUnit Data)
        {
            return _UnitDB.GetByName(Data); 
        }

        public Task<MUnit> GetBySusNo(MUnit Data)
        {
            return _UnitDB.GetBySusNo(Data.Sus_no+Data.Suffix);
        }
    }
}
