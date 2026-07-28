using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CompletedICardRequestDB : GenericRepositoryDL<CompletedICardRequest>, ICompletedICardRequestDB
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly ILogger<CompletedICardRequest> _logger;// For logging

        /// <summary>
        /// Constructor to initialize the CompletedICardRequestDB with necessary contexts and logger.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextDP"></param>
        /// <param name="logger"></param>
        public CompletedICardRequestDB(ApplicationDbContext context, DapperContext contextDP, ILogger<CompletedICardRequest> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        public async Task<CompletedICardRequest?> GetByRequestId(int RequestId)
        {
            var ret = await _context.CompletedICardRequests.FirstOrDefaultAsync(x => x.RequestId == RequestId);
            return ret;
        }
    }
}
