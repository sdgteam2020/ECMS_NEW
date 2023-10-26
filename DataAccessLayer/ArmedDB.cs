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
    public class ArmedDB : GenericRepositoryDL<MArmedType>, IArmedDB
    {
        protected readonly ApplicationDbContext _context;
        public ArmedDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
      

      
        private readonly IConfiguration configuration;
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

       

         public async Task<bool> GetByName(MArmedType DTo)
         {
            // && p.ComdId != DTo.ComdId && p.IsDeleted==true
            var ret = _context.MArmedType.Where(P=>P.ArmedId!=DTo.ArmedId).Select(p => p.ArmedName.ToUpper() == DTo.ArmedName.ToUpper()).FirstOrDefault();
            return ret;
        }
    }
}