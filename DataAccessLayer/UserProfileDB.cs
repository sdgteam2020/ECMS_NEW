using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Dapper.SqlMapper;

namespace DataAccessLayer
{
    public class UserProfileDB : GenericRepositoryDL<MUserProfile>, IUserProfileDB
    {
        protected new readonly  ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<UserProfileDB> _logger;
        public UserProfileDB(ApplicationDbContext context, ILogger<UserProfileDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context; 
            _contextDP = contextDP;
            _logger = logger;
        }
        
        
        /// <summary>
        /// Checks if the specified UserId exists in any foreign key child tables.
        /// Returns counts of related records in TrnDomainMapping, MTrnICardHold, TrnPostingOut (To/From), and TrnFwds (To/From).
        /// </summary>
        /// <param name="UserId">The UserId to check for foreign key references.</param>
        /// <returns>
        /// A <see cref="DTOProfileIdCheckInFKTableResponse"/> containing counts of related records.
        /// If an error occurs, returns a new DTOProfileIdCheckInFKTableResponse with default values.
        /// </returns>
        public async Task<DTOProfileIdCheckInFKTableResponse> ProfileIdCheckInFKTable(int UserId)
        {
            try
            {
                string query = "Select  count(distinct tdm.Id) as TotalTDM, count(distinct th.ICardHoldId) as TotalTH, count(distinct tpo_to.Id) as TotalTPO_To, count(distinct tpo_from.Id) as TotalTPO_From, count(distinct tf_from.TrnFwdId) as TotalTFFrom, count(distinct tf_to.TrnFwdId) as TotalTFTo from UserProfile up" +
                                " left join TrnDomainMapping tdm on tdm.UserId = up.UserId " +
                                " left join MTrnICardHold th on th.UserId = up.UserId " +
                                " left join TrnPostingOut tpo_to on tpo_to.ToUserID = up.UserId " +
                                " left join TrnPostingOut tpo_from on tpo_from.FromUserID = up.UserId " +
                                " left join TrnFwds tf_from on tf_from.FromUserId = up.UserId " +
                                " left join TrnFwds tf_to on tf_to.ToUserId = up.UserId " +
                                " where up.UserId =@UserId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOProfileIdCheckInFKTableResponse>(query, new { UserId });
                    return ret.FirstOrDefault()?? new DTOProfileIdCheckInFKTableResponse();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->ProfileIdCheckInFKTable");
                return new DTOProfileIdCheckInFKTableResponse();
            }
        }

        /// <summary>
        /// Deletes a user profile from the system.
        /// </summary>
        /// <param name="mUserProfile">The user profile to be deleted.</param>
        /// <returns>Returns a response indicating whether the deletion was successful.</returns>
        public async Task<DTOProfileManageDeleteResponse> DeleteProfile(MUserProfile mUserProfile)
        {
            DTOProfileManageDeleteResponse response = new DTOProfileManageDeleteResponse();
            try
            {
                var entity = await _context.Set<MUserProfile>().FindAsync(mUserProfile.UserId);
                if (entity == null)
                {
                    response.Result = false;
                    response.Message = "Profile not found.";
                    return response;
                }

                _context.Set<MUserProfile>().Remove(entity);
                await _context.SaveChangesAsync();

                response.Result = true;
                response.Message = "Profile deleted successfully.";
            }
            catch (ReferenceConstraintException ex) when (ex.InnerException != null)
            {
                var innerMessage = ex.InnerException.Message.ToLower();
                response.Result = false;
                response.Message = innerMessage.Contains("reference constraint") ||
                                   innerMessage.Contains("foreign key") ||
                                   innerMessage.Contains("constraint violation")
                                   ? "ProfileId is used in child table."
                                   : ex.Message;

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->DeleteProfile");
                response.Result = false;
                response.Message = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// Checks if a user profile with the given ArmyNo and UserId already exists.
        /// </summary>
        /// <param name="ArmyNo">The Army number to search for.</param>
        /// <param name="UserId">The UserId to exclude from the search.</param>
        /// <returns>Returns true if the ArmyNo exists for another user; otherwise, false.</returns>
        public async Task<bool?> FindByArmyNoWithUserId(string ArmyNo, int UserId)
        {
            try
            {
                var ret = await _context.UserProfile.AnyAsync(p => p.UserId != UserId && p.ArmyNo.ToUpper() == ArmyNo.ToUpper());
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetProfileByUserId");
                return null;
            }

        }


        /// <summary>
        /// Checks if a user profile with the given ArmyNo exists.
        /// </summary>
        /// <param name="ArmyNo">The Army number to search for.</param>
        /// <returns>Returns true if the ArmyNo exists, otherwise false.</returns>
        public async Task<bool?> FindByArmyNo(string ArmyNo)
        {
            try
            {
                var ret = await _context.UserProfile.AnyAsync(x => x.ArmyNo.ToUpper() == ArmyNo.ToUpper());
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetProfileByUserId");
                return null;
            }

        }


        /// <summary>
        /// Updates user profile and domain mapping details within a database transaction.
        /// </summary>
        /// <param name="dTO">Request containing user profile and mapping update data.</param>
        /// <returns>
        /// A generic response indicating whether the update was successful.
        /// </returns>
        public async Task<DTOGenericResponse<string>> UpdateProfileWithMapping(DTOUpdateProfileWithMappingRequest dTO)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    var userUpdate = await _context.TrnDomainMapping.FindAsync(dTO.TDMId);

                    if (userUpdate == null)
                    {
                        response.Message = "DID is not mapped to the mapping table.";
                        response.Value = "";
                        response.Result = false;
                        return response;
                    }
                    else
                    {
                        userUpdate.IsIO = dTO.IsIO;
                        userUpdate.IsCO = dTO.IsCO;
                        userUpdate.IsRO = dTO.IsRO;
                        userUpdate.IsORO = dTO.IsORO;

                        _context.TrnDomainMapping.Update(userUpdate);
                        await _context.SaveChangesAsync();

                        var mUserProfile = await _context.UserProfile.FindAsync(dTO.UserId);
                        if (mUserProfile == null)
                        {
                            response.Message = "Invalid Profile Id.";
                            response.Value = "";
                            response.Result = false;
                            return response;
                        }
                        else
                        {
                            mUserProfile.Name = dTO.Name;
                            mUserProfile.RankId = dTO.RankId;
                            mUserProfile.Thumbprint = dTO.Thumbprint;

                            _context.UserProfile.Update(mUserProfile);
                            await _context.SaveChangesAsync();
                        }
                        transaction.Commit();
                        response.Message = "'User has been Updated";
                        response.Value = "";
                        response.Result = true;
                        return response;

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(1001, ex, "UserProfileDB->UpdateProfileWithMapping");
                    response.Message = "Update failed";
                    response.Value = "";
                    response.Result = false;
                    return response;
                }
            }
        }


        /// <summary>
        /// Retrieves user profiles by ArmyNo and UserId.
        /// </summary>
        /// <param name="ArmyNo">The Army number to search for.</param>
        /// <param name="UserId">The UserId to exclude from the search.</param>
        /// <returns>A list of user profiles that match the ArmyNo.</returns>
        public async Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo, int UserId)
        {
            var ret = await _context.UserProfile.Where(P=>P.ArmyNo.ToUpper().Contains(ArmyNo.ToUpper())).ToListAsync();
            return ret;
        }


        /// <summary>
        /// Retrieves a user's profile by ArmyNo.
        /// </summary>
        /// <param name="ArmyNo">The Army number to search for.</param>
        /// <returns>A DTO containing the user's profile information if found; otherwise, null.</returns>
        public async Task<DTOProfileResponse?> GetUserProfileByArmyNo(string ArmyNo)
        {
            DTOProfileResponse? dTOProfileResponse = new DTOProfileResponse();
            MUserProfile? mUserProfile = await _context.UserProfile.FirstOrDefaultAsync(x => x.ArmyNo == ArmyNo);
            if (mUserProfile != null)
            {
                dTOProfileResponse = await GetProfileByUserId(mUserProfile.UserId);
                return dTOProfileResponse;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Checks if the ArmyNo is already associated with a user profile.
        /// </summary>
        /// <param name="ArmyNo">The Army number to search for.</param>
        /// <param name="AspNetUsersId">The ASP.NET user's Id to compare with the profile.</param>
        /// <returns>A DTO containing status codes and messages depending on the ArmyNo status.</returns>
        public async Task<DTOProfileResponse> CheckArmyNoInUserProfile(string ArmyNo, int AspNetUsersId)
        {
            DTOProfileResponse? dTOProfileResponse = new DTOProfileResponse();
            MUserProfile? mUserProfile = await _context.UserProfile.FirstOrDefaultAsync(x=>x.ArmyNo== ArmyNo);
            if(mUserProfile!=null)
            {
                TrnDomainMapping? trnDomainMapping = await _context.TrnDomainMapping.FirstOrDefaultAsync(x => x.UserId == mUserProfile.UserId);
                if(trnDomainMapping!=null)
                {
                    if(trnDomainMapping.AspNetUsersId != AspNetUsersId)
                    {
                        dTOProfileResponse.StatusCode = 3;
                        dTOProfileResponse.Title = "You are already mapped to DID.";
                        dTOProfileResponse.Message = "Pl handover the charge of already mapped previous DID- V to other person before registering again for Current DID";
                        return dTOProfileResponse;
                    }
                    else
                    {
                        dTOProfileResponse.StatusCode = 4;
                        return dTOProfileResponse;
                    }

                }
                else
                {
                    dTOProfileResponse = await GetProfileByUserId(mUserProfile.UserId);
                    dTOProfileResponse.StatusCode = 2;
                    dTOProfileResponse.Title = "Your Profile details already exist in the Appl database.";
                    dTOProfileResponse.Message = "Pl map myself to presently logged in?";
                    return dTOProfileResponse;
                }

            }
            else
            {
                dTOProfileResponse.StatusCode = 1;
                return dTOProfileResponse;
            }

        }
        
        
        /// <summary>
        /// Retrieves the user profile by the specified <paramref name="UserId"/> including related data 
        /// such as rank, domain mapping, and user details.
        /// </summary>
        /// <param name="UserId">The user ID to retrieve the profile for.</param>
        /// <returns>
        /// A <see cref="DTOProfileResponse"/> containing user profile details such as army number, rank, 
        /// mobile number, domain information, and mapping status. Returns <c>null</c> if no profile is found 
        /// or an error occurs.
        /// </returns>
        /// <remarks>
        /// This method performs a series of left joins to combine the user profile with related tables:
        /// - <c>UserProfile</c> (basic user details)
        /// - <c>MRank</c> (rank details)
        /// - <c>TrnDomainMapping</c> (domain mapping)
        /// - <c>Users</c> (user identity and domain ID)
        /// The query fetches the user's rank, mobile, extension, dialing code, mapping status, and related domain information.
        /// </remarks>
        public async Task<DTOProfileResponse?> GetProfileByUserId(int UserId)
        {
            try
            {
                var ret = await (from up in _context.UserProfile.Where(x=>x.UserId == UserId)
                                 join rank in _context.MRank on up.RankId equals rank.RankId
                                 join map in _context.TrnDomainMapping on up.UserId equals map.UserId into upmap_jointable
                                 from xmap in upmap_jointable.DefaultIfEmpty()
                                 join u in _context.Users on xmap.AspNetUsersId equals u.Id into xmapu_jointable
                                 from xu in xmapu_jointable.DefaultIfEmpty()
                                 select new DTOProfileResponse
                                 {
                                     ArmyNo = up.ArmyNo,
                                     UserId = up.UserId,
                                     Name = up.Name,
                                     IsRO = xmap != null ? xmap.IsRO:null,
                                     RankId = rank.RankId,
                                     RankName = rank.RankName,
                                     RankAbbreviation= rank.RankAbbreviation,
                                     Mapping = xmap!=null? true : false,
                                     DomainId = xu != null ? xu.DomainId : null,
                                     AspNetUsersId = xu != null ? xu.Id : 0
                                 }
                                ).FirstOrDefaultAsync();
                return ret;

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetProfileByUserId");
                return null; 
            }
        }


        /// <summary>
        /// Retrieves user profile details based on the provided Army number or User ID.
        /// </summary>
        /// <param name="ArmyNo">The Army number of the user whose profile is being retrieved.</param>
        /// <param name="UserId">The User ID associated with the profile to retrieve.</param>
        /// <returns>
        /// A <see cref="DTOUserProfileResponse"/> containing the user's profile data, or null if no matching profile is found.
        /// </returns>
        /// <exception cref="Exception">Logs and returns null if an error occurs during the query execution.</exception>
        public async Task<DTOUserProfileResponse?> GetByArmyNo(string ArmyNo, int UserId)
        {
            try
            {
                string query = @"SELECT prof.ArmyNo,prof.UserId,prof.Name,trnd.Id as TDMId,prof.Thumbprint,trnd.IsRO,trnd.IsIO,trnd.IsCO,trnd.IsORO,prof.IsToken,prof.IsWithTokenApply,ran.RankName Rank,ran.RankId,mapu.UnitMapId UnitId,munit.UnitName,users.DomainId,
                                appt.AppointmentName,trnd.MappedDate,usermodify.DomainId MappedBy,roles.Name RoleName from UserProfile prof 
                                inner join MRank ran on prof.RankId = ran.RankId 
                                inner join TrnDomainMapping trnd  on trnd.UserId = prof.UserId 
                                inner join AspNetUserRoles maprole on maprole.UserId=trnd.AspNetUsersId
                                inner join AspNetRoles roles on roles.Id=maprole.RoleId
                                inner join MAppointment appt on appt.ApptId=trnd.ApptId
                                left join MapUnit mapu on mapu.UnitMapId = trnd.UnitId 
                                left join MUnit munit on munit.UnitId = mapu.UnitId 
                                left join AspNetUsers usermodify on usermodify.Id=trnd.MappedBy 
                                left join AspNetUsers users on trnd.AspNetUsersId = users.Id
                                where prof.ArmyNo = @ArmyNo  OR trnd.AspNetUsersId=@UserId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOUserProfileResponse>(query, new { ArmyNo, UserId });
                    return BasicDetailList.FirstOrDefault();

                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetByArmyNo");
                return null; 
            }

            
        }



        /// <summary>
        /// Retrieves all related user, rank, unit, appointment, and domain mapping data for a given Army number.
        /// </summary>
        /// <param name="ArmyNo">The Army number of the user whose data is being queried.</param>
        /// <returns>
        /// Returns a <see cref="DTOAllRelatedDataByArmyNoResponse"/> object containing:
        /// - User personal details (Name, ArmyNo, UserId)
        /// - Rank details (RankId, RankName)
        /// - Domain mapping and unit information (TrnDomainMappingId, UnitId, UnitName, IsIO, IsCO, IsRO, IsORO, DialingCode, Extension)
        /// - Appointment details (ApptId, AppointmentName)
        /// - Domain info and admin messages (DomainId, AdminMsg)
        /// Returns <c>null</c> if no matching record is found or an exception occurs.
        /// </returns>


        /// <summary>
        /// Retrieves all related data for a given ArmyNo.
        /// </summary>
        /// <param name="ArmyNo">The Army Number of the user to retrieve data for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains 
        /// the related data in a DTO response if found, or null if an error occurs.
        /// </returns>
        public async Task<DTOAllRelatedDataByArmyNoResponse?> GetAllRelatedDataByArmyNo(string ArmyNo)
        {
            try
            {
                var ret = await (from up in _context.UserProfile
                                 join rank in _context.MRank on up.RankId equals rank.RankId
                                 join map in _context.TrnDomainMapping on up.UserId equals map.UserId into upmap_jointable
                                 from xmap in upmap_jointable.DefaultIfEmpty()
                                 join mapunit in _context.MapUnit on xmap.UnitId equals mapunit.UnitId into xmapmapunit_jointable
                                 from xmapunit in xmapmapunit_jointable.DefaultIfEmpty()
                                 join munit in _context.MUnit on xmapunit.UnitId equals munit.UnitId into xmapunitmunit_jointable
                                 from xmunit in xmapunitmunit_jointable.DefaultIfEmpty()
                                 join appo in _context.MAppointment on xmap.ApptId equals appo.ApptId into xmapappo_jointable
                                 from xappo in xmapappo_jointable.DefaultIfEmpty()
                                 join u in _context.Users on xmap.AspNetUsersId equals u.Id into xmapu_jointable
                                 from xu in xmapu_jointable.DefaultIfEmpty()
                                 where up.ArmyNo == ArmyNo 
                                 select new DTOAllRelatedDataByArmyNoResponse
                                 {
                                     Name = up.Name,
                                     ArmyNo = up.ArmyNo,
                                     UserId = up.UserId,
                                     RankName = rank.RankName,
                                     RankId = rank.RankId,
                                     TrnDomainMappingId = xmap != null? xmap.Id : 0,
                                     UnitId = xmunit != null ? xmunit.UnitId : 0,
                                     UnitName = xmunit != null ? xmunit.UnitName : null,
                                     IsIO = xmap != null ? xmap.IsIO : false,
                                     IsCO = xmap != null ? xmap.IsCO : false,
                                     IsRO = xmap != null ? xmap.IsRO : false,
                                     IsORO = xmap != null ? xmap.IsORO : false,
                                     ApptId = (short)(xappo != null ? xappo.ApptId : 0),
                                     AppointmentName = xappo != null ? xappo.AppointmentName:"No Appointment" ,
                                     DomainId = xu != null ? xu.DomainId : null,
                                     AdminMsg = xu != null ? xu.AdminMsg : null
                                 }
                         ).Distinct().FirstOrDefaultAsync();
                return ret;

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetAllRelatedDataByArmyNo");
                return null; 
            }

        }


        /// <summary>
        /// Retrieves the top 5 records related to a given ArmyNo.
        /// </summary>
        /// <param name="ArmyNo">The Army Number of the user to retrieve records for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains 
        /// a list of DTO responses for the top 5 records, or null if an error occurs.
        /// </returns>
        public async Task<List<DTOAllRelatedDataByArmyNoResponse>?> GetTopByArmyNo(string ArmyNo)
        {
            try
            {
                var ret = await (from up in _context.UserProfile.Where(x=>x.ArmyNo.Contains(ArmyNo))
                                 join rank in _context.MRank on up.RankId equals rank.RankId
                                 join map in _context.TrnDomainMapping on up.UserId equals map.UserId into upmap_jointable
                                 from xmap in upmap_jointable.DefaultIfEmpty()
                                 join mapunit in _context.MapUnit on xmap.UnitId equals mapunit.UnitId into xmapmapunit_jointable
                                 from xmapunit in xmapmapunit_jointable.DefaultIfEmpty()
                                 join munit in _context.MUnit on xmapunit.UnitId equals munit.UnitId into xmapunitmunit_jointable
                                 from xmunit in xmapunitmunit_jointable.DefaultIfEmpty()
                                 join appo in _context.MAppointment on xmap.ApptId equals appo.ApptId into xmapappo_jointable
                                 from xappo in xmapappo_jointable.DefaultIfEmpty()
                                 join u in _context.Users on xmap.AspNetUsersId equals u.Id into xmapu_jointable
                                 from xu in xmapu_jointable.DefaultIfEmpty()
                                 select new DTOAllRelatedDataByArmyNoResponse
                                 {
                                     Name = up.Name,
                                     ArmyNo = up.ArmyNo,
                                     UserId = up.UserId,
                                     RankName = rank.RankName,
                                     RankId = rank.RankId,
                                     TrnDomainMappingId = xmap != null ? xmap.Id : 0,
                                     UnitId = xmunit != null ? xmunit.UnitId : 0,
                                     UnitName = xmunit != null ? xmunit.UnitName : null,
                                     ApptId = (short)(xappo != null ? xappo.ApptId : 0),
                                     AppointmentName = xappo != null ? xappo.AppointmentName : "No Appointment",
                                     DomainId = xu != null ? xu.DomainId : null
                                 }
                                ).Take(5).ToListAsync();
                return ret;

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetTopByArmyNo");
                return null; 
            }

        }

        /// <summary>
        /// Retrieves all user profiles related to a given DomainId and UserId.
        /// </summary>
        /// <param name="DomainId">The Domain ID associated with the user profile.</param>
        /// <param name="UserId">The User ID of the person requesting the data.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains 
        /// a list of DTO user profile responses, or an empty list if an error occurs.
        /// </returns>
        public async Task<List<DTOUserProfileResponse>> GetAll(int DomainId, int UserId)
        {
            // return _context.UserProfile.Where(P => P.ArmyNo == ArmyNo).SingleOrDefault();
            string query = "select map.id MapId,users.ArmyNo,users.UserId,appo.ApptId,appo.AppointmentName, ran.RankAbbreviation Rank,"+
                            " users.Name,dmap.UnitId,Uni.UnitName,Uni.Sus_no + Uni.Suffix SusNo,dmap.IsRO,dmap.IsIO,dmap.IsCO" +
                            " from UserProfile users "+
                            " inner join TrnDomainMapping dmap on dmap.UserId = users.UserId "+
                            " inner join MUnit Uni on Uni.UnitId = dmap.UnitId "+
                            " inner join MAppointment appo on appo.ApptId = dmap.ApptId "+
                            " inner join MRank ran on ran.RankId = users.RankId "+
                            " left join MMappingProfile map on map.UserId = users.UserId "+
                            " where dmap.AspNetUsersId = @DomainId";

            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOUserProfileResponse>(query, new { DomainId });
                    return BasicDetailList.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetDataForFwd");
                return new List<DTOUserProfileResponse>();
            }
        }


        /// <summary>
        /// Retrieves basic details for a given RequestId.
        /// </summary>
        /// <param name="RequestId">The Request ID to fetch related basic details.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains 
        /// a list of basic detail view models related to the RequestId.
        /// </returns>
        public async Task<List<BasicDetailVM>> GetByRequestId(int RequestId)
        {
            //var BasicDetailList = _context.BasicDetails.Where(x => x.IsDeleted == false && x.Updatedby == UserId).ToList();

            string query = "SELECT B.RegistrationId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,B.PermanentAddress," +
                "C.StepId StepCounter,C.Id StepId,ty.TypeId ICardType,trnicrd.RequestId " +
                " FROM BasicDetails B  inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                " inner join UserProfile pr on pr.UserId = trnicrd.Updatedby " +
                " WHERE trnicrd.RequestId=@RequestId";

            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailVM>(query, new { RequestId });

                    int sno = 1;
                    var allrecord = (from e in BasicDetailList
                                     select new BasicDetailVM()
                                     {
                                         BasicDetailId = e.BasicDetailId,

                                         Sno = sno++,
                                         FName = e.FName,
                                         LName = e.LName,
                                         ServiceNo = e.ServiceNo,
                                         DOB = e.DOB,
                                         DateOfCommissioning = e.DateOfCommissioning,
                                         PermanentAddress = e.PermanentAddress,
                                         StepCounter = e.StepCounter,
                                         StepId = e.StepId,
                                         ICardType = e.ICardType,
                                         //RegistrationId = e.RegistrationId,
                                         RequestId = e.RequestId,
                                     }).ToList();
                    return await Task.FromResult(allrecord);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetDataForFwd");
                return new List<BasicDetailVM>();
            }
        }

        /// <summary>
        /// Retrieves a list of forwarded ICARD responses based on multiple filtering criteria.
        /// </summary>
        /// <param name="StepId">The step ID to filter by.</param>
        /// <param name="UnitId">The unit ID to filter by.</param>
        /// <param name="Name">The name to filter the users by.</param>
        /// <param name="TypeId">The type of search to perform (e.g., by Name, ArmyNo, etc.).</param>
        /// <param name="RO">The Record Office flag.</param>
        /// <param name="ORO">The ORO flag.</param>
        /// <param name="DomainMapId">The domain mapping ID to exclude from the results.</param>
        /// <returns>A list of DTOFwdICardResponse objects that match the search criteria.</returns>
        public async Task<List<DTOFwdICardResponse>> GetDataForFwd(int StepId, int UnitId, string Name, int TypeId,int RO,int ORO, int DomainMapId)
        {
            try
            {
                string query = "";

                if (TypeId == 0)
                {
                    query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
                              " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
                              " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
                              " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
                              " inner join UserProfile usep on usep.UserId=trndomain.UserId" +
                              " inner join MRank ra on ra.RankId=usep.RankId  " +
                              " where trndomain.AspNetUsersId like @Name ";

                }
                else if (TypeId == 1)
                {
                    Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
                              " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
                              " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
                              " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
                              " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                              " inner join MRank ra on ra.RankId=usep.RankId " +
                              " where usep.ArmyNo like @Name and trndomain.AspNetUsersId !=@DomainMapId ";

                }
                else if (TypeId == 2)
                {
                    Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.Name, usep.ArmyNo,ra.RankAbbreviation from TrnDomainMapping trndomain" +
                              " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
                              " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
                              " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
                              " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                              " inner join MRank ra on ra.RankId=usep.RankId " +
                              " where usep.Name like @Name and trndomain.AspNetUsersId !=@DomainMapId ";

                }
                else if (TypeId == 3)
                {
                    Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
                              " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
                              " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
                              " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
                              " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                              " inner join MRank ra on ra.RankId=usep.RankId " +
                              " where users.DomainId like @Name and trndomain.AspNetUsersId !=@DomainMapId";

                }
                using (var connection = _contextDP.CreateConnection())
                {
                var BasicDetailList = await connection.QueryAsync<DTOFwdICardResponse>(query, new { UnitId, Name, DomainMapId });
                return BasicDetailList.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetDataForFwd");
                return new List<DTOFwdICardResponse>();
            }
        }


        /// <summary>
        /// Retrieves a list of forwarded ICARD responses based on office and unit mapping conditions.
        /// </summary>
        /// <param name="UnitId">The unit ID to filter by.</param>
        /// <param name="RO">The Record Office flag (1 for active Record Office users).</param>
        /// <param name="ORO">The ORO flag (1 for active ORO users).</param>
        /// <param name="IsAfsacCell">Indicates if it is an Afsac Cell user.</param>
        /// <param name="BasicDetailsId">The BasicDetailsId to filter by.</param>
        /// <param name="DomainMapId">The domain mapping ID to exclude from the results.</param>
        /// <returns>A list of DTOFwdICardResponse objects that match the search conditions.</returns>
        public async Task<List<DTOFwdICardResponse>> GetOffrsByUnitMapId(int UnitId, int RO, int ORO, int IsAfsacCell, int BasicDetailsId, int DomainMapId)
        {
            string query = "";
            try
            {
                if (RO == 1)
                {
                    query = @"Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain
                            inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id 
                            inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId 
                            inner join UserProfile usep on usep.UserId=trndomain.UserId 
                            inner join MRank ran on ran.RankId=usep.RankId 
                            inner join MRecordOffice rec on trndomain.id=rec.TDMId 
                            inner join TrnICardRequest req on req.RecordOfficeId=rec.RecordOfficeId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                            where  bas.BasicDetailId=@BasicDetailsId;";
                }
                else if (ORO == 1)
                {
                    query = @"Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from OROMapping oromap
                            inner join TrnDomainMapping trndomain on trndomain.Id=oromap.TDMId 
                            inner join AspNetUsers users on users.Id=trndomain.AspNetUsersId 
                            inner join UserProfile usep on usep.UserId=trndomain.UserId 
                            inner join MRank ran on ran.RankId=usep.RankId
                            inner join TrnICardRequest req on req.RecordOfficeId=oromap.RecordOfficeId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                            where bas.BasicDetailId=@BasicDetailsId;";
                }
                else if (IsAfsacCell == 1)
                {
                    query = @"Select top 1 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from AfsacCellMapping acm 
                            inner join TrnDomainMapping trndomain on trndomain.Id =acm.TDMId 
                            inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id 
                            inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId 
                            inner join UserProfile usep on usep.UserId=trndomain.UserId 
                            inner join MRank ran on ran.RankId=usep.RankId;";
                }
                else
                {
                    query = @"Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain
                            inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id
                            inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId
                            inner join UserProfile usep on usep.UserId=trndomain.UserId
                            inner join MRank ran on ran.RankId=usep.RankId
                            where trndomain.UnitId =@UnitId and trndomain.AspNetUsersId !=@DomainMapId order by ran.Orderby;";
                }
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOFwdICardResponse>(query, new { UnitId, RO, BasicDetailsId, DomainMapId });
                    if(BasicDetailList.Count() == 0)
                    {
                        List<DTOFwdICardResponse> dTOFwdICardResponse = new List<DTOFwdICardResponse>();
                        DTOFwdICardResponse dTOFwdICardResponse1 = new DTOFwdICardResponse();

                        dTOFwdICardResponse1.IsError = true;
                        dTOFwdICardResponse1.ErrorMessage = "You can not fwd your request at this time because profile not mapped. Contact ORO (MP6)";

                        dTOFwdICardResponse.Add(dTOFwdICardResponse1);
                        return dTOFwdICardResponse;
                    }
                    else
                    {
                        return BasicDetailList.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetOffrsByUnitMapId");
                List<DTOFwdICardResponse> dTOFwdICardResponse = new List<DTOFwdICardResponse>();
                DTOFwdICardResponse dTOFwdICardResponse1 = new DTOFwdICardResponse();

                dTOFwdICardResponse1.IsError = true;
                dTOFwdICardResponse1.ErrorMessage = "Internal Error.";

                dTOFwdICardResponse.Add(dTOFwdICardResponse1);
                return dTOFwdICardResponse;
            }
        }

        /// <summary>
        /// Retrieves the user profile's token application status based on the user ID.
        /// </summary>
        /// <param name="UserId">The ID of the user to fetch the profile for.</param>
        /// <returns>The MUserProfile object containing token application details.</returns>
        public async Task<MUserProfile> GetByIsWithoutTokenApply(int UserId)
        {
            try { 
                string query = "SELECT prof.IsWithTokenApply,prof.IsToken" +
                                    " from UserProfile prof " +
                                    " inner join MRank ran on prof.RankId = ran.RankId " +
                                    " inner join TrnDomainMapping trnd  on trnd.UserId = prof.UserId " +
                                    " inner join AspNetUserRoles maprole on maprole.UserId=trnd.AspNetUsersId" +
                                    " inner join AspNetRoles roles on roles.Id=maprole.RoleId" +
                                    " inner join MAppointment appt on appt.ApptId=trnd.ApptId" +
                                    " left join MapUnit mapu on mapu.UnitMapId = trnd.UnitId " +
                                    " left join MUnit munit on munit.UnitId = mapu.UnitId " +
                                    " left join AspNetUsers usermodify on usermodify.Id=trnd.MappedBy " +
                                    " left join AspNetUsers users on trnd.AspNetUsersId = users.Id" +
                                    " where trnd.AspNetUsersId=@UserId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<MUserProfile>(query, new { UserId });
                    return BasicDetailList.FirstOrDefault()?? new MUserProfile();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetByIsWithoutTokenApply");
                return new MUserProfile(); 
            }
        }
        public async Task<DTOCheckedBeforeUpdateProfileResponse> CheckedBeforeUpdateProfile(DTOUpdateProfileWithMappingRequest dTO)
        {
            DTOCheckedBeforeUpdateProfileResponse dTOCheckedBeforeUpdate = new DTOCheckedBeforeUpdateProfileResponse();
            try
            {
                string query = @"SELECT tdm.Id AS TDMId,tdm.UserId AS UserId,mr.ApplyForId AS ApplyForId,mr.RankAbbreviation
                                FROM TrnDomainMapping tdm
                                LEFT JOIN MRank mr ON mr.RankId = @RankId
                                WHERE tdm.AspNetUsersId = @Updatedby";
                using var connection = _contextDP.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<DTOCheckedBeforeUpdateProfileResponse>(query,new { dTO.Updatedby, dTO.RankId });

                return result ?? new DTOCheckedBeforeUpdateProfileResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetByIsWithoutTokenApply");
                return dTOCheckedBeforeUpdate;
            }
        }

    }
}
