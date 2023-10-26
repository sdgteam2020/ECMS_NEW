using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class FormationDB : GenericRepositoryDL<MFormation>, IFormationDB
    {
        protected readonly ApplicationDbContext _context;
        public FormationDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MFormation Dto)
        {
            var ret =_context.MFormation.Select(p => p.FormationName.ToUpper() == Dto.FormationName.ToUpper()).FirstOrDefault();
            return ret;
        }
    }
}