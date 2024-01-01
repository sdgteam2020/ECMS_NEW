using BusinessLogicsLayer.User;
using DapperRepo.Core.Constants;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public class MasterBL : IMasterBL
    {
        private readonly IMasterDB _iMasterDB;

        public MasterBL(IMasterDB iMasterDB)
        {
            _iMasterDB = iMasterDB;   
        }

        public async Task<List<DTORemarksResponse>> GetRemarksByTypeId(DTORemarksRequest Data)
        {
          return  await _iMasterDB.GetRemarksByTypeId(Data);
        }
    }
}
