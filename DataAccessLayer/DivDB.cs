using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DivDB : GenericRepositoryDL<MDiv>, IDivDB
    {
        protected readonly ApplicationDbContext _context;
        public DivDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MDiv Data)
        {
            var ret = _context.MDiv.Select(p => p.DivName.ToUpper() == Data.DivName.ToUpper()).FirstOrDefault();
            return ret;
        }

        public Task<List<DTODivResponse>> GetALLDiv()
        {
            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div = (from div in _context.MDiv
                       join cor in _context.MCorps on div.CorpsId equals cor.CorpsId
                       join Com in _context.MComd on div.ComdId equals Com.ComdId
                       where div.DivId!=1
                       select new DTODivResponse
                       {
                           DivId = div.DivId,
                           DivName = div.DivName,   
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                       }
                     ).ToList(); ;

          


            return Task.FromResult(Div);
        }

        public async Task<List<DTODivResponse>> GetByHId(DTOMHierarchyRequest Data)
        {
            var Div = (from div in _context.MDiv
                         join cor in _context.MCorps on div.CorpsId equals cor.CorpsId
                         join d in _context.MComd
                         on cor.ComdId equals d.ComdId
                         where div.CorpsId == Data.CorpsId && div.ComdId==Data.ComdId && div.DivId!=1
                         select new DTODivResponse
                         {

                             DivId = div.DivId,
                             DivName = div.DivName,



                         }).ToList();


            return await Task.FromResult(Div);
        }
    }
}
