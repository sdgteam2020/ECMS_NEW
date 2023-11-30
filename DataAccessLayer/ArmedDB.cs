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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class ArmedDB : GenericRepositoryDL<MArmedType>, IArmedDB
    {
        protected readonly ApplicationDbContext _context;
        public ArmedDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
      

      
        private readonly IConfiguration configuration;
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

       

         public async Task<bool> GetByName(MArmedType DTo)
         {
            // && p.ComdId != DTo.ComdId && p.IsDeleted==true
            var ret = _context.MArmedType.Where(P=>P.ArmedId!=DTo.ArmedId).Select(p => p.ArmedName.ToUpper() == DTo.ArmedName.ToUpper()).FirstOrDefault();
            return ret;
        }
        public Task<List<DTOArmedResponse>> GetALLArmed()
        {
            var GetALL = (from A in _context.MArmedType
                          join F in _context.MArmedCats
                          on A.ArmedCatId equals F.ArmedCatId

                          select new DTOArmedResponse
                          {
                              ArmedId = A.ArmedId,
                              ArmedName = A.ArmedName,
                              Abbreviation = A.Abbreviation,
                              FlagInf=A.FlagInf,
                              Inf= A.FlagInf==true?"Yes":"No",
                              ArmedCatId = F.ArmedCatId,
                              Name = F.Name,
                          }).ToList();


            return Task.FromResult(GetALL);
        }
    }
}