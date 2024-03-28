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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class CorpsDB : GenericRepositoryDL<MCorps>, ICorpsDB
    {
        protected new readonly ApplicationDbContext _context;
        public CorpsDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
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



        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}



    }
}