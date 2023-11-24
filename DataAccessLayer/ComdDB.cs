using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class ComdDB : GenericRepositoryDL<MComd>, IComdDB
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

       

         public async Task<bool> GetByName(MComd DTo)
         {
            // && p.ComdId != DTo.ComdId && p.IsDeleted==true
            var ret = _context.MComd.Select(p => p.ComdName.ToUpper() == DTo.ComdName.ToUpper()).FirstOrDefault();
            return ret;
        }

        public async Task<int> GetByMaxOrder()
        {
            int ret = _context.MComd.Max(P => P.Orderby);
            return ret+1;
        }

        public async Task<int> GetComdIdbyOrderby(int OrderBy)
        {
            var ret= _context.MComd.Where(P => P.Orderby == OrderBy).Select(c=>c.ComdId).SingleOrDefault(); 
           
            return ret;
        }

        public async Task<IEnumerable<MComd>> GetAllByorder()
        {
            var ret=  _context.MComd.OrderBy(x => x.Orderby).ToList();   
            return ret;
        }
    }
}