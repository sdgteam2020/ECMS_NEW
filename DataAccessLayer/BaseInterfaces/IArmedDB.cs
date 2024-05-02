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
    public interface IArmedDB : IGenericRepositoryDL<MArmedType>
    {
        public Task<bool> GetByName(MArmedType Dto);
        public Task<List<DTOArmedResponse>> GetALLArmed();
        public Task<DTOArmedIdCheckInFKTableResponse?> ArmedIdCheckInFKTable(byte ArmedId);
    }
}
