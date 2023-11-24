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

namespace DataAccessLayer
{
    public class BdeDB : GenericRepositoryDL<MBde>, IBdeDB
    {
        protected readonly ApplicationDbContext _context;
       
        public BdeDB(ApplicationDbContext context ) : base(context)
        {
            _context = context;
          
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MBde Data)
        {
            var ret = _context.MBde.Any(p => p.BdeName.ToUpper() == Data.BdeName.ToUpper());
            return ret;
        }
      

        public Task<List<DTOBdeResponse>> GetALLBdeCat()
        {
            var Corps = (from bde in _context.MBde
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

                         }).ToList();


            return Task.FromResult(Corps);  
        }

        public Task<List<DTOBdeResponse>> GetByHId(DTOMHierarchyRequest Data)
        {
            var Bde = (from bde in _context.MBde
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
                           

                         }).ToList();


            return Task.FromResult(Bde);
        }



        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}



    }
}