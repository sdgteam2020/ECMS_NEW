using BusinessLogicsLayer.BdeCat;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.BdeCate
{
    public class BdeCatBL : GenericRepositoryDL<MBdeCat>, IBdeCatBL
    {
        private readonly IBdeCatDB _iBdeCatDB;

        public BdeCatBL(ApplicationDbContext context, IBdeCatDB BdeCatDB) : base(context)
        {
            _iBdeCatDB = BdeCatDB;
        }

        public Task<List<DTOBdeCatResponse>> GetALLBdeCat()
        {
            return _iBdeCatDB.GetALLBdeCat();
        }

        public Task<bool> GetByName(MBdeCat Data)
        {
           return _iBdeCatDB.GetByName(Data);
        }
    }
}
