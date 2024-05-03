using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class RankDB : GenericRepositoryDL<MRank>, IRankDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<RankDB> _logger;
        public RankDB(ApplicationDbContext context, ILogger<RankDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _logger = logger;
            _contextDP = contextDP;
        }
      

      
        private readonly IConfiguration configuration;
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

       

         public async Task<bool> GetByName(MRank Dto)
         {
            var ret = await _context.MRank.AnyAsync(p =>( p.RankAbbreviation.ToUpper() == Dto.RankAbbreviation.ToUpper() || p.RankName.ToUpper() == Dto.RankName) && p.RankId != Dto.RankId);
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
            var ret=  _context.MRank.Where(x => x.ApplyForId==Type && x.IsActive==true).ToList();   
            return ret;
        }
        public async Task<DTORankIdCheckInFKTableResponse?> RankIdCheckInFKTable(short RankId)
        {
            try
            {
                string query = "Select  count(distinct bd.BasicDetailId) as TotalBD, count(distinct bdt.BasicDetailTempId) as TotalBDT, count(distinct up.UserId) as TotalUP from MRank mrk" +
                                " left join BasicDetails bd on bd.RankId =mrk.RankId " +
                                " left join BasicDetailTemps bdt on bdt.RankId = mrk.RankId " +
                                " left join UserProfile up on up.RankId = mrk.RankId " +
                                " where mrk.RankId =@RankId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTORankIdCheckInFKTableResponse>(query, new { RankId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RankDB->RankIdCheckInFKTable");
                return null;
            }
        }
    }
}