using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Migrations;
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

namespace BusinessLogicsLayer.BdeCate
{
    public class APIDataBL : GenericRepositoryDL<MApiData>, IAPIDataBL
    {
        private readonly IAPIDataDB aPIDataDB;

        public APIDataBL(ApplicationDbContext context, IAPIDataDB aPIDataDB) : base(context)
        {
            this.aPIDataDB = aPIDataDB;
        }

        public Task<MApiData> GetByIC(string ICNo)
        {
           return aPIDataDB.GetByIC(ICNo);
        }
    }
}
