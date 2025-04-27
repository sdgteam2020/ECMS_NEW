using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class MapUnitChangeDB : GenericRepositoryDL<TrnMapUnitChangeRequest>, IMapUnitChangeDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<MapUnitChangeDB> _logger;
        public MapUnitChangeDB(ApplicationDbContext context, DapperContext contextDP, ILogger<MapUnitChangeDB> logger) : base(context)
        {
            _logger = logger;
            _contextDP = contextDP;
            _context = context;
        }
        public async Task<bool> FindUnitIdMapped(int UnitMapId)
        {
            try
            {
                return await _context.TrnMapUnitChangeRequest.AnyAsync(f => f.UnitMapId == UnitMapId && f.IsComplete == false);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitChangeDB->FindUnitIdMapped");
                return false;
            }
        }
        public async Task<DTODataTablesResponse<DTOMapUnitChangeResponse>> GetAllMapUnitChange(DTODataTablesRequestForMapUnitChange request)
        {
            try
            {
                var queryableData = (from unitch in _context.TrnMapUnitChangeRequest.OrderByDescending(x=>x.ChangeMapUnitId)
                                     join upfrom in _context.UserProfile on unitch.FromUserId equals upfrom.UserId
                                     join rkfrom in _context.MRank on upfrom.RankId equals rkfrom.RankId
                                     join ufrom in _context.Users on unitch.Updatedby equals ufrom.Id
                                     join mapunit in _context.MapUnit on unitch.UnitMapId equals mapunit.UnitMapId
                                     join munit in _context.MUnit on mapunit.UnitId equals munit.UnitId
                                     select new DTOMapUnitChangeResponse()
                                     {
                                         ChangeMapUnitId = unitch.ChangeMapUnitId,
                                         UnitMapId = unitch.UnitMapId,
                                         ExistingCh = unitch.ExistingCh,
                                         RequestCh = unitch.RequestCh,
                                         Remark= unitch.Remark,
                                         AdminRemark = unitch.AdminRemark,
                                         IsComplete = unitch.IsComplete,
                                         IsActive = unitch.IsActive,
                                         IsEditAction = unitch.IsEditAction,
                                         FromUpdatedby = unitch.Updatedby ?? 0,
                                         FromUpdatedOn = unitch.UpdatedOn ?? DateTime.Now,
                                         FromUserId = unitch.FromUserId,
                                         ToUpdatedby = unitch.Updatedby,
                                         ToUpdatedOn = unitch.UpdatedOn,
                                         ToUserId = unitch.ToUserId,
                                         UnitName = munit.UnitName,
                                         FromDID = ufrom.DomainId,
                                         FromRankAbbreviation= rkfrom.RankAbbreviation,
                                         FromArmyNo = upfrom.ArmyNo,
                                         FromName = upfrom.Name,
                                     }).AsQueryable();
                if (request.RoleName != "admin")
                {
                    queryableData = queryableData.Where(x => x.UnitMapId == request.UnitMapId);
                }

                // Total records without filtering
                var totalRecords = queryableData.Count();

                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();

                    //queryableData = queryableData.Where(x =>  x.UserId.ToString().ToLower().Contains(searchValue) ||
                    //                          x.DomainId.ToLower().Contains(searchValue)||
                    //                          x.ArmyNo.ToLower().Contains(searchValue));

                    queryableData = queryableData.Where(x => x.FromArmyNo.ToLower().Contains(searchValue));
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

                var responseData = new DTODataTablesResponse<DTOMapUnitChangeResponse>
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
                _logger.LogError(1001, ex, "MapUnitChangeDB->GetAllMapUnitChange");
                List<DTOMapUnitChangeResponse> dTOUserRegnResponses = new List<DTOMapUnitChangeResponse>();
                var responseData = new DTODataTablesResponse<DTOMapUnitChangeResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }
    }
}
