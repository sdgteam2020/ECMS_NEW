using BusinessLogicsLayer.Bde;
using DapperRepo.Core.Constants;
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

namespace BusinessLogicsLayer.BdeCate
{
    public class TrnICardRequestBL : GenericRepositoryDL<MTrnICardRequest>, ITrnICardRequestBL
    {
        private readonly ITrnICardRequestDB _iTrnICardRequestDB;


        public TrnICardRequestBL(ApplicationDbContext context, ITrnICardRequestDB iTrnICardRequestDB) : base(context)
        {
            _iTrnICardRequestDB = iTrnICardRequestDB;
        }

        public async Task<MTrnICardRequest> GetByAspNetUserBy(int AspnetuserId)
        {
           return await _iTrnICardRequestDB.GetByAspNetUserBy(AspnetuserId);
        }
    }
}
