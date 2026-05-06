using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;


namespace DataAccessLayer
{
    public class AccountDB : GenericRepositoryDL<ApplicationUser>, IAccountDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly ILogger<AccountDB> _logger;
        private readonly IDataProtector protector;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserProfileDB userProfileDB;
        private readonly IDomainMapDB domainMapDB;
        private readonly ITrnMappingUnMappingLogDB _trnMappingUnMappingLogDB;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public AccountDB(ApplicationDbContext context, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<AccountDB> logger, UserManager<ApplicationUser> userManager, IUserProfileDB userProfileDB, IDomainMapDB domainMapDB, ITrnMappingUnMappingLogDB trnMappingUnMappingLogDB, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            _passwordHasher = passwordHasher;
            this.userManager = userManager;
            this.userProfileDB = userProfileDB;
            this.domainMapDB = domainMapDB;
            _trnMappingUnMappingLogDB = trnMappingUnMappingLogDB;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        /// <summary>
        /// Returns the total count of user profiles in the system.
        /// </summary>
        /// <returns>
        /// An <see cref="int"/> representing the total number of user profiles.
        /// </returns>
        public async Task<int> TotalProfileCount()
        {
            int ret = await _context.UserProfile.CountAsync();
            return ret;
        }
        
        
        /// <summary>
        /// Checks whether any other user already has the specified <paramref name="DomainId"/>.
        /// </summary>
        /// <param name="DomainId">The domain identifier to test for uniqueness.</param>
        /// <param name="Id">The current record's user ID to exclude from the check.</param>
        /// <returns>
        /// <c>true</c> if a different user (ID != <paramref name="Id"/>) exists with the same
        /// <paramref name="DomainId"/> (case-insensitive); otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Uses a case-insensitive comparison via <c>ToUpper()</c> and excludes the provided ID to
        /// allow updates without triggering a false duplicate.
        /// </remarks>
        public bool GetByDomainId(string DomainId, int Id)
        {
            var ret = _context.Users.Any(x => x.DomainId.ToUpper() == DomainId.ToUpper() && x.Id != Id);
            return ret;
        }
        
        
        public async Task<DTOAccountResponse?> FindDomainId(string DomainId)
        {
            try
            {
                ApplicationUser? applicationUser = await _context.Users.Where(x => x.DomainId == DomainId).FirstOrDefaultAsync();

                if (applicationUser != null)
                {
                    DTOAccountResponse dTOAccountResponse = new DTOAccountResponse();
                    dTOAccountResponse.Id = applicationUser.Id;
                    dTOAccountResponse.DomainId = applicationUser.DomainId;
                    dTOAccountResponse.Active = applicationUser.Active;
                    dTOAccountResponse.AdminFlag = applicationUser.AdminFlag;
                    return dTOAccountResponse;
                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->DomainApproveList");
                return null;
            }

        }
        
        
        public async Task<bool?> FindRoleByName(string Role)
        {
            try
            {
                var ret = await _context.Roles.AnyAsync(x => x.Name !=null ?x.Name.ToUpper() == Role.ToUpper():false);
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->FindRoleByName");
                return null;
            }
        }
        
        
        /// <summary>Builds a server-side DataTables payload of domain registrations.</summary>
        /// <param name="request">DataTables request: draw/start/length/search/sort.</param>
        /// <returns><see cref="DTODataTablesResponse{DTODomainRegnResponse}"/> with totals and current page rows.</returns>
        /// <remarks>
        /// Steps: join Users←→TrnDomainMapping←→UserProfile; count total; search DomainId (case-insensitive);
        /// sort via EF.Property; count filtered; page with Skip/Take; map roles & claims;
        /// on exception log(1001) and return empty (draw=0).
        /// </remarks>
        public async Task<DTODataTablesResponse<DTODomainRegnResponse>> GetAllDomainRegn(DTODataTablesRequest request)
        {
            try
            {
                var queryableData = (from u in _context.Users.OrderByDescending(x => x.Id)
                                       join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                       from xtdm in utdm_jointable.DefaultIfEmpty()
                                       join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                       from xup in xtdmup_jointable.DefaultIfEmpty()
                                       select new DTODomainRegnResponse
                                       {
                                           Id = u.Id,
                                           DomainId = u.DomainId,
                                           AdminFlag = u.AdminFlag,
                                           Active = u.Active,
                                           UpdatedOn = u.UpdatedOn,
                                           Mapped = xup != null ? true : false,
                                           TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                           TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                           TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                           IsIO = xtdm != null ? xtdm.IsIO : false,
                                           IsCO = xtdm != null ? xtdm.IsCO : false,
                                           IsRO = xtdm != null ? xtdm.IsRO : false,
                                           IsORO = xtdm != null ? xtdm.IsORO : false,
                                           ArmyNo = xup != null ? xup.ArmyNo : null,
                                           UserId = xup != null ? xup.UserId : 0,
                                       }).AsQueryable();

                // Total records without filtering
                var totalRecords = await queryableData.CountAsync();


                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(searchValue));
                }

                // Apply sorting

                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    queryableData = request.sortDirection.ToLower() == "asc"
                    ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                    : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                }

                // Total records after filtering
                var filteredRecords = await queryableData.CountAsync();


                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                var userIds = paginatedData.Select(u => u.Id).ToList();

                var roles = await _context.UserRoles
                    .Where(ur => userIds.Contains(ur.UserId))
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name, ur.RoleId })
                    .ToListAsync();

                var claims = await _context.UserClaims
                    .Where(ct => userIds.Contains(ct.UserId))
                    .Select(ct => new { ct.UserId, ct.ClaimType, ct.ClaimValue })
                    .ToListAsync();

                // Map roles and claims to users
                for (int i = 0; i < paginatedData.Count; i++)
                {
                    var user = paginatedData[i];
                    user.RoleNames = roles.Where(r => r.UserId == user.Id).Select(r => r.Name).ToList();
                    user.RoleIds = roles.Where(r => r.UserId == user.Id).Select(r => r.RoleId).ToList();
                    user.ClaimTypes = claims.Where(c => c.UserId == user.Id).Select(c => c.ClaimType).ToList();
                    user.ClaimValues = claims.Where(c => c.UserId == user.Id).Select(c => c.ClaimValue).ToList();
                }

                return new DTODataTablesResponse<DTODomainRegnResponse>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = paginatedData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->GetAllDomainRegn");
                List<DTODomainRegnResponse> dTOUserRegnResponses = new List<DTODomainRegnResponse>();
                var responseData = new DTODataTablesResponse<DTODomainRegnResponse>
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
        /// Retrieves a paginated, sortable, and searchable list of user registration data for DataTables.
        /// </summary>
        /// <param name="request">
        /// DataTables request containing pagination, sorting, filtering, and choice parameters.
        /// </param>
        /// <returns>
        /// A <see cref="DTODataTablesResponse{DTOUserRegnResponse}"/> containing the total records, filtered records, and the current page of user registration data.
        /// </returns>
        /// <remarks>
        /// Supports multiple filter choices (e.g., User, MappedUser, UnMappedUser, ActiveUser, InActiveUser, Verified, NotVerifiedUser, IO, CO).
        /// Joins <c>Users</c> with <c>TrnDomainMapping</c> and <c>UserProfile</c> to build the result set.
        /// Applies filtering by <c>DomainId</c>, sorting by any column, and paginates the results.
        /// On exception, logs the error and returns an empty response.
        /// </remarks>
        public async Task<DTODataTablesResponse<DTOUserRegnResponse>> GetDataForDataTable(DTODataTablesRequest request)
        {
            try
            {
                if (request.Choice == "User")
                {
                    var queryableData = (from u in _context.Users.OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                         from xtdm in utdm_jointable.DefaultIfEmpty()
                                         join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         select new DTOUserRegnResponse()
                                         {  
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = xtdm.UserId != null ? true : false,
                                             TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                             TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                             TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                             IsIO = xtdm != null ? xtdm.IsIO : false,
                                             IsCO = xtdm != null ? xtdm.IsCO : false,
                                             IsRO = xtdm != null ? xtdm.IsRO : false,
                                             IsORO = xtdm != null ? xtdm.IsORO : false,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else if (request.Choice == "MappedUser")
                {
                    var queryableData = (from u in _context.Users.OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId
                                         join up in _context.UserProfile on tdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         where tdm.UserId != null
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = true,
                                             TrnDomainMappingId = tdm.Id,
                                             TrnDomainMappingApptId = tdm.ApptId,
                                             TrnDomainMappingUnitId = tdm.UnitId,
                                             IsIO = tdm.IsIO,
                                             IsCO = tdm.IsCO,
                                             IsRO = tdm.IsRO,
                                             IsORO = tdm.IsORO,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else if (request.Choice == "UnMappedUser")
                {
                    var queryableData = (from u in _context.Users.OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId
                                         join up in _context.UserProfile on tdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         where tdm.UserId == null
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = true,
                                             TrnDomainMappingId = tdm.Id,
                                             TrnDomainMappingApptId = tdm.ApptId,
                                             TrnDomainMappingUnitId = tdm.UnitId,
                                             IsIO = tdm.IsIO,
                                             IsCO = tdm.IsCO,
                                             IsRO = tdm.IsRO,
                                             IsORO = tdm.IsORO,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else if (request.Choice == "ActiveUser")
                {
                    var queryableData = (from u in _context.Users.Where(x => x.Active == true).OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId
                                         join up in _context.UserProfile on tdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = true,
                                             TrnDomainMappingId = tdm.Id,
                                             TrnDomainMappingApptId = tdm.ApptId,
                                             TrnDomainMappingUnitId = tdm.UnitId,
                                             IsIO = tdm.IsIO,
                                             IsCO = tdm.IsCO,
                                             IsRO = tdm.IsRO,
                                             IsORO = tdm.IsORO,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else if (request.Choice == "InActiveUser")
                {
                    var queryableData = (from u in _context.Users.Where(x => x.Active == false).OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId
                                         join up in _context.UserProfile on tdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = true,
                                             TrnDomainMappingId = tdm.Id,
                                             TrnDomainMappingApptId = tdm.ApptId,
                                             TrnDomainMappingUnitId = tdm.UnitId,
                                             IsIO = tdm.IsIO,
                                             IsCO = tdm.IsCO,
                                             IsRO = tdm.IsRO,
                                             IsORO = tdm.IsORO,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else if (request.Choice == "Verified")
                {
                    var queryableData = (from u in _context.Users.Where(x => x.AdminFlag == true).OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId
                                         join up in _context.UserProfile on tdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = true,
                                             TrnDomainMappingId = tdm.Id,
                                             TrnDomainMappingApptId = tdm.ApptId,
                                             TrnDomainMappingUnitId = tdm.UnitId,
                                             IsIO = tdm.IsIO,
                                             IsCO = tdm.IsCO,
                                             IsRO = tdm.IsRO,
                                             IsORO = tdm.IsORO,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else if (request.Choice == "NotVerifiedUser")
                {
                    var queryableData = (from u in _context.Users.Where(x => x.AdminFlag == false).OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId
                                         join up in _context.UserProfile on tdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = true,
                                             TrnDomainMappingId = tdm.Id,
                                             TrnDomainMappingApptId = tdm.ApptId,
                                             TrnDomainMappingUnitId = tdm.UnitId,
                                             IsIO = tdm.IsIO,
                                             IsCO = tdm.IsCO,
                                             IsRO = tdm.IsRO,
                                             IsORO = tdm.IsORO,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else if (request.Choice == "IO")
                {
                    var queryableData = (from u in _context.Users.OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId
                                         join up in _context.UserProfile on tdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         where tdm.IsIO == true
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = true,
                                             TrnDomainMappingId = tdm.Id,
                                             TrnDomainMappingApptId = tdm.ApptId,
                                             TrnDomainMappingUnitId = tdm.UnitId,
                                             IsIO = tdm.IsIO,
                                             IsCO = tdm.IsCO,
                                             IsRO = tdm.IsRO,
                                             IsORO = tdm.IsORO,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else if (request.Choice == "CO")
                {
                    var queryableData = (from u in _context.Users.OrderByDescending(x => x.Id)
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId
                                         join up in _context.UserProfile on tdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         where tdm.IsCO == true
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = true,
                                             TrnDomainMappingId = tdm.Id,
                                             TrnDomainMappingApptId = tdm.ApptId,
                                             TrnDomainMappingUnitId = tdm.UnitId,
                                             IsIO = tdm.IsIO,
                                             IsCO = tdm.IsCO,
                                             IsRO = tdm.IsRO,
                                             IsORO = tdm.IsORO,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
                else
                {
                    var queryableData = (from u in _context.Users
                                         join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                         from xtdm in utdm_jointable.DefaultIfEmpty()
                                         join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                         from xup in xtdmup_jointable.DefaultIfEmpty()
                                         select new DTOUserRegnResponse()
                                         {
                                             Id = u.Id,
                                             DomainId = u.DomainId,
                                             AdminMsg = u.AdminMsg,
                                             AdminFlag = u.AdminFlag,
                                             Active = u.Active,
                                             UpdatedOn = u.UpdatedOn,
                                             Mapped = xtdm.UserId != null ? true : false,
                                             TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                             TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                             TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                             IsIO = xtdm != null ? xtdm.IsIO:false,
                                             IsCO = xtdm != null ? xtdm.IsCO : false,
                                             IsRO = xtdm != null ? xtdm.IsRO : false,
                                             IsORO = xtdm != null ? xtdm.IsORO : false,
                                             ArmyNo = xup != null ? xup.ArmyNo : null,
                                             UserId = xup != null ? xup.UserId : 0,
                                             RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          select r.Name).ToList(),
                                         }).AsQueryable();
                    // Total records without filtering
                    var totalRecords = queryableData.Count();


                    // Apply filtering
                    if (!string.IsNullOrEmpty(request.searchValue))
                    {
                        queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(request.searchValue));
                    }

                    // Apply sorting

                    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                    {
                        
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                    // Total records after filtering
                    var filteredRecords = queryableData.Count();

                    // Paginate the result
                    var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = request.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = filteredRecords, // Total records after filtering
                        data = paginatedData
                    };
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->GetDataForDataTable");
                List<DTOUserRegnResponse> dTOUserRegnResponses = new List<DTOUserRegnResponse>();
                var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
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
        /// Retrieves a paginated, sortable, and searchable list of all user registrations.
        /// </summary>
        /// <param name="request">
        /// DataTables request containing pagination, sorting, and search parameters.
        /// </param>
        /// <returns>
        /// A <see cref="DTODataTablesResponse{DTOUserRegnResponse}"/> containing the total records, filtered records, and the current page of user registration data.
        /// </returns>
        /// <remarks>
        /// Joins <c>Users</c> with <c>TrnDomainMapping</c> and <c>UserProfile</c> to build the result set.
        /// Supports filtering by <c>DomainId</c>, sorting by any column, and paginates the results.
        /// On exception, logs the error and returns an empty response.
        /// </remarks>
        public async Task<DTODataTablesResponse<DTOUserRegnResponse>> GetAllUserRegn(DTODataTablesRequest request)
        {
            try
            {
                var queryableData = (from u in _context.Users.OrderByDescending(x => x.Id)
                                           join tdm in _context.TrnDomainMapping on u.Id equals tdm.AspNetUsersId into utdm_jointable
                                           from xtdm in utdm_jointable.DefaultIfEmpty()
                                           join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                           from xup in xtdmup_jointable.DefaultIfEmpty()
                                           select new DTOUserRegnResponse()
                                           {
                                               Id = u.Id,
                                               DomainId = u.DomainId,
                                               AdminMsg = u.AdminMsg,
                                               AdminFlag = u.AdminFlag,
                                               Active = u.Active,
                                               UpdatedOn = u.UpdatedOn,
                                               TrnDomainMappingId = xtdm != null ? xtdm.Id : 0,
                                               TrnDomainMappingApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                               TrnDomainMappingUnitId = xtdm != null ? xtdm.UnitId : 0,
                                               IsIO = xtdm != null ? xtdm.IsIO : false,
                                               IsCO = xtdm != null ? xtdm.IsCO : false,
                                               IsRO = xtdm != null ? xtdm.IsRO : false,
                                               IsORO = xtdm != null ? xtdm.IsORO : false,
                                               Mapped = xtdm.UserId != null ? true : false,
                                               ArmyNo = xup != null ? xup.ArmyNo : null,
                                               UserId = xup != null ? xup.UserId : 0,
                                               RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == u.Id)
                                                            join r in _context.Roles on ur.RoleId equals r.Id
                                                            select r.Name).ToList(),
                                           }).AsQueryable();

                // Total records without filtering
                var totalRecords = await queryableData.CountAsync();


                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    queryableData = queryableData.Where(x => x.DomainId.ToLower().Contains(searchValue));
                }

                // Apply sorting

                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    
                    queryableData = request.sortDirection.ToLower() == "asc"
                    ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                    : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                }

                // Total records after filtering
                var filteredRecords = await queryableData.CountAsync();


                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                return new DTODataTablesResponse<DTOUserRegnResponse>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = paginatedData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->GetAllDomainRegn");
                List<DTOUserRegnResponse> dTOUserRegns = new List<DTOUserRegnResponse>();
                var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                {
                     draw = request.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegns
                };
                return responseData;
            }
        }

        
        /// <summary>
        /// Retrieves a paginated, sortable, and searchable list of user profiles for profile management.
        /// </summary>
        /// <param name="request">
        /// DataTables request containing pagination, sorting, and search parameters.
        /// </param>
        /// <returns>
        /// A <see cref="DTODataTablesResponse{DTOProfileManageResponse}"/> containing the total records, filtered records, and the current page of profile management data.
        /// </returns>
        /// <remarks>
        /// Joins <c>UserProfile</c> with <c>MRank</c>, <c>MArmedType</c>, <c>TrnDomainMapping</c>, and <c>Users</c> to build the result set.
        /// Supports filtering by ArmyNo, sorting by any column, and paginates the results.
        /// On exception, logs the error and returns an empty response.
        /// </remarks>
        public async Task<DTODataTablesResponse<DTOProfileManageResponse>> GetAllProfileManage(DTODataTablesRequest request)
        {
            try
            {
                var queryableData = (from up in _context.UserProfile
                                     join rk in _context.MRank on up.RankId equals rk.RankId
                                     join at in _context.MArmedType on up.ArmedId equals at.ArmedId
                                     join tdm in _context.TrnDomainMapping on up.UserId equals tdm.UserId into uptdm_jointable
                                     from xtdm in uptdm_jointable.DefaultIfEmpty()
                                     join u in _context.Users on xtdm.AspNetUsersId equals u.Id into xtdmu_jointable
                                     from xu in xtdmu_jointable.DefaultIfEmpty()
                                     select new DTOProfileManageResponse()
                                     {
                                         UserId = up.UserId,
                                         ArmyNo = up.ArmyNo,
                                         Name = up.Name,
                                         IsToken = up.IsToken,
                                         IsWithTokenApply = up.IsWithTokenApply,
                                         IsTokenWaiver = up.IsTokenWaiver,
                                         ReasonTokenWaiver = up.ReasonTokenWaiver,
                                         RankId = rk.RankId,
                                         RankName = rk.RankName,
                                         RankAbbreviation = rk.RankAbbreviation,
                                         ArmedId = at.ArmedId,
                                         ArmedName = at.ArmedName,
                                         Id = xu != null ? xu.Id : 0,
                                         DomainId = xu != null ? xu.DomainId : null,
                                     }).AsQueryable();

                // Total records without filtering
                var totalRecords = queryableData.Count();

                // Apply filtering
               if (!string.IsNullOrEmpty(request.searchValue))
               {
                   string searchValue = request.searchValue.ToLower();

                   queryableData = queryableData.Where(x => x.ArmyNo.ToLower().Contains(searchValue));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                   
                   queryableData = request.sortDirection.ToLower() == "asc"
                   ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                   : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                }

                // Total records after filtering
                var filteredRecords = queryableData.Count();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                var responseData = new DTODataTablesResponse<DTOProfileManageResponse>
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
                _logger.LogError(1001, ex, "AccountDB->ProfileManage");
                List<DTOProfileManageResponse> dTOUserRegnResponses = new List<DTOProfileManageResponse>();
                var responseData = new DTODataTablesResponse<DTOProfileManageResponse>
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
        /// Saves the mapping between a user and domain, with logging for both mapping and unmapping actions.
        /// </summary>
        /// <param name="dTO">
        /// The input DTO containing the details to map or unmap the user from the domain. Includes user ID, domain ID,
        /// updated information (updated by and updated date), and the transaction log history.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DTOUserRegnResultResponse"/> with:
        /// - <c>Result = true</c> on successful mapping or unmapping.
        /// - <c>Result = false</c> with an appropriate message if any validation fails (e.g., profile already mapped).
        /// </returns>
        /// <remarks>
        /// This method checks if the user is already mapped to a domain. If so, it either updates the mapping or unmaps the user.
        /// The mapping history is logged in <c>TrnMappingUnMapping_Log</c> for both actions:
        /// 1) If the user is mapped, a new mapping log is added and the domain mapping is updated with user information.
        /// 2) If the user is unmapping, it logs the current mapping status and updates the domain mapping to null.
        /// Validation checks ensure that the user isn't already mapped or requires removal first.
        /// </remarks>
        public async Task<DTOUserRegnResultResponse?> SaveMapping(DTOUserRegnMappingRequest dTO)
        {
            DTOUserRegnResultResponse dTOUserRegnResultResponse = new DTOUserRegnResultResponse();
            
            try
            {
                //Get Admin Profile Id for Mapping Log History
                TrnDomainMapping? trnDomainMappingAdmin = await domainMapDB.GetByAspnetUserIdBy(dTO.Updatedby);

                TrnDomainMapping trnDomainMapping = await domainMapDB.Get(dTO.TDMId);
                if (dTO.UserId > 0)
                {
                    DTOProfileResponse? dTOProfileResponse = await userProfileDB.GetProfileByUserId(dTO.UserId);//Check Profile Id valid or not
                    if (dTOProfileResponse != null && dTOProfileResponse.Mapping == false) //Not mapped to any Domain
                    {
                        //Insert Log History
                        var mapping_Log_Old = new TrnMappingUnMapping_Log()
                        {
                            TrnMappUnMapLogId = 0,
                            TDMId = trnDomainMapping.Id,
                            UserId = (int)(trnDomainMapping.UserId != null ? trnDomainMapping.UserId : 0),
                            DeregisterUserId = (int)(trnDomainMappingAdmin != null ? (trnDomainMappingAdmin.UserId != null ? trnDomainMappingAdmin.UserId : 0) : 0),
                            IsActive = true,
                            Updatedby = dTO.Updatedby,
                            UpdatedOn = dTO.UpdatedOn,
                        };
                        await _trnMappingUnMappingLogDB.Add(mapping_Log_Old);

                        var mapping_Log_New = new TrnMappingUnMapping_Log()
                        {
                            TrnMappUnMapLogId = 0,
                            TDMId = trnDomainMapping.Id,
                            UserId = dTO.UserId,
                            DeregisterUserId = (int)(trnDomainMappingAdmin != null ? (trnDomainMappingAdmin.UserId != null ? trnDomainMappingAdmin.UserId : 0) : 0),
                            IsActive = true,
                            Updatedby = dTO.Updatedby,
                            UpdatedOn = dTO.UpdatedOn,
                        };
                        await _trnMappingUnMappingLogDB.Add(mapping_Log_New);

                        trnDomainMapping.UserId = dTO.UserId;
                        trnDomainMapping.MappedBy = dTO.Updatedby;
                        trnDomainMapping.MappedDate = dTO.UpdatedOn;
                        trnDomainMapping.Updatedby = dTO.Updatedby;
                        trnDomainMapping.UpdatedOn = dTO.UpdatedOn;
                        await domainMapDB.Update(trnDomainMapping);
                        dTOUserRegnResultResponse.Result = true;
                        dTOUserRegnResultResponse.Message = "Profile mapped.";
                        return dTOUserRegnResultResponse;
                    }
                    else if (dTOProfileResponse != null && dTOProfileResponse.Mapping == true && dTOProfileResponse.DomainId != null && dTOProfileResponse.AspNetUsersId == dTO.Id)//Already mapped to same Domain
                    {
                        dTOUserRegnResultResponse.Result = false;
                        dTOUserRegnResultResponse.Message = "Profile Id -" + dTOProfileResponse.UserId + " is alredy mapped to Domain Id - " + dTOProfileResponse.DomainId + " in Sys.<br/>Action not required.";
                        return dTOUserRegnResultResponse;
                    }
                    else if (dTOProfileResponse != null && dTOProfileResponse.Mapping == true && dTOProfileResponse.DomainId != null && dTOProfileResponse.AspNetUsersId != dTO.Id)//Already mapped to other Domain
                    {
                        dTOUserRegnResultResponse.Result = false;
                        dTOUserRegnResultResponse.Message = "Profile Id -" + dTOProfileResponse.UserId + " is mapped to Domain Id - " + dTOProfileResponse.DomainId + " in Sys.<br/>Pl relieved first and try again.";
                        return dTOUserRegnResultResponse;
                    }
                    else
                    {
                        dTOUserRegnResultResponse.Result = false;
                        dTOUserRegnResultResponse.Message = "Army number not valid.";
                        return dTOUserRegnResultResponse;
                    }
                }
                else
                {
                    //Insert Log History
                    var mapping_Log = new TrnMappingUnMapping_Log()
                    {
                        TrnMappUnMapLogId = 0,
                        TDMId = trnDomainMapping.Id,
                        UserId = (int)(trnDomainMapping.UserId != null ? trnDomainMapping.UserId : 0),
                        DeregisterUserId = (int)(trnDomainMappingAdmin != null ? (trnDomainMappingAdmin.UserId!=null? trnDomainMappingAdmin.UserId : 0):0),
                        IsActive = true,
                        Updatedby = dTO.Updatedby,
                        UpdatedOn = dTO.UpdatedOn,
                    };
                    await _trnMappingUnMappingLogDB.Add(mapping_Log);

                    trnDomainMapping.UserId = null;
                    trnDomainMapping.MappedBy = null;
                    trnDomainMapping.MappedDate = null;
                    trnDomainMapping.Updatedby = dTO.Updatedby;
                    trnDomainMapping.UpdatedOn = dTO.UpdatedOn;
                    await domainMapDB.Update(trnDomainMapping);
                    dTOUserRegnResultResponse.Result = true;
                    dTOUserRegnResultResponse.Message = "Profile Unmapped.";
                    return dTOUserRegnResultResponse;
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->SaveMapping");
                return null;
            }
        }

        
        /// <summary>
        /// Creates or updates a domain registration (user), along with roles, claims,
        /// and <c>TrnDomainMapping</c>, inside a database transaction.
        /// </summary>
        /// <param name="dTO">
        /// Input DTO. If <c>Id &gt; 0</c>, updates the existing user; otherwise inserts a new user.
        /// Also carries role IDs, claim values, and mapping fields (e.g., <c>UnitMappId</c>, <c>ApptId</c>).
        /// </param>
        /// <returns>
        /// <c>true</c> on successful insert/update; <c>false</c> if the target user for update is not found;
        /// <c>null</c> if an exception occurs (transaction is rolled back and error is logged).
        /// </returns>
        /// <remarks>
        /// Update path:
        /// - Loads user; if missing returns <c>false</c>.
        /// - Updates identity fields (DomainId, UserName/Email normalized, Active, AdminFlag/Date, audit fields).
        /// - Replaces all current roles and claims with those in the request.
        /// - Upserts <c>TrnDomainMapping</c> (by <c>TDMId</c>).
        /// - Commits and updates security stamp.
        /// <para/>
        /// Insert path:
        /// - Creates a new user with provided flags and computed identity fields; sets a default password.
        /// - Adds requested roles and claims.
        /// - Inserts <c>TrnDomainMapping</c>.
        /// - Commits and updates security stamp.
        /// <para/>
        /// Uses IST for date fields and logs errors with event ID 1001. No exceptions are thrown out of this method.
        /// </remarks>
        public async Task<bool?> SaveDomainRegn(DTODomainRegnRequest dTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (dTO.Id > 0)
                    {
                        var userUpdate = await _context.Users.FindAsync(dTO.Id); //await userManager.FindByIdAsync(dTO.Id.ToString());

                        if (userUpdate == null)
                        {
                            return false;
                        }
                        else
                        {
                            userUpdate.DomainId = dTO.DomainId;
                            userUpdate.Active = dTO.Active;
                            userUpdate.Updatedby = dTO.Updatedby;
                            userUpdate.UpdatedOn = dTO.UpdatedOn;
                            userUpdate.UserName = dTO.DomainId.ToLower();
                            userUpdate.NormalizedUserName = dTO.DomainId.ToUpper();
                            userUpdate.Email = dTO.DomainId.ToLower() + "@army.mil";
                            userUpdate.NormalizedEmail = dTO.DomainId.ToUpper() + "@ARMY.MIL";
                            userUpdate.LockoutEnabled = true;
                            if (dTO.AdminFlag == true)
                            {
                                userUpdate.AdminFlag = dTO.AdminFlag;
                                userUpdate.AdminFlagDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            }
                            else
                            {
                                userUpdate.AdminFlag = dTO.AdminFlag;
                                userUpdate.AdminFlagDate = null;
                            }
                            _context.Users.Update(userUpdate);
                            await _context.SaveChangesAsync();

                            // Get Current User Role Ids
                            List<int> CurrentRoleIds = new List<int>();
                            CurrentRoleIds = (from ur in _context.UserRoles
                                              where ur.UserId == userUpdate.Id
                                              select ur.RoleId).ToList();

                            // Remove roles to the user
                            foreach (var roleId in CurrentRoleIds)
                            {
                                _context.UserRoles.Remove(new IdentityUserRole<int> { RoleId = roleId, UserId = userUpdate.Id });
                                await _context.SaveChangesAsync();
                            }

                            // Assign new roles to the user
                            foreach (var roleId in dTO.RoleIds)
                            {
                                await _context.UserRoles.AddAsync(new IdentityUserRole<int> { RoleId = roleId, UserId = userUpdate.Id });
                                await _context.SaveChangesAsync();
                            }

                            // Get Current Claims Values
                            List<int> CurrentClaimsIds = new List<int>();
                            CurrentClaimsIds = (from c in _context.UserClaims
                                              where c.UserId == userUpdate.Id
                                              select c.Id).ToList();

                            // Remove claims to the user
                            foreach (var claimsId in CurrentClaimsIds)
                            {
                                _context.UserClaims.Remove(new IdentityUserClaim<int> { Id= claimsId });
                                await _context.SaveChangesAsync();
                            }


                            // Assign new claims to the user
                            if (dTO.ClaimValues != null)
                            {
                                foreach (var claimValues in dTO.ClaimValues)
                                {
                                    ClaimsStore? claimsStore = await _context.ClaimsStore.Where(x => x.ClaimValue == claimValues).FirstOrDefaultAsync();
                                    if (claimsStore != null)
                                    {
                                        await _context.UserClaims.AddAsync(new IdentityUserClaim<int> { UserId = userUpdate.Id, ClaimValue = claimsStore.ClaimValue, ClaimType = claimsStore.ClaimType });
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                            if (dTO.TDMId > 0)
                            {
                                trnDomainMapping = await domainMapDB.Get(dTO.TDMId);
                                trnDomainMapping.AspNetUsersId = userUpdate.Id;
                                trnDomainMapping.UnitId = dTO.UnitMappId;
                                trnDomainMapping.ApptId = dTO.ApptId;
                                trnDomainMapping.IsIO = dTO.IsIO;
                                trnDomainMapping.IsCO = dTO.IsCO;
                                trnDomainMapping.IsRO = dTO.IsRO;
                                trnDomainMapping.IsORO = dTO.IsORO;
                                trnDomainMapping.IsActive = true;
                                trnDomainMapping.Updatedby = dTO.Updatedby;
                                trnDomainMapping.UpdatedOn = dTO.UpdatedOn;

                                _context.TrnDomainMapping.Update(trnDomainMapping);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                trnDomainMapping.AspNetUsersId = userUpdate.Id;
                                trnDomainMapping.UnitId = dTO.UnitMappId;
                                trnDomainMapping.ApptId = dTO.ApptId;
                                trnDomainMapping.IsIO = dTO.IsIO;
                                trnDomainMapping.IsCO = dTO.IsCO;
                                trnDomainMapping.IsRO = dTO.IsRO;
                                trnDomainMapping.IsORO = dTO.IsORO;
                                trnDomainMapping.IsActive = true;
                                trnDomainMapping.Updatedby = dTO.Updatedby;
                                trnDomainMapping.UpdatedOn = dTO.UpdatedOn;
                                await _context.TrnDomainMapping.AddAsync(trnDomainMapping);
                                await _context.SaveChangesAsync();
                            }
                            transaction.Commit();
                            await userManager.UpdateSecurityStampAsync(userUpdate);
                            return true;
                        }
                    }
                    else
                    {
                        var userAdd = new ApplicationUser
                        {
                            DomainId = dTO.DomainId,
                            Active = dTO.Active,
                            AdminFlag = dTO.AdminFlag,
                            AdminFlagDate = dTO.AdminFlag == true ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")) : null,
                            Updatedby = dTO.Updatedby,
                            UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                            UserName = dTO.DomainId.ToLower(),
                            NormalizedUserName = dTO.DomainId.ToUpper(),
                            Email = dTO.DomainId.ToLower() + "@army.mil",
                            NormalizedEmail = dTO.DomainId.ToUpper() + "@ARMY.MIL",
                            LockoutEnabled = true
                        };

                        userAdd.PasswordHash = _passwordHasher.HashPassword(userAdd, Environment.GetEnvironmentVariable("Common__Password") ?? string.Empty);//Default Password
                        await _context.Users.AddAsync(userAdd);
                        await _context.SaveChangesAsync();
                        int Id = userAdd.Id;
                        // Assign new roles to the user
                        foreach (var roleId in dTO.RoleIds)
                        {
                            await _context.UserRoles.AddAsync(new IdentityUserRole<int> { RoleId = roleId, UserId = Id });
                            await _context.SaveChangesAsync();
                        }

                        // Assign new claims to the user
                        if(dTO.ClaimValues != null)
                        {
                            foreach (var claimValues in dTO.ClaimValues)
                            {
                                ClaimsStore? claimsStore = await _context.ClaimsStore.Where(x => x.ClaimValue == claimValues).FirstOrDefaultAsync();
                                if(claimsStore != null)
                                {
                                    await _context.UserClaims.AddAsync(new IdentityUserClaim<int> { UserId = Id, ClaimValue = claimsStore.ClaimValue, ClaimType = claimsStore.ClaimType });
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }

                        var trnmapAdd = new TrnDomainMapping
                        {
                            AspNetUsersId = Id,
                            UnitId = dTO.UnitMappId,
                            ApptId = dTO.ApptId,
                            IsIO =dTO.IsIO,
                            IsCO=dTO.IsCO,
                            IsRO=dTO.IsRO,
                            IsORO=dTO.IsORO,
                            IsActive=true,
                            Updatedby= dTO.Updatedby,
                            UpdatedOn=dTO.UpdatedOn
                        };
                        await _context.TrnDomainMapping.AddAsync(trnmapAdd);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        await userManager.UpdateSecurityStampAsync(userAdd);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(1001, ex, "AccountDB->SaveDomainRegn");
                    return null;
                }
            }
        }


        /// <summary>
        /// Updates the domain flag and related user information for a specified user based on the provided DTO.
        /// </summary>
        /// <param name="dTO">The request DTO containing the updated values for the user.</param>
        /// <returns>
        /// <c>true</c> if the domain flag and user information were successfully updated;
        /// <c>false</c> if the update failed (e.g., user not found or update failed);
        /// <c>null</c> if an exception occurs during the process.
        /// </returns>
        /// <remarks>
        /// This method retrieves the user by ID, updates the user's active status, admin flag, and related properties,
        /// and saves the changes to the user using <c>userManager.UpdateAsync</c>. If the <c>AdminFlag</c> is set to true,
        /// the <c>AdminFlagDate</c> is updated to the current IST time. The method also handles errors by logging them
        /// and returning <c>null</c> in case of an exception.
        /// </remarks>
        public async Task<bool?> UpdateDomainFlag(DTOUserRegnUpdateDomainFlagRequest dTO)
        {
            try
            {
                var userUpdate = await userManager.FindByIdAsync(dTO.Id.ToString());

                if (userUpdate == null)
                {
                    return false;
                }
                else
                {
                    userUpdate.Active = dTO.Active;
                    userUpdate.AdminMsg = dTO.AdminMsg;
                    userUpdate.Updatedby = dTO.Updatedby;
                    userUpdate.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    if (dTO.AdminFlag == true)
                    {
                        userUpdate.AdminFlag = dTO.AdminFlag;
                        userUpdate.AdminFlagDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    }
                    else
                    {
                        userUpdate.AdminFlag = dTO.AdminFlag;
                        userUpdate.AdminFlagDate = null;
                    }
                    var result = await userManager.UpdateAsync(userUpdate);

                    if (result.Succeeded)
                    {
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
                _logger.LogError(1001, ex, "AccountDB->SaveDomainRegn");
                return null;
            }

        }


        /// <summary>
        /// Retrieves all application roles ordered by ID and maps them to <see cref="DTOMasterResponse"/> items.
        /// </summary>
        /// <returns>
        /// A list of <see cref="DTOMasterResponse"/> where each item contains the role ID and name
        /// (defaults to <c>"Role Name Blank"</c> when the source name is null).
        /// </returns>
        /// <remarks>
        /// Queries <c>_context.Roles</c>, orders by <c>Id</c>, materializes the results, and performs a simple
        /// projection to the DTO list.
        /// </remarks>
        public async Task<List<DTOMasterResponse>>GetAllRole()
        {
            List<DTOMasterResponse> lst = new List<DTOMasterResponse>();
            var Ret = await _context.Roles.OrderBy(x=>x.Id).ToListAsync();
            foreach (var r in Ret)
            {
                DTOMasterResponse db = new DTOMasterResponse()
                {
                    Id = r.Id,
                    Name = r.Name != null ? r.Name : "Role Name Blank",
                };
                lst.Add(db);
            }
            return lst;
        }


        /// <summary>
        /// Retrieves all claim definitions from the claims store and maps them to <see cref="DTOClaimsResponse"/>.
        /// </summary>
        /// <returns>
        /// A list of <see cref="DTOClaimsResponse"/> items ordered by <c>ClaimValue</c>.
        /// </returns>
        /// <remarks>
        /// Queries <c>_context.ClaimsStore</c>, orders by <c>ClaimValue</c>, and projects to DTOs (value + type).
        /// </remarks>
        public async Task<List<DTOClaimsResponse>> GetAllClaims()
        {
            List<DTOClaimsResponse> lst = new List<DTOClaimsResponse>();
            var Ret = await _context.ClaimsStore.OrderBy(x => x.ClaimValue).ToListAsync();
            foreach (var r in Ret)
            {
                DTOClaimsResponse db = new DTOClaimsResponse()
                {
                    ClaimValue = r.ClaimValue,
                    ClaimType = r.ClaimType,
                };
                lst.Add(db);
            }
            return lst;
        }

        /// <summary>
        /// Saves the profile and domain mapping for a user based on the provided <paramref name="model"/> and session details.
        /// </summary>
        /// <param name="model">The request model containing updated profile and domain mapping information.</param>
        /// <param name="dTOTempSession">Temporary session data used for saving profile and mapping information.</param>
        /// <returns>
        /// A <see cref="DTOTempSession"/> with the status updated to indicate whether the profile and mapping were saved successfully.
        /// Returns <c>null</c> if the process encounters any issues or fails.
        /// </returns>
        /// <remarks>
        /// This method handles different states based on <paramref name="dTOTempSession.Status"/>:
        /// <list type="bullet">
        ///     <item><description>Status 2: Inserts a new user, maps the user to a domain, and updates user profile details if necessary.</description></item>
        ///     <item><description>Status 3: Updates the domain mapping for an existing user profile.</description></item>
        ///     <item><description>Status 4: Updates the user profile and domain mapping, and logs the operation.</description></item>
        /// </list>
        /// Transactions are used to ensure atomicity of the database operations, and any errors or exceptions are logged with event ID 1001.
        /// </remarks>
        public async Task<DTOTempSession?> ProfileAndMappingSaving(DTOProfileAndMappingRequest model, DTOTempSession dTOTempSession)
        {
            if (dTOTempSession.Status == 2)//New Regn
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var user = new ApplicationUser
                        {
                            DomainId = dTOTempSession.DomainId,
                            Active = true,
                            AdminFlag = false,
                            Updatedby = 1,
                            UpdatedOn = model.UpdatedOn,
                            UserName = dTOTempSession.DomainId.ToLower(),
                            NormalizedUserName = dTOTempSession.DomainId.ToUpper(),
                            Email = dTOTempSession.DomainId.ToLower() + "@army.mil",
                            LockoutEnabled = true, // Enable lockout for this user
                            NormalizedEmail = dTOTempSession.DomainId.ToUpper() + "@ARMY.MIL"
                        };
                        user.PasswordHash = _passwordHasher.HashPassword(user, dTOTempSession.Password);
                        await _context.Users.AddAsync(user);
                        await _context.SaveChangesAsync();

                        // Assign new roles to the user
                        int RoleId = (from r in _context.Roles.Where(x => x.Name == dTOTempSession.RoleName)
                                      select r.Id).FirstOrDefault();
                        await _context.UserRoles.AddAsync(new IdentityUserRole<int> { RoleId = RoleId, UserId = user.Id });
                        await _context.SaveChangesAsync();


                        TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                        trnDomainMapping.AspNetUsersId = user.Id;
                        trnDomainMapping.UnitId = model.UnitMapId;
                        trnDomainMapping.ApptId = model.ApptId;
                        trnDomainMapping.IsRO = model.IsRO;
                        trnDomainMapping.IsIO = model.IsIO;
                        trnDomainMapping.IsCO = model.IsCO;
                        trnDomainMapping.IsORO = model.IsORO;
                        trnDomainMapping.Updatedby = user.Id;
                        trnDomainMapping.UpdatedOn = model.UpdatedOn;

                        if (dTOTempSession.UserId > 0)
                        {
                            MUserProfile? uptUserProfile = await _context.UserProfile.FindAsync(dTOTempSession.UserId);
                            if (uptUserProfile != null)
                            {
                                uptUserProfile.RankId = model.RankId;
                                uptUserProfile.Name = model.Name;
                                uptUserProfile.ArmedId = model.ArmedId;
                                uptUserProfile.IsTokenWaiver = model.IsTokenWaiver;
                                uptUserProfile.ReasonTokenWaiver = model.ReasonTokenWaiver;
                                uptUserProfile.Updatedby = user.Id;
                                uptUserProfile.UpdatedOn = model.UpdatedOn;
                                await _context.SaveChangesAsync();
                                user.AdminMsg = "Domian Id - " + user.DomainId + " & Profile Id- " + uptUserProfile.UserId + ".Your regn request was successfully placed with Admin for necy Approval. Pl note regn No - " + user.Id + " for future correspondence.";
                                trnDomainMapping.UserId = dTOTempSession.UserId;
                            }
                            else
                            {
                                return null;
                            }

                        }
                        else
                        {
                            var mUserProfile = new MUserProfile()
                            {
                                ArmyNo = dTOTempSession.ICNO,
                                RankId = model.RankId,
                                Name = model.Name,
                                ArmedId = model.ArmedId,
                                IsToken = true,
                                IsWithTokenApply=true,
                                IsTokenWaiver= model.IsTokenWaiver,
                                ReasonTokenWaiver = model.ReasonTokenWaiver,
                                Updatedby = user.Id,
                                UpdatedOn = model.UpdatedOn,
                                Thumbprint =model.Thumbprint,
                            };
                            await _context.UserProfile.AddAsync(mUserProfile);
                            await _context.SaveChangesAsync();
                            user.AdminMsg = "Domian Id - " + user.DomainId + " & Profile Id- " + mUserProfile.UserId + ".Your regn request was successfully placed with Admin for necy Approval. Pl note regn No - " + user.Id + " for future correspondence.";
                            trnDomainMapping.UserId = mUserProfile.UserId;
                        }
                        await _context.TrnDomainMapping.AddAsync(trnDomainMapping);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        await userManager.UpdateSecurityStampAsync(user);
                        
                        var mapping_Log = new TrnMappingUnMapping_Log()
                        {
                            TrnMappUnMapLogId = 0,
                            TDMId = trnDomainMapping.Id,
                            UserId = (int)trnDomainMapping.UserId,
                            DeregisterUserId = (int)trnDomainMapping.UserId,
                            IsActive = true,
                            Updatedby = user.Id,
                            UpdatedOn = model.UpdatedOn,
                        };
                        await _trnMappingUnMappingLogDB.Add(mapping_Log);

                        DTOTempSession dTOTempSessionResult = new DTOTempSession();

                        dTOTempSessionResult.AspNetUsersId = user.Id;
                        dTOTempSessionResult.TDMId = trnDomainMapping.Id;
                        dTOTempSessionResult.TDMUnitMapId = trnDomainMapping.UnitId;
                        dTOTempSessionResult.UserId = (int)(trnDomainMapping.UserId != null ? trnDomainMapping.UserId : 0);
                        dTOTempSessionResult.Status = 1;
                        return dTOTempSessionResult;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(1001, ex, "AccountDB->ProfileAndMappingSaving");
                        return null;
                    }

                }
            }   
            else if (dTOTempSession.Status == 3)//Mapping only
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        ApplicationUser? applicationUser = await _context.Users.FindAsync(dTOTempSession.AspNetUsersId);
                        if (applicationUser != null)
                        {
                            TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                            trnDomainMapping.AspNetUsersId = dTOTempSession.AspNetUsersId;
                            trnDomainMapping.UnitId = model.UnitMapId;
                            trnDomainMapping.ApptId = model.ApptId;
                            trnDomainMapping.IsRO = model.IsRO;
                            trnDomainMapping.IsIO = model.IsIO;
                            trnDomainMapping.IsCO = model.IsCO;
                            trnDomainMapping.IsORO = model.IsORO;
                            trnDomainMapping.Updatedby = dTOTempSession.AspNetUsersId;
                            trnDomainMapping.UpdatedOn = model.UpdatedOn;
                            if (dTOTempSession.UserId > 0)
                            {
                                MUserProfile? uptUserProfile = await _context.UserProfile.FindAsync(dTOTempSession.UserId);
                                if (uptUserProfile != null)
                                {
                                    uptUserProfile.RankId = model.RankId;
                                    uptUserProfile.Name = model.Name;
                                    uptUserProfile.ArmedId = model.ArmedId;
                                    uptUserProfile.IsTokenWaiver = model.IsTokenWaiver;
                                    uptUserProfile.ReasonTokenWaiver = model.ReasonTokenWaiver;
                                    uptUserProfile.Updatedby = dTOTempSession.AspNetUsersId;
                                    uptUserProfile.UpdatedOn = model.UpdatedOn;
                                    await _context.SaveChangesAsync();
                                    trnDomainMapping.UserId = dTOTempSession.UserId;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                MUserProfile mUserProfile = new MUserProfile();
                                mUserProfile.ArmyNo = dTOTempSession.ICNO;
                                mUserProfile.RankId = model.RankId;
                                mUserProfile.Name = model.Name;
                                mUserProfile.ArmedId = model.ArmedId;
                                mUserProfile.IsToken = true;
                                mUserProfile.IsWithTokenApply = true;
                                mUserProfile.IsTokenWaiver = model.IsTokenWaiver;
                                mUserProfile.ReasonTokenWaiver = model.ReasonTokenWaiver;
                                mUserProfile.Updatedby = dTOTempSession.AspNetUsersId;
                                mUserProfile.UpdatedOn = model.UpdatedOn;
                                mUserProfile.Thumbprint = model.Thumbprint;
                                await _context.UserProfile.AddAsync(mUserProfile);
                                await _context.SaveChangesAsync();
                                // TempData["success"] = "Your Profile Id - " + dTOTempSession.UserId + " has been successfully mapped to Domain Id - " + dTOTempSession.DomainId + ". > DB ";
                                trnDomainMapping.UserId = mUserProfile.UserId;
                            }

                            await _context.TrnDomainMapping.AddAsync(trnDomainMapping);
                            await _context.SaveChangesAsync();
                            if (model.IsTokenWaiver == true)
                            {
                                applicationUser.AdminFlag = false;
                                applicationUser.AdminMsg = "Domian Id - " + applicationUser.DomainId + " & Profile Id- " + trnDomainMapping.UserId + ".Your token request was successfully placed with Admin for necy Approval. Pl note regn No - " + trnDomainMapping.UserId + " for future correspondence.";
                                await _context.SaveChangesAsync();
                            }
                            transaction.Commit();

                            var mapping_Log = new TrnMappingUnMapping_Log()
                            {
                                TrnMappUnMapLogId = 0,
                                TDMId = trnDomainMapping.Id,
                                UserId = (int)trnDomainMapping.UserId,
                                DeregisterUserId = (int)trnDomainMapping.UserId,
                                IsActive = true,
                                Updatedby = trnDomainMapping.AspNetUsersId,
                                UpdatedOn = model.UpdatedOn,
                            };
                            await _trnMappingUnMappingLogDB.Add(mapping_Log);

                            DTOTempSession dTOTempSessionResult = new DTOTempSession();

                            dTOTempSessionResult.TDMId = trnDomainMapping.Id;
                            dTOTempSessionResult.TDMUnitMapId = trnDomainMapping.UnitId;
                            dTOTempSessionResult.UserId = (int)(trnDomainMapping.UserId != null ? trnDomainMapping.UserId : 0);
                            dTOTempSessionResult.Status = 1;
                            return dTOTempSessionResult;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(1001, ex, "AccountDB->ProfileAndMappingSaving");
                        return null;
                    }

                }
            }
            else if (dTOTempSession.Status == 4)//Profile Update with Mapping
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try 
                    {
                        TrnDomainMapping? trnDomainMapping = await _context.TrnDomainMapping.FindAsync(dTOTempSession.TDMId);
                        if(trnDomainMapping!=null)
                        {
                            ApplicationUser? applicationUser = await _context.Users.FindAsync(trnDomainMapping.AspNetUsersId);
                            if(applicationUser!=null)
                            {
                                if (dTOTempSession.UserId > 0)
                                {
                                    MUserProfile? uptUserProfile = await _context.UserProfile.FindAsync(dTOTempSession.UserId);
                                    if (uptUserProfile != null)
                                    {
                                        uptUserProfile.RankId = model.RankId;
                                        uptUserProfile.Name = model.Name;
                                        uptUserProfile.ArmedId = model.ArmedId;
                                        uptUserProfile.IsTokenWaiver = model.IsTokenWaiver;
                                        uptUserProfile.ReasonTokenWaiver = model.ReasonTokenWaiver;
                                        uptUserProfile.Updatedby = dTOTempSession.AspNetUsersId;
                                        uptUserProfile.UpdatedOn = model.UpdatedOn;
                                        await _context.SaveChangesAsync();

                                        trnDomainMapping.UserId = dTOTempSession.UserId;
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                                else
                                {
                                    MUserProfile mUserProfile = new MUserProfile();
                                    mUserProfile.ArmyNo = dTOTempSession.ICNO;
                                    mUserProfile.RankId = model.RankId;
                                    mUserProfile.Name = model.Name;
                                    mUserProfile.ArmedId = model.ArmedId;
                                    mUserProfile.IsToken = true;
                                    mUserProfile.IsWithTokenApply = true;
                                    mUserProfile.IsTokenWaiver = model.IsTokenWaiver;
                                    mUserProfile.ReasonTokenWaiver = model.ReasonTokenWaiver;
                                    mUserProfile.Updatedby = dTOTempSession.AspNetUsersId;
                                    mUserProfile.UpdatedOn = model.UpdatedOn;
                                    mUserProfile.Thumbprint = model.Thumbprint;
                                    await _context.UserProfile.AddAsync(mUserProfile);
                                    await _context.SaveChangesAsync();

                                    trnDomainMapping.UserId = mUserProfile.UserId;
                                }
                                _context.TrnDomainMapping.Update(trnDomainMapping);
                                await _context.SaveChangesAsync();

                                if (model.IsTokenWaiver == true)
                                {
                                    applicationUser.AdminMsg = "Domian Id - " + applicationUser.DomainId + " & Profile Id- " + trnDomainMapping.UserId + ".Your token request was successfully placed with Admin for necy Approval. Pl note regn No - " + trnDomainMapping.UserId + " for future correspondence.";
                                    await _context.SaveChangesAsync();
                                }

                                transaction.Commit();

                                var mapping_Log = new TrnMappingUnMapping_Log()
                                {
                                    TrnMappUnMapLogId = 0,
                                    TDMId = trnDomainMapping.Id,
                                    UserId = (int)trnDomainMapping.UserId,
                                    DeregisterUserId = (int)trnDomainMapping.UserId,
                                    IsActive = true,
                                    Updatedby = trnDomainMapping.AspNetUsersId,
                                    UpdatedOn = model.UpdatedOn,
                                };
                                await _trnMappingUnMappingLogDB.Add(mapping_Log);


                                DTOTempSession dTOTempSessionResult = new DTOTempSession();
                                dTOTempSessionResult.Status = 5;
                                dTOTempSessionResult.UserId = (int)(trnDomainMapping.UserId != null ? trnDomainMapping.UserId : 0);
                                return dTOTempSessionResult;
                            }
                            else
                            {
                                return null;
                            }

                        }
                        else
                        {
                            return null;
                        }

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(1001, ex, "AccountDB->ProfileAndMappingSaving");
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }

        }
        
        
        /// <summary>
        /// Computes aggregate account statistics: total users, active/inactive counts,
        /// verified/not-verified counts, and mapping stats (mapped/unmapped, IO/CO/RO/ORO).
        /// </summary>
        /// <returns>
        /// A populated <see cref="DTOAccountCountResponse"/> with:
        /// <c>User</c>, <c>ActiveUser</c>, <c>InActiveUser</c>,
        /// <c>VerifiedUser</c>, <c>NotVerifiedUser</c>,
        /// <c>MappedUser</c>, <c>UnMappedUser</c>, <c>IO</c>, <c>CO</c>, <c>RO</c>, <c>ORO</c>.
        /// On exception, returns a new (zeroed) response and logs the error.
        /// </returns>
        /// <remarks>
        /// Executes three aggregated EF Core queries:
        /// 1) Users → active/inactive counts.
        /// 2) Users → verified/not-verified via <c>AdminFlag</c>.
        /// 3) TrnDomainMapping → mapped/unmapped and role-flag counts (IO/CO/RO/ORO).
        /// Uses <c>GroupBy(1)</c> to materialize single rows; logs with event ID 1001 on failure.
        /// </remarks>
        public async Task<DTOAccountCountResponse> AccountCount()
        {
            try
            {
                DTOAccountCountResponse dTOAccountCountResponse = new DTOAccountCountResponse();
                var objActiveUser = await _context.Users.GroupBy(x => 1)
                                    .Select(g => new
                                    {
                                        ActiveUser = g.Count(x => x.Active),
                                        InActiveUser = g.Count(x => !x.Active),
                                    }).FirstOrDefaultAsync();

                var objVerifiedUser = await _context.Users.GroupBy(x => 1)
                                        .Select(g => new
                                        {
                                            VerifiedUser = g.Sum(x => x.AdminFlag ? 1 : 0),
                                            NotVerifiedUser = g.Sum(x => x.AdminFlag ? 0 : 1),
                                        }).FirstOrDefaultAsync();


                var objMappedUser = await _context.TrnDomainMapping.GroupBy(x => 1)
                                    .Select(g => new 
                                    {
                                        MappedUser = g.Count(x => x.UserId != null),
                                        UnMappedUser = g.Count(x => x.UserId == null),
                                        IO  = g.Count(x=>x.IsIO == true),
                                        CO = g.Count(x => x.IsCO == true),
                                        RO = g.Count(x => x.IsRO == true),
                                        ORO = g.Count(x => x.IsORO == true),
                                    }).FirstOrDefaultAsync();



                dTOAccountCountResponse.User = objActiveUser != null ? (objActiveUser.ActiveUser + objActiveUser.InActiveUser) : 0;
                dTOAccountCountResponse.ActiveUser = objActiveUser != null ? objActiveUser.ActiveUser : 0;
                dTOAccountCountResponse.InActiveUser = objActiveUser != null ? objActiveUser.InActiveUser : 0;
                dTOAccountCountResponse.VerifiedUser = objVerifiedUser != null ? objVerifiedUser.VerifiedUser : 0;
                dTOAccountCountResponse.NotVerifiedUser = objVerifiedUser != null ? objVerifiedUser.NotVerifiedUser : 0;
                dTOAccountCountResponse.MappedUser = objMappedUser != null ? objMappedUser.MappedUser : 0;
                dTOAccountCountResponse.UnMappedUser = objMappedUser != null ? objMappedUser.UnMappedUser : 0;
                dTOAccountCountResponse.IO = objMappedUser != null ? objMappedUser.IO : 0;
                dTOAccountCountResponse.CO = objMappedUser != null ? objMappedUser.CO : 0;
                dTOAccountCountResponse.RO = objMappedUser != null ? objMappedUser.RO : 0;
                dTOAccountCountResponse.ORO = objMappedUser != null ? objMappedUser.ORO : 0;

                return dTOAccountCountResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AccountDB->AccountCount");
                return new DTOAccountCountResponse();
            }

        }
        
        
        /// <summary>
        /// Saves a unit along with its mapping information in the database.
        /// </summary>
        /// <param name="dTO">A <see cref="DTOSaveUnitWithMappingRequest"/> object containing details of the unit and mapping to be saved.</param>
        /// <returns>
        /// Returns <c>true</c> if the unit and mapping were successfully saved, <c>null</c> if an error occurred.
        /// </returns>
        /// <remarks>
        /// - Creates a new record in <c>TrnUnregdUser</c> for the user associated with the unit.
        /// - If <c>UnitId</c> is 0, a new <c>MUnit</c> record is created; otherwise, updates existing unit mapping.
        /// - Creates a corresponding <c>MapUnit</c> record to map the unit to various organizational entities (Comd, Corps, Div, Bde, etc.).
        /// - Uses a database transaction to ensure atomicity; commits on success and rolls back on any exception.
        /// - Timestamps are saved in India Standard Time (IST).
        /// - Errors are logged using the application's logger.
        /// </remarks>
        public async Task<bool> SaveUnitWithMapping(DTOSaveUnitWithMappingRequest dTO)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                var trnRegUser = new TrnUnregdUser
                {
                    Name = dTO.Name,
                    ServiceNo = dTO.ServiceNo,
                    Rank = dTO.Rank,
                    DomainId = dTO.DomainId,
                    IsActive = true,
                    UpdatedOn = indiaTime,
                };
                await _context.TrnUnregdUser.AddAsync(trnRegUser);
                await _context.SaveChangesAsync();
                
                MUnit mUnit = new MUnit();
                
                if (dTO.UnitId == 0)
                {
                    mUnit = new MUnit
                    {
                        Sus_no = dTO.Sus_no,
                        Suffix = dTO.Suffix,
                        UnitName = dTO.UnitName,
                        Abbreviation= dTO.Abbreviation,
                        Prefix = dTO.Prefix,
                        IsVerify = false,
                        IsActive = true,
                        Updatedby = null,
                        UpdatedOn = indiaTime,
                        UnregdUserId = trnRegUser.UnregdUserId
                    };

                    await _context.MUnit.AddAsync(mUnit);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    mUnit.UnitId = dTO.UnitId;
                }

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
                    Updatedby = null,
                    UpdatedOn = indiaTime,
                };
                await _context.MapUnit.AddAsync(mapUnit);
                await _context.SaveChangesAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(1001, ex, "AccountDB->SaveUnitWithMapping");
                return false;
            }
        }

        
        /// <summary>
        /// Retrieves all user claims from the database, grouped by claim type, and returns them in a format suitable for DataTables.
        /// </summary>
        /// <remarks>
        /// The method supports server-side pagination, sorting, and filtering for DataTables.
        /// In case of an exception, an empty response is returned and the error is logged.
        /// </remarks>
        /// <param name="request">A <see cref="DTODataTablesRequest"/> object containing paging, sorting, and draw parameters from the client.</param>
        /// <returns>
        /// Returns a <see cref="DTODataTablesResponse{DTOClaimsStoreResponse}"/> containing:
        /// - <c>draw</c>: The draw counter from the client request.
        /// - <c>recordsTotal</c>: Total number of records without filtering.
        /// - <c>recordsFiltered</c>: Total number of records after applying filtering.
        /// - <c>data</c>: The paginated and optionally sorted list of claims grouped by claim type.
        /// </returns>
        public async Task<DTODataTablesResponse<DTOClaimsStoreResponse>> GetAllClaimsOrderBy(DTODataTablesRequest request)
        {
            try
            {
                var queryableData = _context.UserClaims
                                    .GroupBy(uc => uc.ClaimType)
                                    .Select(uc => new DTOClaimsStoreResponse 
                                    { 
                                        ClaimType = uc.FirstOrDefault().ClaimType,
                                        TotalUsers = uc.Count()
                                    }) 
                                    .AsQueryable();

                // Total records without filtering
                var totalRecords = queryableData.Count();

                // Apply sorting
                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    
                    queryableData = request.sortDirection.ToLower() == "asc"
                    ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                    : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                }

                // Total records after filtering
                var filteredRecords = queryableData.Count();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                var responseData = new DTODataTablesResponse<DTOClaimsStoreResponse>
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
                _logger.LogError(1001, ex, "AccountDB->Claims");
                List<DTOClaimsStoreResponse> dTOUserRegnResponses = new List<DTOClaimsStoreResponse>();
                var responseData = new DTODataTablesResponse<DTOClaimsStoreResponse>
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
        /// Retrieves all users associated with a specific claim type and returns them in a format suitable for DataTables.
        /// </summary>
        /// <remarks>
        /// The method performs the following operations:
        /// 1. Joins multiple tables (_ClaimsStore, UserClaims, Users, TrnDomainMapping, UserProfile, MRank, MAppointment, MUnit) to get comprehensive user details.
        /// 2. Filters users based on the specified <see cref="DTODataTablesRequest.Choice"/> claim type.
        /// 3. Applies optional search filtering on the ArmyNo field.
        /// 4. Supports sorting and server-side pagination for DataTables.
        /// 5. Returns a structured <see cref="DTODataTablesResponse{DTOUsersByClaim}"/>.
        /// 
        /// In case of an exception, an empty response is returned and the error is logged.
        /// </remarks>
        /// <param name="request">A <see cref="DTODataTablesRequest"/> object containing draw, search, pagination, and sorting parameters.</param>
        /// <returns>
        /// A <see cref="DTODataTablesResponse{DTOUsersByClaim}"/> object containing:
        /// - <c>draw</c>: The draw counter from the client request.
        /// - <c>recordsTotal</c>: Total number of records without filtering.
        /// - <c>recordsFiltered</c>: Total number of records after applying search filtering.
        /// - <c>data</c>: The paginated list of users associated with the claim type.
        /// </returns>
        public async Task<DTODataTablesResponse<DTOUsersByClaim>> GetAllUsersByClaim(DTODataTablesRequest request)
        {
            try
            {
                var queryableData = (from cs in _context.ClaimsStore
                                     join uc in _context.UserClaims on cs.ClaimValue equals uc.ClaimValue
                                     join us in _context.Users on uc.UserId equals us.Id
                                     join tdm in _context.TrnDomainMapping on us.Id equals tdm.AspNetUsersId into utdm_jointable
                                     from xtdm in utdm_jointable.DefaultIfEmpty()
                                     join up in _context.UserProfile on xtdm.UserId equals up.UserId into xtdmup_jointable
                                     from xup in xtdmup_jointable.DefaultIfEmpty()
                                     join r in _context.MRank on xup.RankId equals r.RankId into xtdmr_jointable
                                     from xr in xtdmr_jointable.DefaultIfEmpty()
                                     join apt in _context.MAppointment on xtdm.ApptId equals apt.ApptId into xtdapt_jointable
                                     from xapt in xtdapt_jointable.DefaultIfEmpty()
                                     join unit in _context.MUnit on xtdm.UnitId equals unit.UnitId into xtdunit_jointable
                                     from xunit in xtdunit_jointable.DefaultIfEmpty()
                                     where cs.ClaimType == request.Choice
                                     select new DTOUsersByClaim
                                     {
                                        DomainId = us.DomainId,
                                        Rank = xr.RankAbbreviation,
                                        ArmyNo = xup != null ? xup.ArmyNo : null,
                                        AppointmentName = xapt.AppointmentName,
                                        Name = xup.Name,
                                        Unit = xunit.UnitName,
                                        RoleNames = (from ur in _context.UserRoles.Where(x => x.UserId == us.Id)
                                                    join r in _context.Roles on ur.RoleId equals r.Id
                                                    select r.Name).ToList()
                                     }).AsQueryable();

                // Total records without filtering
                var totalRecords = queryableData.Count();

                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    queryableData = queryableData.Where(x => x.ArmyNo.ToLower().Contains(searchValue));
                }

                //Apply sorting
                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    
                    queryableData = request.sortDirection.ToLower() == "asc"
                    ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                    : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                }

                // Total records after filtering
                var filteredRecords = queryableData.Count();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                var responseData = new DTODataTablesResponse<DTOUsersByClaim>
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
                _logger.LogError(1001, ex, "AccountDB->UserByClaim");
                List<DTOUsersByClaim> dTOUserRegnResponses = new List<DTOUsersByClaim>();
                var responseData = new DTODataTablesResponse<DTOUsersByClaim>
                {
                     draw = request.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }
    }
    
}
