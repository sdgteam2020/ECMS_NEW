using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDet
{
    public interface IBasicDetailBL:IGenericRepository<BasicDetail>
    {
        public Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId,int step,int type);
      
    }
}
