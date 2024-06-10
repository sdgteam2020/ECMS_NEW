using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class IssuingAuthorityDB : GenericRepositoryDL<MIssuingAuthority>, IIssuingAuthorityDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<IssuingAuthorityDB> _logger;
        public IssuingAuthorityDB(ApplicationDbContext context, DapperContext contextDP, ILogger<IssuingAuthorityDB> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }
        public async Task<List<DTOIssuingAuthorityResponse>> GetByApplyForId(byte ApplyForId)
        {
            var data = await (from m in _context.MIssuingAuthority
                               join mapp in _context.MApplyFor
                               on m.ApplyForId equals mapp.ApplyForId
                               where m.ApplyForId == ApplyForId
                               select new DTOIssuingAuthorityResponse
                               {
                                   IssuingAuthorityId = m.IssuingAuthorityId,
                                   IssuingAuthorityName = m.Name,
                               }).ToListAsync();
            return data;
        }
    }
}
