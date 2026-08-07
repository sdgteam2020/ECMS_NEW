using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.TrnLoginLog
{
   public interface ITrnLoginLogBL
   {

        public Task<bool> Add(TrnLogin_Log Data);
        public Task<bool> AddDataExport(DTODataExported Data);
        public Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId);
        public Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId, int UnitId, DateTime? FmDate, DateTime? ToDate);
        public Task<bool> XmlFileDigitalSign(DTOXmlFilesFwdLogRequest Data);
        public Task<DTOXmlFilesFwdLogRequest> XmlFileDigitalSignFromData(int RequestId);

        public Task<TrnLogin_Log?> GetByToken(Guid loginGuid);
        public Task<bool> Update(TrnLogin_Log Data);
    }
}
