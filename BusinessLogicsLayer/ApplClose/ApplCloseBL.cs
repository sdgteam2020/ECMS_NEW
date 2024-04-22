using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Posting
{
    public class ApplCloseBL : GenericRepositoryDL<TrnApplClose>, IApplCloseBL
    {


      
        private readonly IApplCloseDB _iApplCloseDB;
        public ApplCloseBL(ApplicationDbContext context, IApplCloseDB iApplCloseDB) : base(context)
        {
            _iApplCloseDB = iApplCloseDB;   
        }

        public async Task<bool> RequestIdExists(TrnApplClose DTo)
        {
          return  await _iApplCloseDB.RequestIdExists(DTo);   
        }
    }
}
