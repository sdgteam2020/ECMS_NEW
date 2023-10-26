using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ICorpsDB : IGenericRepositoryDL<MCorps>
    {
        public Task<bool> GetByName(MCorps Data);
        public Task<List<DTOCorpsResponse>> GetALLCorps();
        public Task<List<DTOCorpsResponse>> GetByComdId(int ComdId);
    }
}
