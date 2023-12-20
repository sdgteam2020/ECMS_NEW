using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class RegistrationDB : GenericRepositoryDL<MRegistration>, IRegistrationDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        public RegistrationDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
        }

        private readonly IConfiguration configuration;

        public async Task<List<MRegistration>> GetByApplyFor(MRegistration Data)
        {
            var ret =await _context.MRegistration.Where(x => x.ApplyForId == Data.ApplyForId).ToListAsync();
            return ret;
        }

        public async Task<DTOApplyCardDetailsResponse> GetApplyCardDetails(DTOApplyCardDetailsRequest Data)
        {
            try
            {
               
                string query = "select App.Name ApplyFor,reg.Name Registraion,(select Name from MICardType where TypeId=@TypeId) Type from MApplyFor App inner join" +
                                " MRegistration reg on App.ApplyForId=reg.ApplyForId" +
                                " and App.ApplyForId=@ApplyForId and reg.RegistrationId=@RegistrationId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOApplyCardDetailsResponse>(query, new { Data.ApplyForId, Data.RegistrationId, Data.TypeId });
                    int sno = 1;

                    return BasicDetailList.SingleOrDefault();

                }
            }
            catch (Exception ex) {
                return null;
            }
           
        }
    }
}