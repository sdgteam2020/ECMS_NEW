using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
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
    public class RegimentalDB : GenericRepositoryDL<MRegimental>, IRegimentalDB
    {
        protected readonly ApplicationDbContext _context;
        public RegimentalDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
      

      
        private readonly IConfiguration configuration;
       
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

        public Task<List<DTORegimentalResponse>> GetAllData()
        {
            var Corps = (from c in _context.MRegimental
                         join d in _context.MArmedType
                         on c.ArmedId equals d.ArmedId
                        
                         select new DTORegimentalResponse
                         {
                             RegId=c.RegId,
                             Name=c.Name,
                             Abbreviation=c.Abbreviation,
                             ArmedId=c.RegId, //c.ArmedId
                             ArmedName =d.ArmedName,
                             Location=c.Location,

                         }).OrderByDescending(x=>x.RegId).ToList();



            return Task.FromResult(Corps);
        }
    }
}