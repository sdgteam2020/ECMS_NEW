using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
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
    public interface IComdBL : IGenericRepositoryDL<DataTransferObject.Domain.Master.MComd>
    {
        public Task<bool> GetByName(DataTransferObject.Domain.Master.MComd Dto);
        public Task<int> GetByMaxOrder(); 
        public Task<DTOGenericResponse<string>> OrderByChange(DataTransferObject.Domain.Master.MComd Dto);
        public Task<IEnumerable<DataTransferObject.Domain.Master.MComd>> GetAllByorder();
        public Task<DTOTreeViewUnitResponse> GetBinaryTree(int Id);
        public Task<DTOComdIdCheckInFKTableResponse?> ComdIdCheckInFKTable(byte ComdId);
        public Task<DTODataTablesResponse<DTOAllCommand_PaginationResponse>> GetAllCommand_Pagination(DTODataTablesRequest dTO);
    }
}
