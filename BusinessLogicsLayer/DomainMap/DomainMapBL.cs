using BusinessLogicsLayer.Bde;
using DapperRepo.Core.Constants;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.BdeCate
{
    public class DomainMapBL : GenericRepositoryDL<TrnDomainMapping>, IDomainMapBL
    {
        private readonly IDomainMapDB _IDomainMapDB;

        public DomainMapBL(ApplicationDbContext context, IDomainMapDB domainMapDB) : base(context)
        {
            _IDomainMapDB = domainMapDB;
        }

        public Task<TrnDomainMapping> GetByAspnetUserIdBy(TrnDomainMapping Data)
        {
            return _IDomainMapDB.GetByAspnetUserIdBy(Data);
        }

        public Task<bool> GetByDomainId(TrnDomainMapping Data)
        {
            return _IDomainMapDB.GetByDomainId(Data);   
        }

        public Task<TrnDomainMapping> GetByDomainIdbyUnit(TrnDomainMapping Data)
        {
            return _IDomainMapDB.GetByDomainIdbyUnit(Data);
        }

        public Task<TrnDomainMapping> GetByRequestId(int RequestId)
        {
            return _IDomainMapDB.GetByRequestId(RequestId);
        }
    }
}
