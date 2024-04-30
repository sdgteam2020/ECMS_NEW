using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public interface IComd : IGenericRepository<DataTransferObject.Domain.Master.MComd>
    {

        public Task<bool> GetByName(DataTransferObject.Domain.Master.MComd DTo);
        public Task<int> GetByMaxOrder(); 
        public Task<byte> OrderByChange(DataTransferObject.Domain.Master.MComd DTo);
        public Task<IEnumerable<DataTransferObject.Domain.Master.MComd>> GetAllByorder();
        public Task<DTOTreeViewUnitResponse> GetBinaryTree(int Id);
        public Task<DTOComdIdCheckInFKTableResponse?> ComdIdCheckInFKTable(byte ComdId);
    }
}
