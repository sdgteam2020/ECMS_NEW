using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IRecordOfficeDB : IGenericRepositoryDL<MRecordOffice>
    {
        public Task<int> GetByName(MRecordOffice Dto);
        public Task<bool> GetByTDMId(int TDMId);
        public Task<DTOGetUpdateRecordOfficeResponse?> GetUpdateRecordOffice(int TDMId);
        public Task<List<DTORecordOfficeResponse>> GetAllData();
        public Task<List<DTOGetMappedForRecordResponse>?> GetDDMappedForRecord(int UnitMapId);
        public Task<bool?> UpdateROValue(DTOUpdateROValueRequest dTO);
    }
}
