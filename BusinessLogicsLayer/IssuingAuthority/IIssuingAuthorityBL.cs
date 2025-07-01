using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.IssuingAuthority
{
    public interface IIssuingAuthorityBL : IGenericRepositoryDL<MIssuingAuthority>
    {
        public Task<List<DTOIssuingAuthorityResponse>> GetByApplyForId(byte ApplyForId);
    }
}
