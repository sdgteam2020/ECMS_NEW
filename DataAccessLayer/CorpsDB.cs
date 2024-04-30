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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class CorpsDB : GenericRepositoryDL<MCorps>, ICorpsDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<CorpsDB> _logger;
        public CorpsDB(ApplicationDbContext context, DapperContext contextDP, ILogger<CorpsDB> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MCorps Data)
        {
            //var ret = _context.MCorps.Where(p=> p.ComdId != Data.ComdId).Select(p => p.CorpsName.ToUpper() == Data.CorpsName.ToUpper()).FirstOrDefault();

            var ret = await _context.MCorps.AnyAsync(p => p.CorpsName.ToUpper() == Data.CorpsName.ToUpper() && p.CorpsId != Data.CorpsId);
            return ret;
        }

        public async Task<List<DTOCorpsResponse>> GetALLCorps()
        {
            var Corps = await (from c in _context.MCorps
                                 join d in _context.MComd
                                 on c.ComdId equals d.ComdId
                                 where c.CorpsId!=1
                                 select new DTOCorpsResponse
                                 {
                                     CorpsId = c.CorpsId,
                                     CorpsName = c.CorpsName,
                                     ComdName = d.ComdName,
                                     ComdId=d.ComdId,
                                 }).ToListAsync();
            return Corps;  
        }

        public async Task<List<DTOCorpsResponse>> GetByComdId(int ComdId)
        {
            var Corps = await (from c in _context.MCorps
                                 join d in _context.MComd
                                 on c.ComdId equals d.ComdId where c.ComdId == ComdId   
                                 select new DTOCorpsResponse
                                 {
                                     CorpsId = c.CorpsId,
                                     CorpsName = c.CorpsName,
                                 }).ToListAsync();
            return Corps;
        }
        public async Task<DTOCorpsIdCheckInFKTableResponse?> CorpsIdCheckInFKTable(byte CorpsId)
        {
            try
            {
                string query = "Select  count(distinct mbd.BdeId) as TotalBde ,count(distinct mdiv.DivId) as TotalDiv,count(distinct mapunit.UnitMapId) as TotalMapUnit from MCorps mcor" +
                                " left join MBde mbd on mbd.CorpsId = mcor.CorpsId " +
                                " left join MDiv mdiv on mdiv.CorpsId = mcor.CorpsId " +
                                " left join MapUnit mapunit on mapunit.CorpsId = mcor.CorpsId " +
                                " where mcor.CorpsId = @CorpsId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOCorpsIdCheckInFKTableResponse>(query, new { CorpsId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "CorpsDB->CorpsIdCheckInFKTable");
                return null;
            }
        }



        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}



    }
}