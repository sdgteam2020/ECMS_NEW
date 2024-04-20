using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicsLayer.Master;
using DataTransferObject.Requests;

namespace BusinessLogicsLayer.RecordOffice
{
    public class RecordOfficeBL : GenericRepositoryDL<MRecordOffice>, IRecordOfficeBL
    {
        private readonly IRecordOfficeDB _RecordOfficeDB;

        public RecordOfficeBL(ApplicationDbContext context, IRecordOfficeDB iRecordOfficeDB) : base(context)
        {
            _RecordOfficeDB = iRecordOfficeDB;
        }

        public Task<List<DTORecordOfficeResponse>> GetAllData()
        {
            return _RecordOfficeDB.GetAllData();
        }
        public async Task<bool> GetByTDMId(int TDMId)
        {
            return await _RecordOfficeDB.GetByTDMId(TDMId);
        }

        public async Task<int> GetByName(MRecordOffice Dto)
        {
            Dto.Name = Dto.Name.Trim().TrimEnd().TrimStart();
            return await _RecordOfficeDB.GetByName(Dto);
        }
        public async Task<DTOGetUpdateRecordOfficeResponse?> GetUpdateRecordOffice(int TDMId)
        {
            return await _RecordOfficeDB.GetUpdateRecordOffice(TDMId);
        }
        public async Task<List<DTOGetMappedForRecordResponse>?> GetDDMappedForRecord(int UnitMapId)
        {
            return await _RecordOfficeDB.GetDDMappedForRecord(UnitMapId);
        }
        public async Task<bool?> UpdateROValue(DTOUpdateROValueRequest dTO)
        {
            return await _RecordOfficeDB.UpdateROValue(dTO);
        }
    }
}
