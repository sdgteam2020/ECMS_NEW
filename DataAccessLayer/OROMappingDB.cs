using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class OROMappingDB : GenericRepositoryDL<OROMapping>, IOROMappingDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<OROMappingDB> _logger;
        public OROMappingDB(ApplicationDbContext context, DapperContext contextDP, ILogger<OROMappingDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        /// <summary>
        /// Gets a boolean value indicating whether there is any existing record in the OROMapping table
        /// that has a different OROMappingId than the one specified in the DTO.
        /// </summary>
        /// <param name="Dto">The DTO object containing the OROMappingId to compare.</param>
        /// <returns>A boolean value indicating whether a record exists with a different OROMappingId.</returns>
        public async Task<bool> GetByName(OROMapping Dto)
        {
            var ret =  await _context.OROMapping.AnyAsync(x => x.OROMappingId != Dto.OROMappingId);
            return ret;
        }

        /// <summary>
        /// Retrieves all OROMapping records along with related details like Rank, Record Office, and Unit information.
        /// </summary>
        /// <returns>A list of DTOOROMappingResponse objects containing OROMapping and related data.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the database query.</exception>
        public async Task<List<DTOOROMappingResponse>?> GetAllOROMapping()
        {
            try
            {
                string query = "";
                query = "Select oromap.OROMappingId,oromap.ArmedIdList,oromap.RankId,mrak.RankName,mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,oromap.TDMId,oromap.UnitId,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName" +
                        " ,(select STRING_AGG(ArmedName,'#') from MArmedType where ArmedId in (select value from string_split(oromap.ArmedIdList,','))) ArmNameList from OROMapping oromap" +
                        " inner join MRecordOffice mrecord on mrecord.RecordOfficeId=oromap.RecordOfficeId" +
                        " left join MRank mrak on mrak.RankId=oromap.RankId" +
                        " left join TrnDomainMapping trndomain on trndomain.Id=oromap.TDMId" +
                        " left join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " left join MRank ra on ra.RankId=usep.RankId " +
                        " left join MapUnit mapunit on mapunit.UnitMapId = oromap.UnitId " +
                        " left join MUnit munit on munit.UnitId =mapunit.UnitId order by oromap.OROMappingId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOOROMappingResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "OROMappingDB->GetAllOROMapping");
                return null;
            }

        }

        /// <summary>
        /// Retrieves a list of all OROMapping records along with their associated Record Office names.
        /// </summary>
        /// <returns>A list of DTOAllOROResponse objects containing OROMappingId and RecordOffice details.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the database query.</exception>
        public async Task<List<DTOAllOROResponse>> GetAllORO()
        {
            try
            {
                string query = "";
                query = @"SELECT oro.OROMappingId,rec.RecordOfficeId,rec.Name FROM OROMapping oro
                        INNER JOIN MRecordOffice rec on oro.RecordOfficeId=rec.RecordOfficeId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOAllOROResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "OROMappingDB->GetAllORO");
                return new List<DTOAllOROResponse>();
            }

        }
    }
}
