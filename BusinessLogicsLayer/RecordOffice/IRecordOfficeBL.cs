using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.RecordOffice
{
    public interface IRecordOfficeBL : IGenericRepository<MRecordOffice>
    {
        public Task<int> GetByName(MRecordOffice Dto);
        public Task<bool> GetByTDMId(int UnitId, int? TDMId);
        public Task<DTOGetROByUserIdResponse?> GetROByUserId(int UserId);
        public Task<List<DTORecordOfficeResponse>?> GetAllData();
        public Task<DTOGetUpdateRecordOfficeResponse?> GetUpdateRecordOffice(int TDMId);
        public Task<List<DTOGetMappedForRecordResponse>?> GetDDMappedForRecord(int UnitMapId);
        public Task<bool?> UpdateROValue(DTOUpdateROValueRequest dTO);
    }
}
