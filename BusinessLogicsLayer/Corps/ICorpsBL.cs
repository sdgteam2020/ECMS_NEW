using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Corps
{
    public interface ICorpsBL : IGenericRepository<MCorps>
    {

        public Task<bool> GetByName(MCorps Data);
        public Task<List<DTOCorpsResponse>> GetByComdId(int ComdId);
        public Task<List<DTOCorpsResponse>> GetALLCorps();
    }
}
