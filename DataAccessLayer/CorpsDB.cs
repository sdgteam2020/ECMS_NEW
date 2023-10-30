using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
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
        protected readonly ApplicationDbContext _context;
        public CorpsDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MCorps Data)
        {
            var ret = _context.MCorps.Where(p=> p.ComdId == Data.ComdId).Select(p => p.CorpsName.ToUpper() == Data.CorpsName.ToUpper()).FirstOrDefault();

          
            return ret;
        }

        public Task<List<DTOCorpsResponse>> GetALLCorps()
        {
            var Corps = (from c in _context.MCorps
                         join d in _context.MComd
                         on c.ComdId equals d.ComdId
                         where c.CorpsId!=1
                         select new DTOCorpsResponse
                         {
                             
                             CorpsId = c.CorpsId,
                             CorpsName = c.CorpsName,
                             ComdName = d.ComdName,
                             ComdId=d.ComdId,

                         }).ToList();
           


            return Task.FromResult(Corps);  
        }

        public async Task<List<DTOCorpsResponse>> GetByComdId(int ComdId)
        {
            var Corps = (from c in _context.MCorps
                         join d in _context.MComd
                         on c.ComdId equals d.ComdId where c.ComdId == ComdId   
                         select new DTOCorpsResponse
                         {

                             CorpsId = c.CorpsId,
                             CorpsName = c.CorpsName,
                            
                            

                         }).ToList();


            return await Task.FromResult(Corps);
        }



        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}



    }
}