using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IBasicDetailDB:IGenericRepositoryDL<BasicDetail>
    {
        public Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId,int step, int type);
        public Task<List<DTOICardTypeRequest>> GetAllICardType();
    }
}
