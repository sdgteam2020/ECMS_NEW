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
    public class TrnLoginLogBL : ITrnLoginLogBL
    {
        private readonly ITrnLoginLogDB _iTrnLoginLogDB;


        public TrnLoginLogBL(ITrnLoginLogDB iTrnLoginLogDB)
        {
            _iTrnLoginLogDB = iTrnLoginLogDB;
        }

        public async Task<bool> Add(TrnLogin_Log Data)
        {
            return await _iTrnLoginLogDB.Add(Data);
        }

        public Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId)
        {
            return _iTrnLoginLogDB.GetAllUserByUnitId(UnitId);
        }

        public Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId, DateTime? FmDate, DateTime? ToDate)
        {
            if(FmDate==null)
                FmDate=DateTime.Now;
            if (ToDate == null)
                ToDate = DateTime.Now;

            return _iTrnLoginLogDB.GetLoginLogByUserId(AspnetUserId, FmDate, ToDate);
        }
    }
}
