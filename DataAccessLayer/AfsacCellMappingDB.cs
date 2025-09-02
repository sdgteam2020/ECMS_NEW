using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    /// <summary>
    /// Data Access Layer for AfsacCellMapping entity, providing database operations.
    /// And implements the IAfsacCellMappingDB interface.
    /// </summary>
    public class AfsacCellMappingDB : GenericRepositoryDL<AfsacCellMapping>, IAfsacCellMappingDB
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly ILogger<AfsacCellMappingDB> _logger;// For logging

        /// <summary>
        /// Constructor to initialize the AfsacCellMappingDB with necessary contexts and logger.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextDP"></param>
        /// <param name="logger"></param>
        public AfsacCellMappingDB(ApplicationDbContext context, DapperContext contextDP, ILogger<AfsacCellMappingDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }


        /// <summary>
        /// Asynchronously checks if any record in the AfsacCellMapping table exists with a different AfsacCellMappingId 
        /// than the one provided in the Dto parameter.
        /// </summary>
        /// <param name="Dto">The Data Transfer Object containing the AfsacCellMappingId to be checked against.</param>
        /// <returns>Returns true if a record exists with a different AfsacCellMappingId, otherwise false.</returns>
        public async Task<bool> GetByName(AfsacCellMapping Dto)
        {
            // LINQ query using AnyAsync() to check if there are any records in the AfsacCellMapping table 
            // where the AfsacCellMappingId is not equal to the one in the provided Dto.
            // This query returns a boolean indicating whether any such record exists.
            var ret = await _context.AfsacCellMapping
                                    .AnyAsync(x => x.AfsacCellMappingId != Dto.AfsacCellMappingId);

            // Return the result of the query.
            return ret;
        }

        public async Task<List<DTOAfsacCellMappingResponse>?> GetAllAfsacCellMapping()
        {
            try
            {
                string query = "";
                query = "Select acmap.AfsacCellMappingId,acmap.TDMId,acmap.UnitId,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName from AfsacCellMapping acmap" +
                        " left join TrnDomainMapping trndomain on trndomain.Id=acmap.TDMId" +
                        " left join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " left join MRank ra on ra.RankId=usep.RankId " +
                        " left join MapUnit mapunit on mapunit.UnitMapId = acmap.UnitId " +
                        " left join MUnit munit on munit.UnitId =mapunit.UnitId order by acmap.AfsacCellMappingId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOAfsacCellMappingResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AfsacCellMappingDB->GetAllAfsacCellMapping");
                return null;
            }

        }
    }
}