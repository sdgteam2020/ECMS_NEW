using BusinessLogicsLayer.Bde;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Response;
using DataTransferObject.Requests;

namespace BusinessLogicsLayer.Div
{
    public class DivBL : GenericRepositoryDL<MDiv>, IDivBL
    {
        private readonly IDivDB _DivDB;

        public DivBL(ApplicationDbContext context, IDivDB sqnDB) : base(context)
        {
            _DivDB = sqnDB;
        }
        public Task<List<DTODivResponse>> GetALLDiv()
        {
            return _DivDB.GetALLDiv();
        }

        public Task<List<DTODivResponse>> GetByHId(DTOMHierarchyRequest Data)
        {
            return _DivDB.GetByHId(Data);
        }

        public Task<bool> GetByName(MDiv Data)
        {
            return _DivDB.GetByName(Data);
        }
    }
}
