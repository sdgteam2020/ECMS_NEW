using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class MapUnitDB : GenericRepositoryDL<MapUnit>, IMapUnitDB
    {
        protected readonly ApplicationDbContext _context;
        public MapUnitDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MapUnit Data)
        {
            var ret = _context.MapUnit.Any(p => p.UnitId == Data.UnitId);
            return ret;
        }

        public Task<List<DTOMapUnitResponse>> GetALLUnit(DTOMHierarchyRequest unit, string Unit1)
        {
            Unit1 = string.IsNullOrEmpty(Unit1) ? "" : Unit1.ToLower();
            if (unit.ComdId == null)unit.ComdId = 0;
            if (unit.CorpsId == null)unit.CorpsId = 0;
            if (unit.DivId == null)unit.DivId = 0;
            if (unit.BdeId == null)unit.BdeId = 0;

            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div = (from uni in _context.MapUnit
            //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
            //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
            //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
            //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
            join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd
                       on uni.ComdId equals Com.ComdId
                    //   on new { uni.ComdId } equals new { Com.ComdId }
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                      where MUni.UnitName == "" || MUni.UnitName.ToLower().Contains(Unit1)
                       select new DTOMapUnitResponse
                       {
                           UnitMapId= uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           UnitId = uni.UnitId,
                           BdeId = bde.BdeId,  
                           BdeName = bde.BdeName,   
                           DivId = div.DivId,
                           DivName = div.DivName,
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                           Suffix=MUni.Suffix,
                           Sus_no=MUni.Sus_no,
                           FmnBrach= uni.FmnBrach
                           
                       }
                     ).Distinct().Take(200).ToList(); ;




            return Task.FromResult(Div);
        }

        public Task<List<DTOMapUnitResponse>> GetALLByUnitName(string UnitName)
        {

           
            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div = (from uni in _context.MapUnit
                           //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
                           //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
                           //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
                           //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd on uni.ComdId equals Com.ComdId
                       //   on new { uni.ComdId } equals new { Com.ComdId }
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                       where MUni.UnitName.Contains(UnitName)
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           //UnitId = uni.UnitId,
                           //BdeId = bde.BdeId,
                           //BdeName = bde.BdeName,
                           //DivId = div.DivId,
                           //DivName = div.DivName,
                           //CorpsId = cor.CorpsId,
                           //CorpsName = cor.CorpsName,
                           //ComdName = Com.ComdName,
                           //ComdId = Com.ComdId,
                           //Suffix = MUni.Suffix,
                           //Sus_no = MUni.Sus_no
                       }
                     ).Distinct().ToList(); ;




            return Task.FromResult(Div);
        }

        public Task<DTOMapUnitResponse> GetALLByUnitMapId(int UnitMapId)
        {


            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div = (from uni in _context.MapUnit
                           //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
                           //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
                           //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
                           //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd
                       on uni.ComdId equals Com.ComdId
                       //   on new { uni.ComdId } equals new { Com.ComdId }
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                       where uni.UnitMapId==UnitMapId
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           UnitId = uni.UnitId,
                           BdeId = bde.BdeId,
                           BdeName = bde.BdeName,
                           DivId = div.DivId,
                           DivName = div.DivName,
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                           Suffix = MUni.Suffix,
                           Sus_no = MUni.Sus_no
                       }
                     ).Distinct().SingleOrDefault() ;




            return Task.FromResult(Div);
        }
        public async Task<DTOMapUnitResponse> GetALLByUnitById(int UnitId)
        {


            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div =await (from uni in _context.MapUnit
                           //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
                           //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
                           //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
                           //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd
                       on uni.ComdId equals Com.ComdId
                       //   on new { uni.ComdId } equals new { Com.ComdId }
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                       where MUni.UnitId == UnitId
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           UnitId = uni.UnitId,
                           BdeId = bde.BdeId,
                           BdeName = bde.BdeName,
                           DivId = div.DivId,
                           DivName = div.DivName,
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                           Suffix = MUni.Suffix,
                           Sus_no = MUni.Sus_no
                       }
                     ).Distinct().SingleOrDefaultAsync();





            return (Div);
        }
    }
 }
