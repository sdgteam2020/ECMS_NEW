using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Posting
{
    public interface IPostingBL
    {
        public Task<DTOPostingInResponse> GetArmyDataForPostingIn(string ArmyNo);
    }
}
