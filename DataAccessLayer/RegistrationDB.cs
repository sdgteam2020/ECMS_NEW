using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class RegistrationDB : GenericRepositoryDL<MRegistration>, IRegistrationDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<RegistrationDB> _logger;
        public RegistrationDB(ApplicationDbContext context, DapperContext contextDP, ILogger<RegistrationDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        public async Task<List<MRegistration>> GetByApplyFor(MRegistration Data)
        {
            try
            {
                var ret = await _context.MRegistration.Where(x => x.ApplyForId == Data.ApplyForId).ToListAsync();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RegistrationDB->GetByApplyFor");
                return new List<MRegistration>();
            }

        }

        public async Task<DTOApplyCardDetailsResponse> GetApplyCardDetails(DTOApplyCardDetailsRequest Data)
        {
            try
            {
                string query = "select App.Name ApplyFor,reg.Name Registraion,(select Name from MICardType where TypeId=@TypeId) Type,users.DomainId,unit.UnitName,unit.Suffix,unit.Sus_no,pro.Name,ranks.RankAbbreviation,pro.ArmyNo  from MApplyFor App inner join" +
                                " MRegistration reg on App.ApplyForId=reg.ApplyForId" +
                                " and App.ApplyForId=@ApplyForId and reg.RegistrationId=@RegistrationId"+
                                " inner join TrnDomainMapping trn on trn.AspNetUsersId = @UserId" +
                                " inner join AspNetUsers users on users.Id = trn.AspNetUsersId"+
                                " inner join MapUnit mapuni on mapuni.UnitMapId = trn.UnitId"+
                                " inner join MUnit unit on unit.UnitId = mapuni.UnitId"+
                                " left join UserProfile pro on pro.UserId = trn.UserId" +
                                " inner join MRank ranks on ranks.RankId = pro.RankId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOApplyCardDetailsResponse>(query, new { Data.ApplyForId, Data.RegistrationId, Data.TypeId,Data.UserId });

                    return BasicDetailList.FirstOrDefault() ?? new DTOApplyCardDetailsResponse();

                }
            }
            catch (Exception ex) {
                _logger.LogError(1001, ex, "RegistrationDB->GetApplyCardDetails");
                return new DTOApplyCardDetailsResponse();
            }
           
        }
    }
}