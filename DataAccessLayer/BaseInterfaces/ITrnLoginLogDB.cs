using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ITrnLoginLogDB
    {
        public Task<bool> Add(TrnLogin_Log Data);
        public Task<bool> AddDataExport(DTODataExported Data);
        public Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId);
        public Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId, DateTime? FmDate, DateTime? ToDate);
        public Task<bool> XmlFileDigitalSign(DTOXmlFilesFwdLogRequest Data);
    }
}
