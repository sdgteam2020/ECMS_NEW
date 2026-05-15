using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        /// <summary>
        /// Retrieves a list of MRegistration records based on the ApplyForId.
        /// </summary>
        /// <param name="Data">An instance of MRegistration containing the ApplyForId to filter the records.</param>
        /// <returns>A list of MRegistration records that match the ApplyForId.</returns>
        public async Task<List<MRegistration>> GetByApplyFor(MRegistration Data)
        {
            try
            {
                var ret = await _context.MRegistration
                            .Where(x => x.ApplyForId == Data.ApplyForId)
                            .Select(x => new MRegistration
                            {
                                RegistrationId = x.RegistrationId,
                                Name = x.Name,
                                Order = x.Order,
                                ApplyForId = x.ApplyForId
                            })
                            .AsNoTracking()
                            .ToListAsync();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RegistrationDB->GetByApplyFor");
                return new List<MRegistration>();
            }

        }


        /// <summary>
        /// Retrieves detailed apply card information for a specific user and registration.
        /// </summary>
        /// <param name="Data">An instance of DTOApplyCardDetailsRequest containing ApplyForId, RegistrationId, TypeId, and UserId to filter the query.</param>
        /// <returns>A DTOApplyCardDetailsResponse object containing the detailed card information, or a default instance if not found or in case of an error.</returns>
        public async Task<DTOApplyCardDetailsResponse> GetApplyCardDetails(DTOApplyCardDetailsRequest Data)
        {
            try
            {
                string query = "select App.Name ApplyFor,reg.Name Registraion,(select Name from MICardType where TypeId=@TypeId) Type,users.DomainId,unit.UnitName,unit.Suffix,unit.Sus_no,pro.Name,ranks.RankAbbreviation,pro.ArmyNo  from MApplyFor App inner join" +
                                " MRegistration reg on App.ApplyForId=reg.ApplyForId" +
                                " and App.ApplyForId=@ApplyForId and reg.RegistrationId=@RegistrationId" +
                                " inner join TrnDomainMapping trn on trn.AspNetUsersId = @UserId" +
                                " inner join AspNetUsers users on users.Id = trn.AspNetUsersId" +
                                " inner join MapUnit mapuni on mapuni.UnitMapId = trn.UnitId" +
                                " inner join MUnit unit on unit.UnitId = mapuni.UnitId" +
                                " left join UserProfile pro on pro.UserId = trn.UserId" +
                                " inner join MRank ranks on ranks.RankId = pro.RankId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOApplyCardDetailsResponse>(query, new { Data.ApplyForId, Data.RegistrationId, Data.TypeId, Data.UserId });

                    return BasicDetailList.FirstOrDefault() ?? new DTOApplyCardDetailsResponse();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RegistrationDB->GetApplyCardDetails");
                return new DTOApplyCardDetailsResponse();
            }

        }

        public async Task<List<MArmyPrefixRule>> GetArmyPrefixRules(DTOApplyForRequest Data)
        {
            try
            {
                var result = await _context.MArmyPrefixRule
                               .Where(x => x.ApplyForId == Data.ApplyForId && x.IsActive)
                               .OrderBy(x => x.Order)
                               .Select(x => new
                               {
                                   x.Id,
                                   x.Prefix,
                                   x.MinDigits,
                                   x.MaxDigits,
                                   x.StorePrefix
                               })
                               .AsNoTracking()
                               .ToListAsync();
                return result.Select(x => new MArmyPrefixRule
                {
                    Id=x.Id,
                    Prefix = x.Prefix,
                    MinDigits = x.MinDigits,
                    MaxDigits = x.MaxDigits,
                    StorePrefix = x.StorePrefix
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RegistrationDB->GetArmyPrefixRules");
                return new List<MArmyPrefixRule>();
            }
        }
    }
}