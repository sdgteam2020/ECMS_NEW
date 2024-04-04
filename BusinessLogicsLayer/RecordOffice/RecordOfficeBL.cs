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

        public Task<bool> GetByName(MRecordOffice Dto)
        {
            Dto.Name = Dto.Name.Trim().TrimEnd().TrimStart();
            return _RecordOfficeDB.GetByName(Dto);
        }
    }
}
