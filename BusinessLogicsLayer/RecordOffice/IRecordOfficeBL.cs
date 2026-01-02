using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.RecordOffice
{
    public interface IRecordOfficeBL : IGenericRepositoryDL<MRecordOffice>
    {
        public Task<int> GetByName(MRecordOffice Dto);
        public Task<bool> GetByTDMId(int UnitId, int? TDMId);
        public Task<DTOGetROByTDMIdResponse?> GetROByTDMId(int TDMId);
        public Task<List<DTORecordOfficeResponse>?> GetAllData();
        public Task<DTOGetUpdateRecordOfficeResponse?> GetUpdateRecordOffice(int RecordOfficeId);
        public Task<List<DTOGetMappedForRecordResponse>?> GetDDMappedForRecord(int UnitMapId);
        public Task<bool?> UpdateROValue(DTOUpdateROValueRequest dTO);
        public Task<DTODataTablesResponse<DTORecordOfficeResponse>> GetAllRecordOffice_Pagination(DTODataTablesRequest dTO);
    }
}
