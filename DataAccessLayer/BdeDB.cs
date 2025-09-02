using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Checks if a Brigade (BDE) with the specified name already exists in the database, excluding the current record being edited.
        /// This method queries the `MBde` table and compares the `BdeName` to the provided name, ensuring that the `BdeId` is not the same as the one provided for the current record.
        /// </summary>
        /// <param name="Data">The `MBde` object containing the Brigade name (`BdeName`) and `BdeId` to check for duplication in the database.</param>
        /// <returns>
        /// A boolean indicating whether a Brigade with the same name exists in the database but is not the same as the current one. 
        /// Returns `null` in case of an exception.
        /// </returns>
        /// <remarks>
        /// The method performs a case-insensitive comparison on the `BdeName` and ensures the `BdeId` is not the same as the provided `BdeId` to prevent checking the record being edited.
        /// </remarks>
        public async Task<bool?> GetByName(MBde Data)
        {
            try
            {
                // Fetch all existing Brigade records from the database without tracking changes.
                List<MBde> mBdes = await _context.MBde.AsNoTracking().ToListAsync();

                // Check if there is any Brigade with the same name, excluding the current Brigade by comparing the BdeId
                var ret = mBdes.Any(p => p.BdeName.ToUpper() == Data.BdeName.ToUpper() && p.BdeId != Data.BdeId);

                // Return true if a matching Brigade name exists, otherwise false
                return ret;
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the database query
                _logger.LogError(1001, ex, "BdeDB->GetByName");

                // Return null in case of an error, indicating that the check could not be performed
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

        /// <summary>
        /// Retrieves a list of all Brigade (BDE) categories from the database, including associated Division, Corps, and Command data.
        /// This method performs a join between the `MBde`, `MDiv`, `MCorps`, and `MComd` tables to fetch related data for each Brigade.
        /// The method filters out the first record (BdeId = 1) and returns the result as a list of <see cref="DTOBdeResponse"/> objects.
        /// </summary>
        /// <returns>
        /// A list of <see cref="DTOBdeResponse"/> objects containing Brigade (BDE) data along with associated Division, Corps, and Command information.
        /// </returns>
        /// <remarks>
        /// This method uses LINQ to perform the join operation between `MBde`, `MDiv`, `MCorps`, and `MComd` tables. The filtered result excludes any Brigade with a `BdeId` of 1.
        /// </remarks>
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

        /// <summary>
        /// Retrieves a list of Brigade (BDE) information based on the provided hierarchy data (ComdId, CorpsId, DivId).
        /// This method filters the Brigades by the given command (`ComdId`), corps (`CorpsId`), and division (`DivId`), excluding the Brigade with ID 1.
        /// </summary>
        /// <param name="Data">The request object containing the hierarchy data: `ComdId`, `CorpsId`, and `DivId`.</param>
        /// <returns>
        /// A list of `DTOBdeResponse` objects representing the Brigades (BDEs) that match the specified hierarchy. 
        /// Each object contains the Brigade ID and its name.
        /// </returns>
        /// <remarks>
        /// This method performs a join between the `MBde`, `MDiv`, `MCorps`, and `MComd` tables to retrieve the Brigade data.
        /// It filters based on the provided `ComdId`, `CorpsId`, and `DivId`, ensuring that the Brigade with `BdeId = 1` is excluded from the results.
        /// </remarks>
        public async Task<List<DTOBdeResponse>> GetByHId(DTOMHierarchyRequest Data)
        {
            // Perform a LINQ query to get Brigades matching the provided hierarchy details
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

        /// <summary>
        /// Checks if the Brigade (BDE) with the given `BdeId` exists in the `MapUnit` table. 
        /// This method returns the count of distinct `UnitMapId` related to the provided `BdeId`.
        /// </summary>
        /// <param name="BdeId">The `BdeId` of the Brigade to check in the `MapUnit` table.</param>
        /// <returns>
        /// A `DTOBdeIdCheckInFKTableResponse` object containing the total count of `UnitMapId` associated with the `BdeId` 
        /// or `null` if an exception occurs during the process.
        /// </returns>
        /// <remarks>
        /// This method executes a SQL query that counts how many unique `UnitMapId`s are related to the provided `BdeId` in the `MapUnit` table.
        /// It returns the result as part of the `DTOBdeIdCheckInFKTableResponse` object.
        /// </remarks>
        public async Task<DTOBdeIdCheckInFKTableResponse?> BdeIdCheckInFKTable(byte BdeId)
        {
            try
            {
                // SQL query to check how many units are related to the given BdeId in MapUnit
                string query = @"Select  count(distinct mapunit.UnitMapId) as TotalMapUnit from MBde mbd
                                left join MapUnit mapunit on mapunit.BdeId = mbd.BdeId
                                where mbd.BdeId = @BdeId";

                // Using a database connection to execute the query and fetch the result
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query asynchronously and map the result to DTOBdeIdCheckInFKTableResponse
                    var ret = await connection.QueryAsync<DTOBdeIdCheckInFKTableResponse>(query, new { BdeId });

                    // Return the first record from the result (or null if no records are found)
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the execution of the method
                _logger.LogError(1001, ex, "BdeDB->BdeIdCheckInFKTable");

                // Return null in case of an error
                return null;
            }
        }
    }
}