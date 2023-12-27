using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Registration
{
    public interface IRegistrationBL : IGenericRepository<DataTransferObject.Domain.Master.MRegistration>
    {
        public Task<List<MRegistration>> GetByApplyFor(MRegistration Data);
        public Task<DTOApplyCardDetailsResponse> GetApplyCardDetails(DTOApplyCardDetailsRequest Data);
    }
}
