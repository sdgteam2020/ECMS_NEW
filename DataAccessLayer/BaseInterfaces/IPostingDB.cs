using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IPostingDB
    {
        public Task<DTOPostingInResponse> GetArmyDataForPostingIn(string ArmyNo);
    }
}
