using DataTransferObject.Domain.Master;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Master
{
    public interface IArmedBL : IGenericRepository<MArmedType>
    {

        public Task<bool> GetByName(MArmedType Dto);
        public Task<List<DTOArmedResponse>> GetALLArmed();
        public Task<DTOArmedIdCheckInFKTableResponse?> ArmedIdCheckInFKTable(byte ArmedId);
    }
}
