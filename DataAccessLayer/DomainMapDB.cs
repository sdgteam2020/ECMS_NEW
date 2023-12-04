using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DomainMapDB : GenericRepositoryDL<TrnDomainMapping>, IDomainMapDB
    {
        protected readonly ApplicationDbContext _context;
        public DomainMapDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> GetByDomainId(TrnDomainMapping Data)
        {
            var ret = _context.TrnDomainMapping.Any(p => p.AspNetUsersId == Data.AspNetUsersId);
            return ret;
        }

        public async Task<TrnDomainMapping> GetByDomainIdbyUnit(TrnDomainMapping Data)
        {
            return await _context.TrnDomainMapping.Where(p => p.AspNetUsersId == Data.AspNetUsersId).SingleOrDefaultAsync();
        }

        
    }
}
