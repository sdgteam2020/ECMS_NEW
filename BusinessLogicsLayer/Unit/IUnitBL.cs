using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Unit
{
    public interface IUnitBL : IGenericRepository<MUnit>
    {

        public Task<bool> GetByName(MUnit Data);
        public Task<MUnit?> GetBySusNo(string Sus_no); 
        public Task<List<MUnit>> GetAllUnit(string Sus_no);
        public Task<bool> FindSusNo(string Sus_no);
    }
}

