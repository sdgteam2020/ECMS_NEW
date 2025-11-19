using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Registration
{
    public interface IRegistrationBL : IGenericRepositoryDL<DataTransferObject.Domain.Master.MRegistration>
    {
        public Task<List<MRegistration>> GetByApplyFor(MRegistration Data);
        public Task<DTOApplyCardDetailsResponse> GetApplyCardDetails(DTOApplyCardDetailsRequest Data);
    }
}
