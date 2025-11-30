using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class DomainMapDB : GenericRepositoryDL<TrnDomainMapping>, IDomainMapDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly ILogger<DomainMapDB> _logger;
        public DomainMapDB(ApplicationDbContext context, ILogger<DomainMapDB> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the domain mapping record associated with a given ASP.NET User ID.
        /// This method performs a query to fetch the `TrnDomainMapping` record that corresponds to the specified `AspNetUsersId`.
        /// </summary>
        /// <param name="AspNetUsersId">The ASP.NET User ID used to find the associated domain mapping record.</param>
        /// <returns>
        /// Returns a `TrnDomainMapping` object if a record is found, otherwise returns `null`.
        /// </returns>
        /// <remarks>
        /// This method uses Entity Framework's LINQ query to filter the `TrnDomainMapping` table based on the provided `AspNetUsersId`.
        /// It returns the first record matching the condition or `null` if no match is found.
        /// </remarks>
        public async Task<TrnDomainMapping?> GetByAspnetUserIdBy(int AspNetUsersId)
        {
               return  await _context.TrnDomainMapping.FirstOrDefaultAsync(p => p.AspNetUsersId == AspNetUsersId);
        }

        
        /// <summary>
        /// Retrieves the domain mapping record associated with a given User ID.
        /// This method performs a query to fetch the `TrnDomainMapping` record that corresponds to the specified `UserId`.
        /// </summary>
        /// <param name="UserId">The User ID used to find the associated domain mapping record.</param>
        /// <returns>
        /// Returns a `TrnDomainMapping` object if a record is found, otherwise returns `null`.
        /// </returns>
        /// <remarks>
        /// This method uses Entity Framework's LINQ query to filter the `TrnDomainMapping` table based on the provided `UserId`.
        /// It returns the first record matching the condition or `null` if no match is found.
        /// </remarks>
        public async Task<TrnDomainMapping?> GetTrnDomainMappingByUserId(int UserId)
        {
            // Perform the query to fetch the TrnDomainMapping record for the given UserId.
            var ret = await _context.TrnDomainMapping.Where(p => p.UserId == UserId).FirstOrDefaultAsync();
            return ret; // Return the found record or null if no record is found
        }


        /// <summary>
        /// Checks if a domain mapping exists for a given `AspNetUsersId` in the `TrnDomainMapping` table.
        /// This method checks whether there is any record in the `TrnDomainMapping` table with the specified `AspNetUsersId`.
        /// </summary>
        /// <param name="Data">The `TrnDomainMapping` object containing the `AspNetUsersId` to be checked.</param>
        /// <returns>
        /// Returns `true` if a matching domain mapping record exists, otherwise `false`.
        /// </returns>
        /// <remarks>
        /// This method performs an asynchronous query to the `TrnDomainMapping` table to check if there is any record
        /// with the provided `AspNetUsersId`. The `AnyAsync` method is used for efficiency, as it stops execution once
        /// a matching record is found.
        /// </remarks>
        public async Task<bool> GetByDomainId(TrnDomainMapping Data)
        {
            // Query the TrnDomainMapping table to check if any record matches the given AspNetUsersId.
            var ret = await _context.TrnDomainMapping.AnyAsync(p => p.AspNetUsersId == Data.AspNetUsersId);
            return ret; // Return true if a matching record exists, otherwise false
        }



        /// <summary>
        /// Retrieves the domain mapping record associated with a given `RequestId` from the `TrnDomainMapping` table.
        /// This method performs a join between the `TrnDomainMapping` and `TrnICardRequest` tables to fetch the associated `AspNetUsersId` and `UserId` based on the provided `RequestId`.
        /// </summary>
        /// <param name="RequestId">The unique identifier of the request for which the domain mapping details are to be retrieved.</param>
        /// <returns>
        /// Returns a `TrnDomainMapping` object containing the `AspNetUsersId` and `UserId` if a matching record is found, otherwise returns `null`.
        /// </returns>
        /// <remarks>
        /// This method uses LINQ to perform an inner join between `TrnDomainMapping` and `TrnICardRequest` tables, and filters by `RequestId`.
        /// If no record is found, it returns `null`.
        /// </remarks>
        public async Task<TrnDomainMapping?> GetByRequestId(int RequestId)
        {
            // LINQ query to join TrnDomainMapping and TrnICardRequest tables by TrnDomainMappingId and filter by RequestId
            var ret = await (from trndomap in _context.TrnDomainMapping
                      join trnicardreq in _context.TrnICardRequest on trndomap.Id equals trnicardreq.TrnDomainMappingId
                      where trnicardreq.RequestId == RequestId
                      select new TrnDomainMapping
                      {
                        AspNetUsersId=trndomap.AspNetUsersId,
                        UserId= trndomap.UserId,
                      }).FirstOrDefaultAsync();
            return  ret; // Return the result (TrnDomainMapping or null)
        }

        
        /// <summary>
        /// Retrieves the first domain mapping record along with related user and profile data for a given DomainId and Role.
        /// </summary>
        /// <param name="DomainId">The unique identifier of the domain to query.</param>
        /// <param name="Role">The role name to filter associated user roles.</param>
        /// <returns>
        /// Returns a <see cref="TrnDomainMapping"/> object with related <see cref="ApplicationUser"/>, 
        /// <see cref="MUserProfile"/>, role, and rank information. Returns <c>null</c> if no matching record is found or an error occurs.
        /// </returns>
        public async Task<TrnDomainMapping?> GetAllRelatedDataByDomainId(string DomainId,string Role)
        {
            try
            {
                var result = await (from au in _context.Users.Where(x => x.DomainId == DomainId)
                                    join tdm in _context.TrnDomainMapping on au.Id equals tdm.AspNetUsersId into autdm_jointable
                                    from xtdm in autdm_jointable.DefaultIfEmpty()
                                    join up in _context.UserProfile on xtdm.UserId equals up.UserId into tdmup_jointable
                                    from xup in tdmup_jointable.DefaultIfEmpty()
                                    select new TrnDomainMapping
                                    {
                                        Id = xtdm != null ? xtdm.Id : 0,
                                        UnitId = xtdm != null ? xtdm.UnitId : 0,
                                        MapUnit = xtdm != null ? xtdm.MapUnit : null,
                                        ApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                        IsIO = xtdm != null ? xtdm.IsIO : false,
                                        IsToken = xup != null ? xup.IsToken : false,
                                        IsCO = xtdm != null ? xtdm.IsCO : false,
                                        IsRO = xtdm != null ? xtdm.IsRO : false,
                                        IsORO = xtdm != null ? xtdm.IsORO : false,
                                        AspNetUsersId = au != null ? au.Id : 0,
                                        UserId = xup != null ? xup.UserId : null,
                                        ApplicationUser = au != null ? au : null,
                                        MUserProfile = xup != null ? xup : null,
                                        Role = (from ur in _context.UserRoles.Where(x => x.UserId == au.Id)
                                                join r in _context.Roles on ur.RoleId equals r.Id
                                                where  r.Name.ToUpper() == Role.ToUpper()
                                                select r).FirstOrDefault(),
                                        Rank = xup == null ? null :(from rank in _context.MRank.Where(x=>x.RankId == xup.RankId)
                                                                          select rank).FirstOrDefault(),
                                    }).FirstOrDefaultAsync();
                return (TrnDomainMapping?)result;
            }
            catch(Exception ex)
            {
                _logger.LogInformation(1001, ex, "GetAllRelatedDataByDomainId");
                return null;
            }

        }

        /// <summary>
        /// Retrieves the profile data associated with an ASP.NET user by their `AspNetUserId` from the `TrnDomainMapping` and related tables.
        /// This method performs a left join on the `TrnDomainMapping` table and `UserProfile` table to retrieve the user's profile details and domain mapping data.
        /// </summary>
        /// <param name="Id">The ASP.NET user ID for which the profile data and domain mapping are to be retrieved.</param>
        /// <returns>
        /// Returns a `TrnDomainMapping` object containing the user's domain mapping and profile details if a match is found.
        /// If no record is found, returns `null`.
        /// </returns>
        /// <remarks>
        /// This method uses LINQ with left joins to combine data from the `Users`, `TrnDomainMapping`, and `UserProfile` tables.
        /// It retrieves the domain mapping information (e.g., `UserId`, `UnitId`) and related profile information.
        /// </remarks>
        public async Task<TrnDomainMapping?> GetProfileDataByAspNetUserId(int Id)
        {
            try
            {
                // LINQ query to join Users, TrnDomainMapping, and UserProfile based on the given ASP.NET user ID
                var result = await (from au in _context.Users
                                    join tdm in _context.TrnDomainMapping on au.Id equals tdm.AspNetUsersId into autdm_jointable
                                    from xtdm in autdm_jointable.DefaultIfEmpty()
                                    join up in _context.UserProfile on xtdm.UserId equals up.UserId into tdmup_jointable
                                    from xup in tdmup_jointable.DefaultIfEmpty()
                                    where au.Id == Id
                                    select new TrnDomainMapping
                                    {
                                        Id = xtdm != null ? xtdm.Id : 0,
                                        AspNetUsersId = au != null ? au.Id : 0,
                                        UserId = xtdm != null ? xtdm.UserId : 0,
                                        UnitId = xtdm != null ? xtdm.UnitId : 0,
                                        ApplicationUser = au != null ? au : null,
                                        MUserProfile = xup != null ? xup : null,
                                    }).FirstOrDefaultAsync();
                return (TrnDomainMapping?)result; // Return the domain mapping and profile data
            }
            catch (Exception ex)
            {
                _logger.LogInformation(1001, ex, "GetProfileDataByAspNetUserId");
                return null;
            }

        }
    }
}
