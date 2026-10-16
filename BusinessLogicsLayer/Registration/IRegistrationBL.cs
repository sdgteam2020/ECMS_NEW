using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Registration
{
    public interface IRegistrationBL : IGenericRepositoryDL<DataTransferObject.Domain.Master.MRegistration>
    {
        public Task<DTOGenericResponse<List<MRegistration>>> GetByApplyFor(MRegistration Data);
        public Task<DTOApplyCardDetailsResponse> GetApplyCardDetails(DTOApplyCardDetailsRequest Data);
        public Task<List<MArmyPrefixRule>> GetArmyPrefixRules(DTOApplyForRequest Data);
        public Task<List<MArmyPrefixRule>> GetAllArmyPrefixRules();

    }
}
