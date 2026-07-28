using BusinessLogicsLayer.Category;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.CompletedICard
{
    public class CompletedICardRequestBL : GenericRepositoryDL<CompletedICardRequest>, ICompletedICardRequestBL
    {
        private readonly ICompletedICardRequestDB _CompletedICardRequestDB;
        public CompletedICardRequestBL(ApplicationDbContext context, ICompletedICardRequestDB completedICardRequestDB) : base(context)
        {
            _CompletedICardRequestDB = completedICardRequestDB;
        }
        public async Task<CompletedICardRequest?> GetByRequestId(int RequestId)
        {
            return await _CompletedICardRequestDB.GetByRequestId(RequestId);
        }
    }
}
