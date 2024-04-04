using DataTransferObject.Domain.Master;
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
        public Task<bool> GetByName(MRecordOffice Dto);
        public Task<List<DTORecordOfficeResponse>> GetAllData();
    }
}
