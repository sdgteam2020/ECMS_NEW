using Azure.Core;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
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
    public class ICardHoldDB : GenericRepositoryDL<MTrnICardHold>, IICardHoldDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<MTrnICardHold> _logger;
        public ICardHoldDB(ApplicationDbContext context, DapperContext contextDP, ILogger<MTrnICardHold> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }
        public async Task<bool> GetByRequestId(MTrnICardHold dTO)
        {
            if(dTO.ICardHoldId == 0)
            {
                var ret = await _context.MTrnICardHold.AnyAsync(x => x.RequestId == dTO.RequestId);
                return ret;
            }
            else
            {
                var ret = await _context.MTrnICardHold.AnyAsync(x => x.RequestId == dTO.RequestId && x.ICardHoldId != dTO.ICardHoldId);
                return ret;
            }

        }
    }
}
