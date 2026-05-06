using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using static Dapper.SqlMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class MapUnitDB : GenericRepositoryDL<MapUnit>, IMapUnitDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<MapUnitDB> _logger;
        public MapUnitDB(ApplicationDbContext context, DapperContext contextDP, ILogger<MapUnitDB> logger) : base(context)
        {
            _logger = logger;
            _contextDP = contextDP;
            _context = context;
        }
        
        
        
        /// <summary>
        /// Checks whether a unit is already mapped in the <c>MapUnit</c> table based on the given SUS number.
        /// Performs a LEFT JOIN between <c>MUnit</c> and <c>MapUnit</c> and returns the mapping status.
        /// </summary>
        /// <param name="SUSNo">The concatenated SUS number and suffix (e.g., "12345A") to check for mapping.</param>
        /// <returns>
        /// An instance of <see cref="DTOCheckUnitMappedInMapUnitResponse"/> containing:
        /// - <c>UnitId</c>: The ID of the unit in <c>MUnit</c>.
        /// - <c>IsVerify</c>: Indicates whether the unit has been verified.
        /// - <c>UnitMapId</c>: The ID of the mapping in <c>MapUnit</c>, or null if not mapped.
        /// Returns null if no matching unit is found.
        /// </returns>
        /// <remarks>
        /// - Uses Dapper to execute a SQL query asynchronously.
        /// - Query concatenates <c>Sus_no</c> and uppercased <c>Suffix</c> to match the input <paramref name="SUSNo"/>.
        /// - Intended for use before saving a unit mapping to prevent duplicates.
        /// </remarks>
        public async Task<DTOGenericResponse<DTOCheckUnitMappedInMapUnitResponse>> CheckUnitMappedInMapUnit(DTOSaveUnitWithMappingRequest dTO)
        {
            string SUSNO_WithoutSuffix = dTO.Sus_no.Trim();
            string SUSNo = dTO.Sus_no + dTO.Suffix.ToUpper();
            var response = new DTOGenericResponse<DTOCheckUnitMappedInMapUnitResponse>();
            var normalized = SUSNo.Trim();
            var Prefix = normalized[..Math.Min(3, normalized.Length)];

            string query = @"Select MUnit.UnitId,MUnit.Sus_no,MUnit.Suffix,MUnit.Prefix,MUnit.IsVerify,MapUnit.UnitMapId from MUnit
                            LEFT JOIN MapUnit on MUnit.UnitId = MapUnit.UnitId
                            where MUnit.Prefix =@Prefix";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var Unit = await connection.QueryAsync<DTOCheckUnitMappedInMapUnitResponse>(query, new { Prefix });
                    var result =  Unit.FirstOrDefault(x =>x.Sus_no.Equals(SUSNO_WithoutSuffix));
                    if(result != null && result.Suffix == dTO.Suffix)
                    {
                        if (result.UnitMapId == null)
                        {
                            response.Result = true;
                            response.Message = "ok";
                        }
                        else
                        {
                            if (result.IsVerify == false)
                            {
                                response.Result = false;
                                response.Message = "Unit not verified by Admin.";
                            }
                            else
                            {
                                response.Result = false;
                                response.Message = "Unit already mapped!";
                            }
                        }
                        response.Value = result;
                    }
                    else
                    {
                        // Retrieve all units from the database without tracking them in memory (for performance reasons).
                        List<MUnit> mUnits = await _context.MUnit.AsNoTracking().ToListAsync();
                        bool exists = mUnits.Any(x =>x.UnitName.Equals(dTO.UnitName, StringComparison.OrdinalIgnoreCase) || x.Abbreviation.Equals(dTO.Abbreviation, StringComparison.OrdinalIgnoreCase));
                        if (exists)
                        {
                            response.Result = false;
                            response.Message = "Unit Name or Abbreviation already exists.";
                            response.Value = new DTOCheckUnitMappedInMapUnitResponse();
                        }
                        else
                        {
                            var duplicateSUSNo = mUnits.FirstOrDefault(x => x.Sus_no.Equals(SUSNO_WithoutSuffix));

                            if (duplicateSUSNo != null)
                            {
                                response.Result = false;
                                response.Message = "SUS No already exists.";
                                response.Value = new DTOCheckUnitMappedInMapUnitResponse();
                            }
                            else
                            {
                                response.Result = true;
                                response.Message = "ok";
                                response.Value = new DTOCheckUnitMappedInMapUnitResponse();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->CheckUnitMappedInMapUnit");
                response.Result = false;
                response.Message = "An error occurred while checking the unit mapping.";
                response.Value = new DTOCheckUnitMappedInMapUnitResponse();
            }
            return response;
        }

        
        /// <summary>
        /// Checks if a unit with the given UnitId exists, excluding the current UnitMapId.
        /// </summary>
        /// <param name="Data">The MapUnit object containing the UnitId and UnitMapId to check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if a matching unit exists, otherwise false.</returns>
        public async Task<bool> GetByName(MapUnit Data)
        {
            var ret = await _context.MapUnit.AnyAsync(p => p.UnitId == Data.UnitId && p.UnitMapId!=Data.UnitMapId);
            return ret;
        }

        
        /// <summary>
        /// Finds whether a unit with the given UnitId exists in the MapUnit table.
        /// </summary>
        /// <param name="UnitId">The UnitId to check for existence.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a nullable boolean indicating the existence of the unit (null in case of an error).</returns>
        public async Task<bool?> FindUnitId(int UnitId)
        {
            try
            {
                var ret = await _context.MapUnit.AnyAsync(p => p.UnitId == UnitId);
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->FindUnitId");
                return null;
            }

        }

        
        /// <summary>
        /// Finds whether a unit with the given UnitId and UnitMapId exists in the MapUnit table, excluding the current mapping.
        /// </summary>
        /// <param name="UnitId">The UnitId to check for existence.</param>
        /// <param name="UnitMapId">The UnitMapId to exclude in the search.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a nullable boolean indicating the existence of the unit (null in case of an error).</returns>
        public async Task<bool?> FindUnitIdMapped(int UnitId,int UnitMapId)
        {
            try
            {
                var ret = await _context.MapUnit.AnyAsync(p => p.UnitId == UnitId && p.UnitMapId != UnitMapId);
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->FindUnitIdMapped");
                return null;
            }
        }

        
        /// <summary>
        /// Retrieves a paginated list of units with their corresponding hierarchy details.
        /// </summary>
        /// <param name="request">The DTO request containing pagination, sorting, and filtering information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response with the paginated data.</returns>
        public async Task<DTODataTablesResponse<DTOMapUnitResponse>> GetALLUnit(DTODataTablesRequestForMapUnit request)
        {
            try
            {
                var queryableData = (from uni in _context.MapUnit.OrderByDescending(x => x.UnitMapId)
                                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                                       join Com in _context.MComd on uni.ComdId equals Com.ComdId
                                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                                       join div in _context.MDiv on uni.DivId equals div.DivId
                                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                                       join pso in _context.MPso on uni.PsoId equals pso.PsoId
                                       join FmnBranch in _context.MFmnBranches on uni.FmnBranchID equals FmnBranch.FmnBranchID
                                       join SubDte in _context.MSubDte on uni.SubDteId equals SubDte.SubDteId
                                       select new DTOMapUnitResponse
                                       {
                                           UnitMapId = uni.UnitMapId,
                                           UnitName = MUni.UnitName,
                                           IsVerify = MUni.IsVerify,
                                           UnitId = uni.UnitId,
                                           BdeId = bde.BdeId,
                                           BdeName = bde.BdeName,
                                           DivId = div.DivId,
                                           DivName = div.DivName,
                                           CorpsId = cor.CorpsId,
                                           CorpsName = cor.CorpsName,
                                           ComdName = Com.ComdName,
                                           ComdId = Com.ComdId,
                                           Suffix = MUni.Suffix,
                                           Sus_no = MUni.Sus_no,
                                           Prefix = MUni.Prefix,
                                           UnitType = uni.UnitType,
                                           PsoId = pso.PsoId,
                                           PSOName = pso.PSOName,
                                           FmnBranchID = FmnBranch.FmnBranchID,
                                           BranchName = FmnBranch.BranchName,
                                           SubDteId = SubDte.SubDteId,
                                           SubDteName = SubDte.SubDteName,
                                       }
                                     ).AsQueryable();

                // Total records without filtering
                var totalRecords = queryableData.Count();


                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    queryableData = queryableData.Where(x => x.Prefix.ToLower().Contains(searchValue));
                }

                // Apply sorting

                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    //queryableData = queryableData.OrderBy(request.SortColumn + " " + request.SortColumnDirection);
                    queryableData = request.sortDirection.ToLower() == "asc"
                    ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                    : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));

                }

                // Total records after filtering
                var filteredRecords = queryableData.Count();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                var responseData = new DTODataTablesResponse<DTOMapUnitResponse>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords, // Total records without filtering
                    recordsFiltered = filteredRecords, // Total records after filtering
                    data = paginatedData
                };
                return responseData;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->GetALLUnit_");
                List<DTOMapUnitResponse> dTOUserRegnResponses = new List<DTOMapUnitResponse>();
                var responseData = new DTODataTablesResponse<DTOMapUnitResponse>
                {
                     draw = request.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }

        }

        
        /// <summary>
        /// Retrieves a list of map unit responses based on the provided unit name.
        /// </summary>
        /// <param name="UnitName">The name of the unit to search for. The search is case-insensitive.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of <see cref="DTOMapUnitResponse"/> objects.</returns>
        public async Task<List<DTOMapUnitResponse>> GetALLByUnitName(string UnitName)
        {
            if (string.IsNullOrWhiteSpace(UnitName))
                return new List<DTOMapUnitResponse>();

            var normalized = UnitName.Trim();
            var prefix = normalized[..Math.Min(3, normalized.Length)];
            try
            {
                var Unit = await (from uni in _context.MapUnit
                           join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                           where MUni.Prefix == prefix && MUni.IsVerify == true
                           select new DTOMapUnitResponse
                           {
                               UnitMapId = uni.UnitMapId,
                               UnitName = MUni.UnitName,
                               Suffix = MUni.Suffix,
                               Sus_no = MUni.Sus_no,
                           }).AsNoTracking().ToListAsync();
                return Unit.Where(x => (x.Sus_no + x.Suffix).Contains(normalized)).Take(10).ToList();
            }
            catch (Exception ex)
            {
                // Log the error in case of an exception
                _logger.LogError(1001, ex, "MapUnitDB->GetALLByUnitName");
                // Return null in case of an error
                return new List<DTOMapUnitResponse>();
            }

        }

        public async Task<List<DTOMapUnitResponse>> GetALLByUnitNameForBD(string UnitName,int UnitId,bool SameUnit)
        {
            if (string.IsNullOrWhiteSpace(UnitName))
                return new List<DTOMapUnitResponse>();

            var normalized = UnitName.Trim();
            var prefix = normalized[..Math.Min(3, normalized.Length)];
            try
            {
                var Unit = await (from uni in _context.MapUnit
                                  join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                                  where MUni.Prefix == prefix && MUni.IsVerify == true && (SameUnit || uni.UnitMapId != UnitId)
                                  select new DTOMapUnitResponse
                                  {
                                      UnitMapId = uni.UnitMapId,
                                      UnitName = MUni.UnitName,
                                      Suffix = MUni.Suffix,
                                      Sus_no = MUni.Sus_no,
                                  }).AsNoTracking().ToListAsync();
                return Unit.Where(x => (x.Sus_no + x.Suffix).Contains(normalized)).Take(10).ToList();

            }
            catch (Exception ex)
            {
                // Log the error in case of an exception
                _logger.LogError(1001, ex, "MapUnitDB->GetALLByUnitNameForBD");
                // Return null in case of an error
                return new List<DTOMapUnitResponse>();
            }

        }


        /// <summary>
        /// Retrieves a unit by its UnitMapId.
        /// </summary>
        /// <param name="UnitMapId">The UnitMapId to search for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unit details if found, otherwise null.</returns>
        public Task<DTOMapUnitResponse> GetALLByUnitMapId(int UnitMapId)
        {
            var Div = (from uni in _context.MapUnit
                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd on uni.ComdId equals Com.ComdId
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                       join pso in _context.MPso on uni.PsoId equals pso.PsoId
                       join FmnBranch in _context.MFmnBranches on uni.FmnBranchID equals FmnBranch.FmnBranchID
                       join SubDte in _context.MSubDte on uni.SubDteId equals SubDte.SubDteId
                       where uni.UnitMapId==UnitMapId
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           UnitAbbreviation= MUni.Abbreviation,
                           UnitId = uni.UnitMapId,
                           BdeId = bde.BdeId,
                           BdeName = bde.BdeName,
                           DivId = div.DivId,
                           DivName = div.DivName,
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                           Suffix = MUni.Suffix,
                           Sus_no = MUni.Sus_no,
                           UnitType = uni.UnitType,
                           PsoId = pso.PsoId,
                           PSOName = pso.PSOName,
                           FmnBranchID = FmnBranch.FmnBranchID,
                           BranchName = FmnBranch.BranchName,
                           SubDteId = SubDte.SubDteId,
                           SubDteName = SubDte.SubDteName,
                       }
                     ).Distinct().SingleOrDefault() ;




            return Task.FromResult(Div);
        }

        /// <summary>
        /// Retrieves a unit by its UnitMapId.
        /// </summary>
        /// <param name="UnitMapId">The UnitMapId to search for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unit details if found, otherwise null.</returns>
        public async Task<DTOMapUnitResponse> GetALLByUnitById(int UnitId)
        {


            var Div =await (from uni in _context.MapUnit
                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd
                       on uni.ComdId equals Com.ComdId
                       //   on new { uni.ComdId } equals new { Com.ComdId }
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                            join pso in _context.MPso on uni.PsoId equals pso.PsoId
                            join FmnBranch in _context.MFmnBranches on uni.FmnBranchID equals FmnBranch.FmnBranchID
                            join SubDte in _context.MSubDte on uni.SubDteId equals SubDte.SubDteId
                            where MUni.UnitId == UnitId
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           UnitId = uni.UnitId,
                           BdeId = bde.BdeId,
                           BdeName = bde.BdeName,
                           DivId = div.DivId,
                           DivName = div.DivName,
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                           Suffix = MUni.Suffix,
                           Sus_no = MUni.Sus_no,
                           UnitType = uni.UnitType,
                           PsoId = pso.PsoId,
                           PSOName = pso.PSOName,
                           FmnBranchID = FmnBranch.FmnBranchID,
                           BranchName = FmnBranch.BranchName,
                           SubDteId = SubDte.SubDteId,
                           SubDteName = SubDte.SubDteName,
                       }
                     ).Distinct().SingleOrDefaultAsync();





            return (Div);
        }

        /// <summary>
        /// Saves or updates the unit mapping with the provided details, within a transaction.
        /// </summary>
        /// <param name="dTO">The DTO containing the unit mapping details to be saved or updated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the operation succeeded, false otherwise.</returns>
        public async Task<bool?> SaveUnitWithMapping(DTOSaveUnitWithMappingByAdminRequest dTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (dTO.UnitMapId == 0)
                    {
                        MUnit? mUnit = await _context.MUnit.FindAsync(dTO.UnitId);
                        if(mUnit!=null)
                        {
                            var mapUnit = new MapUnit
                            {
                                UnitId = mUnit.UnitId,
                                UnitType = dTO.UnitType,
                                ComdId = dTO.ComdId,
                                CorpsId = dTO.CorpsId,
                                DivId = dTO.DivId,
                                BdeId = dTO.BdeId,
                                FmnBranchID = dTO.FmnBranchID,
                                PsoId = dTO.PsoId,
                                SubDteId = dTO.SubDteId,
                                IsActive = true,
                                Updatedby = dTO.Updatedby,
                                UpdatedOn = dTO.UpdatedOn,
                            };
                            await _context.MapUnit.AddAsync(mapUnit);
                            await _context.SaveChangesAsync();

                            mUnit.IsVerify = dTO.IsVerify;
                            await _context.SaveChangesAsync();
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        MUnit? mUnit = await _context.MUnit.FindAsync(dTO.UnitId);
                        if (mUnit != null)
                        {
                            MapUnit? mapUnit = await _context.MapUnit.FindAsync(dTO.UnitMapId);
                            if(mapUnit!=null)
                            {

                                mapUnit.UnitId = mUnit.UnitId;
                                mapUnit.UnitType = dTO.UnitType;
                                mapUnit.ComdId = dTO.ComdId;
                                mapUnit.CorpsId = dTO.CorpsId;
                                mapUnit.DivId = dTO.DivId;
                                mapUnit.BdeId = dTO.BdeId;
                                mapUnit.FmnBranchID = dTO.FmnBranchID;
                                mapUnit.PsoId = dTO.PsoId;
                                mapUnit.SubDteId = dTO.SubDteId;
                                mapUnit.IsActive = true;
                                mapUnit.Updatedby = dTO.Updatedby;  
                                mapUnit.UpdatedOn = dTO.UpdatedOn;

                                _context.MapUnit.Update(mapUnit);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                return false;
                            }
                            
                            mUnit.IsVerify = dTO.IsVerify;
                            await _context.SaveChangesAsync();
                            
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(1001, ex, "MapUnitDB->SaveUnitWithMapping");
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks if a given UnitMapId is referenced in any foreign key tables.
        /// </summary>
        /// <param name="UnitMapId">The UnitMapId to check for foreign key references.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a DTO with counts of references in different tables, or null if an error occurred.</returns>
        public async Task<DTOUnitMapIdCheckInFKTableResponse?> UnitMapIdCheckInFKTable(int UnitMapId)
        {
            try
            {
                string query = "Select count(distinct bd.BasicDetailId) as TotalBD, count(distinct mro.RecordOfficeId) as TotalRO, count(distinct tdm.Id) as TotalTDM, count(distinct tfwd.TrnFwdId) as TotalTF,count(distinct tpo.Id)as TotalTPOFrom,count(distinct tpo_.Id)as TotalTPOTo from MapUnit munit" +
                                " left join BasicDetails bd on bd.UnitId = munit.UnitMapId " +
                                " left join MRecordOffice mro on mro.UnitId =  munit.UnitMapId " +
                                " left join TrnDomainMapping tdm on tdm.UnitId = munit.UnitMapId " +
                                " left join TrnFwds tfwd on tfwd.UnitId = munit.UnitMapId " +
                                " left join TrnPostingOut tpo on tpo.FromUnitID= munit.UnitMapId " +
                                " left join TrnPostingOut tpo_ on tpo_.ToUnitID= munit.UnitMapId " +
                                " where munit.UnitMapId=@UnitMapId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOUnitMapIdCheckInFKTableResponse>(query, new { UnitMapId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->UnitMapIdCheckInFKTable");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of units based on their hierarchical relationship, filtered by provided unit parameters.
        /// </summary>
        /// <param name="dTO">The DTO containing filtering parameters such as UnitType, UnitMapId, ComdId, CorpsId, DivId, BdeId, etc.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a list of DTOUnitResponse objects representing units matching the provided filters.</returns>
        public async Task<List<DTOUnitResponse>> GetUnitByHierarchy(DTOMHierarchyRequest dTO)
        {
            try
            {
                string query = @"SELECT unit.UnitMapId as UnitId,unt.UnitName,unt.Suffix,unt.Sus_no FROM MapUnit unit
                                Inner join MUnit unt on unt.UnitId = unit.UnitId
                                WHERE
	                                unit.UnitType =@UnitType
	                                AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                AND(
                                    (@UnitType = 1 AND
                                        unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                        AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                        AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                        AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    )
                                    OR
                                    (@UnitType = 2 AND
                                        unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                        AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                        AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                        AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                    )
                                    OR
                                    (@UnitType = 3 AND
                                        unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                        AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                    )
                                )";

                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UnitMapId", dTO.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitType", dTO.UnitType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", dTO.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", dTO.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", dTO.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", dTO.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", dTO.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", dTO.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", dTO.SubDteId, DbType.Byte, ParameterDirection.Input);

                    var ret = await connection.QueryAsync<DTOUnitResponse>(query, parameters);
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->GetUnitByHierarchy");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of units for an Icard request, filtered by provided parameters.
        /// </summary>
        /// <param name="Data">The DTO containing filtering parameters such as ComdId, CorpsId, DivId, BdeId, and UnitMapId.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a list of DTOUnitResponse objects representing units matching the provided filters.</returns>
        public async Task<List<DTOUnitResponse>> GetUnitByHierarchyForIcardRequest(DTOMHierarchyRequest Data)
        {
            try
            {
                string query = " SELECT distinct munit.UnitMapId UnitId FROM TrnICardRequest trnicrd" +
                               " inner join BasicDetails B on trnicrd.BasicDetailId = B.BasicDetailId" +
                               " inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId" +
                               " inner join MapUnit munit on map.UnitId=munit.UnitMapId" +
                               " where munit.ComdId=ISNULL(@ComdId,munit.ComdId) " +
                               " and munit.CorpsId=ISNULL(@CorpsId,munit.CorpsId)" +
                               " and munit.DivId=ISNULL(@DivId,munit.DivId)" +
                               " and munit.BdeId=ISNULL(@BdeId,munit.BdeId)" +
                               " and munit.UnitMapId=ISNULL(@UnitId,munit.UnitMapId)";


                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOUnitResponse>(query, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.UnitMapId });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->GetUnitByHierarchy");
                return null;
            }
        }
    }
 }
