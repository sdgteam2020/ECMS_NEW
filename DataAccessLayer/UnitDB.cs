using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Logger;
using Dapper;
using Azure.Core;

namespace DataAccessLayer
{
    public class UnitDB : GenericRepositoryDL<MUnit>, IUnitDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<UnitDB> _logger;
        public UnitDB(ApplicationDbContext context, ILogger<UnitDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _logger = logger;
            _contextDP = contextDP;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MUnit Data)
        {
            List<MUnit> mUnits = await _context.MUnit.AsNoTracking().ToListAsync();
            var ret = mUnits.Any(p => p.UnitName.ToUpper() == Data.UnitName.ToUpper() && p.UnitId !=Data.UnitId);
            return ret;
        }
        public async Task<bool> FindSusNo(string Sus_no)
        {
            var ret = _context.MUnit.Any(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no.ToUpper());
            return ret;
        }

        public async Task<MUnit?> GetBySusNo(string Sus_no)
        {
            try
            {
                return await _context.MUnit.Where(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetBySusNo");
                return null;
            }
        }
        public async Task<bool?> GetBySusNoWithUnitId(string Sus_no, int UnitId)
        {
            try
            {
                return await _context.MUnit.AnyAsync(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no && x.UnitId != UnitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetBySusNoWithUnitId");
                return null;
            }
        }
        public async Task<DTODataTablesResponse<MUnit>> GetAllUnit(DTODataTablesRequest request)
        {
            try
            {
                var queryableData = (from u in _context.MUnit.OrderByDescending(x => x.UnitId)
                                     select new MUnit()
                                     {
                                         UnitId = u.UnitId,
                                         Sus_no = u.Sus_no,
                                         Suffix = u.Suffix,
                                         UnitName = u.UnitName,
                                         Abbreviation = u.Abbreviation,
                                         IsVerify = u.IsVerify,
                                     }).AsQueryable();
                // Total records without filtering
                var totalRecords = queryableData.Count();


                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    queryableData = queryableData.Where(x => x.Sus_no.ToLower().Contains(searchValue));
                }

                // Apply sorting

                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    if (request.sortColumn == "UnitName" || request.sortColumn == "Abbreviation")
                    {

                    }
                    else 
                    {
                        //queryableData = queryableData.OrderBy(request.SortColumn + " " + request.SortColumnDirection);
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                }

                // Total records after filtering
                var filteredRecords = queryableData.Count();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                var responseData = new DTODataTablesResponse<MUnit>
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
                _logger.LogError(1001, ex, "UnitDB->GetAllUnit_");
                List<MUnit> dTOUserRegnResponses = new List<MUnit>();
                var responseData = new DTODataTablesResponse<MUnit>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }
        public async Task<List<DTOUnitResponse>?> GetTopBySUSNo(string SUSNo)
        {
            try
            {
                var Unit = await (from unit in _context.MUnit.Where(x => (x.Sus_no + x.Suffix).Contains(SUSNo))
                                  select new DTOUnitResponse
                                  {
                                      UnitId = unit.UnitId,
                                      Sus_no = unit.Sus_no + unit.Suffix,
                                      UnitName = unit.UnitName,
                                      Abbreviation = unit.Abbreviation,
                                      IsVerify = unit.IsVerify,
                                  }).Take(5).ToListAsync();
                return Unit;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetTopBySUSNo");
                return null;
            }

        }
        public async Task<DTOUnitIdCheckInFKTableResponse?> UnitIdCheckInFKTable(int UnitId)
        {
            try
            {
                string query = "Select  count(mapunit.UnitMapId) as TotalMapUnit from MUnit munit" +
                                " left join MapUnit mapunit on mapunit.UnitId = munit.UnitId " +
                                " where munit.UnitId = @UnitId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOUnitIdCheckInFKTableResponse>(query, new { UnitId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->UnitIdCheckInFKTable");
                return null;
            }
        }
    }
}