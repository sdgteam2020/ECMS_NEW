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
    public class RegimentalDB : GenericRepositoryDL<MRegimental>, IRegimentalDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<RegimentalDB> _logger;
        public RegimentalDB(ApplicationDbContext context, DapperContext contextDP, ILogger<RegimentalDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
      
        public async Task<bool> GetByName(MRegimental Dto)
        {
            List<MRegimental> mRegimentals = await _context.MRegimental.AsNoTracking().ToListAsync();
            var ret = mRegimentals.Any(x => (x.Name.ToUpper() == Dto.Name.ToUpper() || x.Abbreviation.ToUpper() == Dto.Abbreviation.ToUpper()) && x.RegId != Dto.RegId);
            return ret;
        }

        public async Task<List<DTORegimentalResponse>> GetByArmedId(byte ArmedId)
        {
            var data = await (from a in _context.MArmedType.AsNoTracking()
                               join r in _context.MRegimental.AsNoTracking()
                               on a.ArmedId equals r.ArmedId
                               where r.ArmedId == ArmedId
                               select new DTORegimentalResponse
                               {
                                   RegId = r.RegId,
                                   Name = r.Name,
                               }).ToListAsync();
            return data;
        }

        public async Task<List<DTORegimentalResponse>> GetAllData()
        {
            try
            {
                string query = "";
                query = @"Select mreg.RegId,mreg.Name,mreg.Location,mreg.Abbreviation,mreg.UnitId,marmed.ArmedId,marmed.ArmedName, munit.Sus_no,munit.Suffix,munit.Abbreviation AS UnitAbbreviation,munit.UnitName
                        from MRegimental mreg
                        inner join MArmedType marmed on marmed.ArmedId=mreg.ArmedId
                        left join MapUnit mapunit on mapunit.UnitMapId = mreg.UnitId
                        left join MUnit munit on munit.UnitId =mapunit.UnitId order by mreg.RegId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTORegimentalResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RegimentalDB->GetAllData");
                return new List<DTORegimentalResponse>() ;
            }
        }
    }
}