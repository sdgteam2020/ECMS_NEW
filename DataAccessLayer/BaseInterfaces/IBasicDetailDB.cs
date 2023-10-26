using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IBasicDetailDB:IGenericRepositoryDL<BasicDetail>
    {
        public Task<List<DTOBasicDetailRequest>> GetALLBasicDetail(int UserId);
    }
}
