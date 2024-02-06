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
    public class RankDB : GenericRepositoryDL<MRank>, IRankDB
    {
        protected readonly ApplicationDbContext _context;
        public RankDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
      

      
        private readonly IConfiguration configuration;
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

       

         public async Task<bool> GetByName(MRank DTo)
         {
            // && p.ComdId != DTo.ComdId && p.IsDeleted==true
            var ret = _context.MRank.Select(p => p.RankAbbreviation.ToUpper() == DTo.RankAbbreviation.ToUpper()).FirstOrDefault();
            return ret;
        }

        public async Task<short> GetByMaxOrder()
        {
            short ret = _context.MRank.Max(P => P.Orderby);
            return (short)(ret + 1);
        }

        public async Task<int> GetRankIdbyOrderby(short OrderBy)
        {
            var ret= _context.MRank.Where(P => P.Orderby == OrderBy).Select(c=>c.RankId).SingleOrDefault(); 
           
            return ret;
        }

        public async Task<IEnumerable<MRank>> GetAllByorder()
        {
            var ret=  _context.MRank.OrderBy(x => x.Orderby).ToList();   
            return ret;
        }
        public async Task<IEnumerable<MRank>> GetAllByType(int Type)
        {
            var ret=  _context.MRank.Where(x => x.ApplyForId==Type).ToList();   
            return ret;
        }
    }
}