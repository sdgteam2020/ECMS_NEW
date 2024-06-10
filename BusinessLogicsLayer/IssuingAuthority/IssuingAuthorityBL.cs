using BusinessLogicsLayer.Corps;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.IssuingAuthority
{
    public class IssuingAuthorityBL : GenericRepositoryDL<MIssuingAuthority>, IIssuingAuthorityBL
    {
        private readonly IIssuingAuthorityDB _iIssuingAuthorityDB;

        public IssuingAuthorityBL(ApplicationDbContext context, IIssuingAuthorityDB iIssuingAuthorityDB) : base(context)
        {
            _iIssuingAuthorityDB = iIssuingAuthorityDB;
        }
        public async Task<List<DTOIssuingAuthorityResponse>> GetByApplyForId(byte ApplyForId)
        {
            return await _iIssuingAuthorityDB.GetByApplyForId(ApplyForId);
        }
    }
}
