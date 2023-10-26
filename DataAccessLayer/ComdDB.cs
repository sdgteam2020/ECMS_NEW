using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response.User;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class ComdDB : GenericRepositoryDL<Comd>, IComdDB
    {
        protected readonly ApplicationDbContext _context;
        public ComdDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
      

      
        private readonly IConfiguration configuration;
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

       

         public async Task<bool> GetByName(Comd DTo)
         {
            // && p.ComdId != DTo.ComdId && p.IsDeleted==true
            var ret = _context.MComd.Select(p => p.ComdName.ToUpper() == DTo.ComdName.ToUpper()).FirstOrDefault();
            return ret;
        }
    }
}