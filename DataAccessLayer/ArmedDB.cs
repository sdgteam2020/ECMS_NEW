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
    public class ArmedDB : GenericRepositoryDL<MArmedType>, IArmedDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<ArmedDB> _logger;
        public ArmedDB(ApplicationDbContext context, ILogger<ArmedDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
      

      
        private readonly IConfiguration configuration;
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

       

         public async Task<bool> GetByName(MArmedType DTo)
         {
            var ret = await _context.MArmedType.AnyAsync(x=>(x.ArmedName.ToUpper() == DTo.ArmedName.ToUpper() || x.Abbreviation.ToUpper() == DTo.Abbreviation.ToUpper()) && x.ArmedId != DTo.ArmedId);
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
        public async Task<DTOArmedIdCheckInFKTableResponse?> ArmedIdCheckInFKTable(byte ArmedId)
        {
            try
            {
                string query = "Select count(distinct bd.BasicDetailId) as TotalBD, count(mrec.RecordOfficeId)as TotalRO from MArmedType marm" +
                                " left join BasicDetails bd on bd.ArmedId = marm.ArmedId " +
                                " left join MRecordOffice mrec on mrec.ArmedId = marm.ArmedId " +
                                " where marm.ArmedId=@ArmedId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOArmedIdCheckInFKTableResponse>(query, new { ArmedId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ArmedDB->ArmedIdCheckInFKTable");
                return null;
            }
        }
    }
}