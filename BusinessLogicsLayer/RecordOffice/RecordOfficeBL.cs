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

        public async Task<List<DTORecordOfficeResponse>?> GetAllData()
        {
            return await _RecordOfficeDB.GetAllData();
        }
        public async Task<bool> GetByTDMId(int UnitId, int? TDMId)
        {
            return await _RecordOfficeDB.GetByTDMId(UnitId,TDMId);
        }

        public async Task<int> GetByName(MRecordOffice Dto)
        {
            Dto.Name = Dto.Name.Trim().TrimEnd().TrimStart();
            return await _RecordOfficeDB.GetByName(Dto);
        }
        public async Task<DTOGetUpdateRecordOfficeResponse?> GetUpdateRecordOffice(int RecordOfficeId)
        {
            return await _RecordOfficeDB.GetUpdateRecordOffice(RecordOfficeId);
        }
        public async Task<List<DTOGetMappedForRecordResponse>?> GetDDMappedForRecord(int UnitMapId)
        {
            return await _RecordOfficeDB.GetDDMappedForRecord(UnitMapId);
        }
        public async Task<bool?> UpdateROValue(DTOUpdateROValueRequest dTO)
        {
            return await _RecordOfficeDB.UpdateROValue(dTO);
        }
        public async Task<DTOGetROByUserIdResponse?> GetROByUserId(int UserId)
        {
            return await _RecordOfficeDB.GetROByUserId(UserId);
        }
    }
}
