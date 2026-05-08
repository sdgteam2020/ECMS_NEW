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
using DataTransferObject.Requests;

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

        public async Task<bool> AddDataExport(DTODataExported Data)
        {
            return await _iTrnLoginLogDB.AddDataExport(Data);
        }

        public Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId)
        {
            return _iTrnLoginLogDB.GetAllUserByUnitId(UnitId);
        }

        public Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId, int UnitId, DateTime? FmDate, DateTime? ToDate)
        {
            if(FmDate==null)
                FmDate=DateTime.Now;
            if (ToDate == null)
                ToDate = DateTime.Now;

            return _iTrnLoginLogDB.GetLoginLogByUserId(AspnetUserId, UnitId, FmDate, ToDate);
        }

        public Task<bool> XmlFileDigitalSign(DTOXmlFilesFwdLogRequest Data)
        {
            return _iTrnLoginLogDB.XmlFileDigitalSign(Data);
        } 
        public Task<DTOXmlFilesFwdLogRequest> XmlFileDigitalSignFromData(int[] RequestId)
        {
            return _iTrnLoginLogDB.XmlFileDigitalSignFromData(RequestId);
        }
        public async Task<TrnLogin_Log?> GetByToken(Guid loginGuid)
        {
            return await _iTrnLoginLogDB.GetByToken(loginGuid);
        }
        public async Task<bool> Update(TrnLogin_Log Data)
        {
            return await _iTrnLoginLogDB.Update(Data);
        }
    }
}
