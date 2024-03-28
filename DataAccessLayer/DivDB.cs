using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
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
        protected new readonly ApplicationDbContext _context;
        public DivDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MDiv Data)
        {
            var ret = await _context.MDiv.AnyAsync(p => p.DivName.ToUpper() == Data.DivName.ToUpper() && p.DivId != Data.DivId);
            return ret;
        }

        public async Task<List<DTODivResponse>> GetALLDiv()
        {
            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div = await (from div in _context.MDiv
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
                             ).ToListAsync(); ;
            return Div;
        }

        public async Task<List<DTODivResponse>> GetByHId(DTOMHierarchyRequest Data)
        {
            var Div = await (from div in _context.MDiv
                             join cor in _context.MCorps on div.CorpsId equals cor.CorpsId
                             join d in _context.MComd
                             on cor.ComdId equals d.ComdId
                             where div.CorpsId == Data.CorpsId && div.ComdId==Data.ComdId && div.DivId!=1
                             select new DTODivResponse
                             {
                                 DivId = div.DivId,
                                 DivName = div.DivName,
                             }).ToListAsync();
            return Div;
        }
    }
}
