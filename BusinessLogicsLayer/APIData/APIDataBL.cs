using BusinessLogicsLayer.APIData;
using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.APIData
{
    public class APIDataBL : GenericRepository<MApiData>, IAPIDataBL
    {
        private readonly IAPIDataDB _aPIDataDB;

        public APIDataBL(IAPIDataDB aPIDataDB) 
        {
            _aPIDataDB = aPIDataDB;
        }

        public Task<MApiData> GetByIC(DTOAPIDataRequest Data)
        {
           return _aPIDataDB.GetByIC(Data);
        }

        public Task<MApiDataOffrs> GetByoffrsIC(DTOAPIDataRequest Data)
        {
            return _aPIDataDB.GetByoffrsIC(Data);
        }
    }
}
