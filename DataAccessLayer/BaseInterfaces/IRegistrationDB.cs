using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IRegistrationDB : IGenericRepositoryDL<MRegistration>
    {
       
        public Task<DTOGenericResponse<List<MRegistration>>> GetByApplyFor(MRegistration Data);
        public Task<DTOApplyCardDetailsResponse> GetApplyCardDetails(DTOApplyCardDetailsRequest Data);
        public Task<List<MArmyPrefixRule>> GetArmyPrefixRules(DTOApplyForRequest Data);
        public Task<List<MArmyPrefixRule>> GetAllArmyPrefixRules();
    }
}
