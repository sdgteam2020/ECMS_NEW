using BusinessLogicsLayer.RecordOffice;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.OROMapp
{
    public class OROMappingBL : GenericRepositoryDL<OROMapping>, IOROMappingBL
    {
        private readonly IOROMappingDB _OROMappingDB;
        public OROMappingBL(ApplicationDbContext context, IOROMappingDB iOROMappingDB) : base(context)
        {
            _OROMappingDB = iOROMappingDB;
        }
        public async Task<List<DTOOROMappingResponse>?> GetAllOROMapping()
        {
            return await _OROMappingDB.GetAllOROMapping();
        }
        public async Task<bool> GetByName(OROMapping Dto)
        {
            return await _OROMappingDB.GetByName(Dto);
        }
    }
}
