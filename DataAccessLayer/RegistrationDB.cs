using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class RegistrationDB : GenericRepositoryDL<MRegistration>, IRegistrationDB
    {
        protected readonly ApplicationDbContext _context;
        public RegistrationDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
      
        private readonly IConfiguration configuration;

        public async Task<List<MRegistration>> GetByApplyFor(MRegistration Data)
        {
            var ret =await _context.MRegistration.Where(x => x.ApplyForId == Data.ApplyForId).ToListAsync();
            return ret;
        }
        




    }
}