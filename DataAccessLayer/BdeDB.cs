using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using DataAccessLayer.Logger;
using Microsoft.Extensions.Logging;
using Dapper;

namespace DataAccessLayer
{
    public class BdeDB : GenericRepositoryDL<MBde>, IBdeDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<BdeDB> _logger;

        public BdeDB(ApplicationDbContext context, DapperContext contextDP, ILogger<BdeDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        private readonly IConfiguration configuration;

        public async Task<bool?> GetByName(MBde Data)
        {
            try
            {
                List<MBde> mBdes = await _context.MBde.ToListAsync();
                
                var ret = mBdes.Any(p => p.BdeName.ToUpper() == Data.BdeName.ToUpper() && p.BdeId != Data.BdeId);
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BdeDB->GetByName");
                return null;
            }
        }
        public async Task<bool?> FindByBdeWithId(string BdeName, byte BdeId)
        {
            try
            {
                //var ret = await _context.MBde.AnyAsync(p => p.BdeId != BdeId && p.BdeName.ToUpper() == BdeName.ToUpper());
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BdeDB->FindByBdeWithId");
                return null;
            }

        }


        public async Task<List<DTOBdeResponse>> GetALLBdeCat()
        {
            var Corps = await (from bde in _context.MBde
                         join div in _context.MDiv
                         on bde.DivId equals div.DivId
                        
                         join cor in _context.MCorps
                         on bde.CorpsId equals cor.CorpsId
                         join Com in _context.MComd
                         on bde.ComdId equals Com.ComdId
                         
                         where  bde.BdeId!=1
                         select new DTOBdeResponse
                         {
                             BdeId=bde.BdeId,
                             BdeName=bde.BdeName,
                             DivId=div.DivId,
                             DivName=div.DivName,
                             CorpsId = cor.CorpsId,
                             CorpsName = cor.CorpsName,
                             ComdName = Com.ComdName,
                             ComdId= Com.ComdId,

                         }).ToListAsync();


            return Corps;  
        }

        public async Task<List<DTOBdeResponse>> GetByHId(DTOMHierarchyRequest Data)
        {
            var Bde = await (from bde in _context.MBde
                         join div in _context.MDiv
                         on bde.DivId equals div.DivId

                         join cor in _context.MCorps
                        on bde.CorpsId equals cor.CorpsId
                         join Com in _context.MComd
                         on bde.ComdId equals Com.ComdId

                         where bde.ComdId==Data.ComdId && bde.CorpsId==Data.CorpsId && bde.DivId==Data.DivId &&  bde.BdeId != 1
                         select new DTOBdeResponse
                         {
                             BdeId = bde.BdeId,
                             BdeName = bde.BdeName,
                           

                         }).ToListAsync();


            return Bde;
        }



        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}



    }
}