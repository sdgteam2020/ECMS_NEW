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
    public class TrnFwnBL : GenericRepositoryDL<MTrnFwd>, ITrnFwnBL
    {

        private readonly ITrnFwnDB _ITrnFwnDB;

      
        public TrnFwnBL(ApplicationDbContext context, ITrnFwnDB iTrnFwnDB) : base(context)
        {
            _ITrnFwnDB = iTrnFwnDB;
        }

        public Task<bool> UpdateAllBYRequestId(int RequestId)
        {
            return _ITrnFwnDB.UpdateAllBYRequestId(RequestId);
        }
        public async Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId)
        {
            return await _ITrnFwnDB.UpdateFieldBYTrnFwdId(TrnFwdId);
        }
        public async Task<bool?> SaveInternalFwd(DTOSaveInternalFwdRequest dTO)
        {
            return await _ITrnFwnDB.SaveInternalFwd(dTO);
        }
    }
}
