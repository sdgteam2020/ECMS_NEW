using BusinessLogicsLayer.User;
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

namespace BusinessLogicsLayer.Corps
{
    public class CorpsBL : GenericRepositoryDL<MCorps>, ICorpsBL
    {
        private readonly ICorpsDB _iCorpsDB;

        public CorpsBL(ApplicationDbContext context, ICorpsDB corpsDB) : base(context)
        {
            _iCorpsDB = corpsDB;
        }

        public Task<List<DTOCorpsResponse>> GetALLCorps()
        {
            return _iCorpsDB.GetALLCorps();
        }

        public Task<List<DTOCorpsResponse>> GetByComdId(int ComdId)
        {
            return _iCorpsDB.GetByComdId(ComdId);
        }

        public Task<bool> GetByName(MCorps Data)
        {
           return _iCorpsDB.GetByName(Data);
        }
    }
}
