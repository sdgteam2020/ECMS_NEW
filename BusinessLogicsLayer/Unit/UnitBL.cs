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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.Unit
{
    public class UnitBL : GenericRepositoryDL<MUnit>, IUnitBL
    {
        private readonly IUnitDB _UnitDB;

        public UnitBL(ApplicationDbContext context, IUnitDB UnitDB) : base(context)
        {
            _UnitDB = UnitDB;
        }

        public Task<List<MUnit>> GetAllUnit(string Sus_no)
        {
            return _UnitDB.GetAllUnit(Sus_no);
        }

        public Task<bool> GetByName(MUnit Data)
        {
            return _UnitDB.GetByName(Data); 
        }

        public Task<MUnit> GetBySusNo(MUnit Data)
        {
            return _UnitDB.GetBySusNo(Data.Sus_no+Data.Suffix);
        }
        public async Task<bool> FindSusNo(string Sus_no)
        {
            return await _UnitDB.FindSusNo(Sus_no);
        }
    }
}
