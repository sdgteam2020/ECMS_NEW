using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    /// <summary>
    /// Data Access Layer for ArmedType entity, providing database operations.
    /// and implements the IArmedDB interface.
    /// for basic CRUD operations.
    /// </summary>
    public class ArmedDB : GenericRepositoryDL<MArmedType>, IArmedDB
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly ILogger<ArmedDB> _logger;// For logging
        public ArmedDB(ApplicationDbContext context, ILogger<ArmedDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously checks if any armed type exists with the same ArmedName or Abbreviation but a different ArmedId.
        /// This ensures that there are no duplicates in either ArmedName or Abbreviation, while excluding the current armed type's own ArmedId.
        /// </summary>
        /// <param name="DTo">The MArmedType object containing the ArmedName, Abbreviation, and ArmedId to check.</param>
        /// <returns>
        /// Returns true if a record with the same ArmedName or Abbreviation but a different ArmedId exists, otherwise false.
        /// </returns>
        public async Task<bool> GetByName(MArmedType DTo)
        {
            // LINQ query using AnyAsync() to check if there is any record in the MArmedType table
            // where the ArmedName or Abbreviation matches the provided DTo values (case-insensitive),
            // and the ArmedId is different from the current armed type's ArmedId (i.e., excluding the current record).
            var ret = await _context.MArmedType
                                    .AnyAsync(x => (x.ArmedName.ToUpper() == DTo.ArmedName.ToUpper() ||
                                                     x.Abbreviation.ToUpper() == DTo.Abbreviation.ToUpper()) &&
                                                     x.ArmedId != DTo.ArmedId);

            // Return true if a matching armed type is found, otherwise false.
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
                              FlagInf = A.FlagInf,
                              Inf = A.FlagInf == true ? "Yes" : "No",
                              ArmedCatId = F.ArmedCatId,
                              Name = F.Name,
                          }).OrderByDescending(x => x.ArmedId).ToList();


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