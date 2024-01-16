using BusinessLogicsLayer.Bde;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using BusinessLogicsLayer.Unit;

namespace BusinessLogicsLayer.TrnLoginLog
{
    public class TrnLoginLogBL : GenericRepositoryDL<TrnLogin_Log>, ITrnLoginLogBL
    {
        private readonly ITrnLoginLogDB _iTrnLoginLogDB;


        public TrnLoginLogBL(ApplicationDbContext context, ITrnLoginLogDB iTrnLoginLogDB) : base(context)
        {
            _iTrnLoginLogDB = iTrnLoginLogDB;
        }
    
        public Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId)
        {
            return _iTrnLoginLogDB.GetAllUserByUnitId(UnitId);
        }

        public Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId)
        {
            return _iTrnLoginLogDB.GetLoginLogByUserId(AspnetUserId);
        }
    }
}
