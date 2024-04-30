using BusinessLogicsLayer.Bde;
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
    public class BdeBL : GenericRepositoryDL<MBde>, IBdeBL
    {
        private readonly IBdeDB _iBdeCatDB;

        public BdeBL(ApplicationDbContext context, IBdeDB BdeCatDB) : base(context)
        {
            _iBdeCatDB = BdeCatDB;
        }

        public Task<List<DTOBdeResponse>> GetALLBdeCat()
        {
            return _iBdeCatDB.GetALLBdeCat();
        }

        public Task<List<DTOBdeResponse>> GetByHId(DTOMHierarchyRequest Data)
        {
            return _iBdeCatDB.GetByHId(Data);
        }

        public Task<bool?> GetByName(MBde Data)
        {
           return _iBdeCatDB.GetByName(Data);
        }
        public async Task<bool?> FindByBdeWithId(string BdeName, byte BdeId)
        {
            return await _iBdeCatDB.FindByBdeWithId(BdeName, BdeId);
        }
        public async Task<DTOBdeIdCheckInFKTableResponse?> BdeIdCheckInFKTable(byte BdeId)
        {
            return await _iBdeCatDB.BdeIdCheckInFKTable(BdeId);
        }
    }
}
