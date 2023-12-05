using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class TrnICardRequestDB : GenericRepositoryDL<MTrnICardRequest>, ITrnICardRequestDB
    {
        protected readonly ApplicationDbContext _context;
        public TrnICardRequestDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<MTrnICardRequest> GetByAspNetUserBy(int AspnetuserId)
        {
            return null;// await _context.TrnICardRequest.Where(P => P.TrnDomainMappingId == AspnetuserId).ToListAsync();
        }
    }
}
