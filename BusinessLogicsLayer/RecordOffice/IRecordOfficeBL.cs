using DataTransferObject.Domain.Master;
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
        public Task<bool> GetByName(MRecordOffice Dto);
        public Task<List<DTORecordOfficeResponse>> GetAllData();
    }
}
