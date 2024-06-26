﻿using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using DataTransferObject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DataAccessLayer
{
    public class UserProfileDB : GenericRepositoryDL<MUserProfile>, IUserProfileDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<UserProfileDB> _logger;
        public UserProfileDB(ApplicationDbContext context, ILogger<UserProfileDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context; 
            _contextDP = contextDP;
            _logger = logger;
        }
        private readonly IConfiguration configuration;
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
        public async Task<bool?> UpdateProfileWithMapping(DTOUpdateProfileWithMappingRequest dTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    var userUpdate = await _context.TrnDomainMapping.FindAsync(dTO.TDMId);

                    if (userUpdate == null)
                    {
                        return false;
                    }
                    else
                    {
                        userUpdate.Extension = dTO.Extension;
                        userUpdate.DialingCode = dTO.DialingCode;
                        userUpdate.IsIO = dTO.IsIO;
                        userUpdate.IsCO = dTO.IsCO;
                        userUpdate.IsRO = dTO.IsRO;
                        userUpdate.IsORO = dTO.IsORO;

                        _context.TrnDomainMapping.Update(userUpdate);
                        await _context.SaveChangesAsync();

                        var mUserProfile = await _context.UserProfile.FindAsync(dTO.UserId);
                        if (mUserProfile == null)
                        {
                            return false;
                        }
                        else
                        {
                            mUserProfile.UserId = dTO.UserId;
                            mUserProfile.Name = dTO.Name;
                            mUserProfile.RankId = dTO.RankId;
                            mUserProfile.MobileNo = dTO.MobileNo;
                            mUserProfile.IsToken = dTO.IsToken;
                            mUserProfile.Thumbprint = dTO.Thumbprint;

                            _context.UserProfile.Update(mUserProfile);
                            await _context.SaveChangesAsync();
                        }
                        transaction.Commit();
                        return true;

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(1001, ex, "UserProfileDB->UpdateProfileWithMapping");
                    return null;
                }
            }
        }
        public async Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo, int UserId)
        {
            var ret = _context.UserProfile.Where(P=>P.ArmyNo.ToUpper().Contains(ArmyNo.ToUpper())).ToList();
            return ret;
        }
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
                                     MobileNo = up.MobileNo,
                                     DialingCode= xmap != null ? xmap.DialingCode:null,
                                     Extension= xmap != null ? xmap.Extension:null,
                                     IsRO = xmap != null ? xmap.IsRO:null,
                                     RankId = rank.RankId,
                                     RankName = rank.RankName,
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
        public async Task<DTOUserProfileResponse?> GetByArmyNo(string ArmyNo, int UserId)
        {
            try
            {
                //var ret = await (from up in _context.UserProfile
                //                 join rank in _context.MRank on up.RankId equals rank.RankId
                //                 join map in _context.TrnDomainMapping on up.UserId equals map.UserId into upmap_jointable
                //                 from xmap in upmap_jointable.DefaultIfEmpty()
                //                 join mapunit in _context.MapUnit on xmap.UnitId equals mapunit.UnitId into xmapmapunit_jointable
                //                 from xmapunit in xmapmapunit_jointable.DefaultIfEmpty()
                //                 join munit in _context.MUnit on xmapunit.UnitId equals munit.UnitId into xmapunitmunit_jointable
                //                 from xmunit in xmapunitmunit_jointable.DefaultIfEmpty()
                //                 where up.ArmyNo == ArmyNo //&&  user.Updatedby == UserId
                //                 select new DTOUserProfileResponse
                //                 {

                //                     ArmyNo = up.ArmyNo,
                //                     UserId = up.UserId,
                //                     Name = up.Name,
                //                     IntOffr = up.IntOffr,
                //                     IsIO = up.IsIO,
                //                     IsCO = up.IsCO,
                //                     Rank = rank.RankName,
                //                     RankId = rank.RankId,
                //                     UnitId = xmunit!=null?xmunit.UnitId:0,
                //                     UnitName = xmunit!=null ? xmunit.UnitName:"No Unit",

                //                 }
                //                ).Distinct().FirstOrDefaultAsync();

               
                string query = "SELECT prof.ArmyNo,prof.UserId,prof.Name,prof.MobileNo,trnd.Id as TDMId,trnd.DialingCode,trnd.Extension,prof.Thumbprint,trnd.IsRO,trnd.IsIO,trnd.IsCO,trnd.IsORO,prof.IsToken,ran.RankName Rank,ran.RankId,mapu.UnitMapId UnitId,munit.UnitName,users.DomainId," +
                                " appt.AppointmentName,trnd.MappedDate,usermodify.DomainId MappedBy,roles.Name RoleName from UserProfile prof "+
                                " inner join MRank ran on prof.RankId = ran.RankId "+
                                " inner join TrnDomainMapping trnd  on trnd.UserId = prof.UserId "+
                                " inner join AspNetUserRoles maprole on maprole.UserId=trnd.AspNetUsersId"+
                                " inner join AspNetRoles roles on roles.Id=maprole.RoleId"+
                                " inner join MAppointment appt on appt.ApptId=trnd.ApptId"+
                                " left join MapUnit mapu on mapu.UnitMapId = trnd.UnitId "+
                                " left join MUnit munit on munit.UnitId = mapu.UnitId "+
                                " left join AspNetUsers usermodify on usermodify.Id=trnd.MappedBy "+
                                " left join AspNetUsers users on trnd.AspNetUsersId = users.Id"+
                                " where prof.ArmyNo = @ArmyNo  OR trnd.AspNetUsersId=@UserId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOUserProfileResponse>(query, new { ArmyNo, UserId });
                    int sno = 1;

                    return BasicDetailList.FirstOrDefault();

                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetByArmyNo");
                return null; 
            }

            
        }
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
                                     DialingCode = xmap != null ? xmap.DialingCode : "",
                                     Extension = xmap != null ? xmap.Extension : "",
                                     ApptId = (short)(xappo != null ? xappo.ApptId : 0),
                                     AppointmentName = xappo != null ? xappo.AppointmentName:"No Appointment" ,
                                     DomainId = xu != null ? xu.DomainId : null,
                                     AdminMsg = xu != null ? xu.AdminMsg : null
                                 }
                         ).Distinct().FirstOrDefaultAsync();
                return ret;

            }
            catch (Exception ex) { return null; }

        }
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
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailList = await connection.QueryAsync<DTOUserProfileResponse>(query, new { DomainId });
                int sno = 1;
                
                return BasicDetailList.ToList();

            }
            //var ret = (from user in _context.UserProfile

            //           join Uni in _context.MUnit on user.UnitId equals Uni.UnitId
            //           join app in _context.MAppointment on user.ApptId equals app.ApptId
            //           join forma in _context.MFormation on app.FormationId equals forma.FormationId
            //           join rank in _context.MRank on user.RankId equals rank.RankId
            //           join map in _context.MMappingProfile on user.UserId equals map.UserId
            //           join userio in _context.UserProfile on map.IOId equals userio.UserId
            //           into io from xio in io.DefaultIfEmpty()
            //           join UniO in _context.MUnit on xio.UnitId equals UniO.UnitId into Uio from xUio in Uio.DefaultIfEmpty()
            //           join rankIO in _context.MRank on xio.RankId equals rankIO.RankId into Rio from xRio in Rio.DefaultIfEmpty()

            //           join usergso in _context.UserProfile on map.GSOId equals usergso.UserId
            //            into gso
            //           from xgso in gso.DefaultIfEmpty()

            //           join UnGSO in _context.MUnit on xgso.UnitId equals UnGSO.UnitId into Ugso
            //           from xUgso in Ugso.DefaultIfEmpty()
            //           join rankGSO in _context.MRank on xgso.RankId equals rankGSO.RankId into Rgso
            //           from xRgso in Rgso.DefaultIfEmpty()
            //           where user.Updatedby == UserId
            //           select new DTOUserProfileResponse
            //           {
            //               MapId=map.Id,
            //               ArmyNo = user.ArmyNo,
            //               UserId = user.UserId,
            //               FormationId=forma.FormationId,
            //               FormationName = forma.FormationName,
            //               ApptId=app.ApptId,
            //               AppointmentName = app.AppointmentName,
            //               Rank = rank.RankName,
            //               Name = user.Name,
            //               UnitId = user.UnitId,
            //               UnitName = Uni.UnitName,
            //               SusNo = Uni.Sus_no+Uni.Suffix,
            //               IntOffr=user.IntOffr,


            //               IOArmyNo = xio.ArmyNo,
            //               IOName= xRio.RankName+" "+ xio.Name,
            //               IOUserId = xio.UserId,
            //               UnitIdIo = xUio.UnitId,
            //               UnitIo= xUio.UnitName,
            //               IOSusNo = xUio.Sus_no + xUio.Suffix,

            //               GSOArmyNo = xgso.ArmyNo,
            //               GSOName = xRgso.RankName+" "+ xgso.Name,
            //               GSOUserId = xgso.UserId,
            //               UnitIdGSO = xUgso.UnitId,
            //               UnitGSO = xUgso.UnitName,
            //               GSOSusNo= xUgso.Sus_no+ xUgso.Suffix
            //           }
            //         ).Distinct().ToList();




            //return Task.FromResult(ret);
        }
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
        public async Task<List<DTOFwdICardResponse>> GetDataForFwd(int StepId, int UnitId, string Name, int TypeId,int IsIO, int IsCO, int RO,int ORO, int DomainMapId)
        {
            #region old code write by Kapoor Sir
            //try
            //{

            //    string query = "";
            //    if (TypeId == 0)
            //    {
            //        query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //  " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //  " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //  " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //  " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //  " inner join MRank ra on ra.RankId=usep.RankId and trndomain.AspNetUsersId !=@DomainMapId " +
            //  "  where trndomain.AspNetUsersId like @Name";

            //    }
            //    if (IsIO == 1)
            //    {

            //        if (TypeId == 1)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      " where usep.ArmyNo like @Name";

            //        }
            //        else if (TypeId == 2)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.Name, usep.ArmyNo,ra.RankAbbreviation from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsIO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where usep.Name like @Name";

            //        }
            //        else if (TypeId == 3)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsIO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where users.DomainId like @Name";

            //        }
            //    }
            //    else if (RO == 1)
            //    {
            //        if (TypeId == 1)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where usep.IsRO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where trndomain.IsRO=1 and usep.ArmyNo like @Name";

            //        }
            //        else if (TypeId == 2)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.Name, usep.ArmyNo,ra.RankAbbreviation from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsRO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where trndomain.IsRO=1 and usep.Name like @Name";

            //        }
            //        else if (TypeId == 3)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsRO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where trndomain.IsRO=1 and users.DomainId like @Name";

            //        }
            //    }
            //    else if (ORO == 1)
            //    {
            //        if (TypeId == 1)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsORO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And" +
            //      " where trndomain.IsORO=1 and usep.ArmyNo like @Name";

            //        }
            //        else if (TypeId == 2)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.Name, usep.ArmyNo,ra.RankAbbreviation from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsORO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where trndomain.IsORO=1 and usep.Name like @Name";

            //        }
            //        else if (TypeId == 3)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsORO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where trndomain.IsORO=1 and users.DomainId like @Name";

            //        }
            //    }
            //    else if (IsCO == 1)
            //    {
            //        if (TypeId == 1)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsCO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where usep.ArmyNo like @Name";

            //        }
            //        else if (TypeId == 2)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.Name, usep.ArmyNo,ra.RankAbbreviation from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsCO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where usep.Name like @Name";

            //        }
            //        else if (TypeId == 3)
            //        {
            //            Name = "%" + Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
            //            query = "Select Top 5 trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,mapp.AppointmentName,usep.ArmyNo,ra.RankAbbreviation,usep.Name from TrnDomainMapping trndomain" +
            //      " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
            //      " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
            //      " inner join MAppointment mapp on mapp.ApptId=trndomain.ApptId" +
            //      " left join UserProfile usep on usep.UserId=trndomain.UserId" +
            //      " inner join MRank ra on ra.RankId=usep.RankId " +
            //      //" where trndomain.IsCO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId)) And " +
            //      " where users.DomainId like @Name";

            //        }
            //    }
            //    using (var connection = _contextDP.CreateConnection())
            //    {
            //        var BasicDetailList = await connection.QueryAsync<DTOFwdICardResponse>(query, new { UnitId, Name, DomainMapId });
            //        int sno = 1;
            //        //var allrecord = (from e in BasicDetailList
            //        //                 select new DTOBasicDetailRequest()
            //        //                 {
            //        //                     BasicDetailId = e.BasicDetailId,
            //        //                     EncryptedId = protector.Protect(e.BasicDetailId.ToString()),
            //        //                     Sno = sno++,
            //        //                     Name = e.Name,
            //        //                     ServiceNo = e.ServiceNo,
            //        //                     DOB = e.DOB,
            //        //                     DateOfCommissioning = e.DateOfCommissioning,
            //        //                     PermanentAddress = e.PermanentAddress,
            //        //                     StepCounter = e.StepCounter,
            //        //                     StepId = e.StepId,
            //        //                     ICardType = e.ICardType,
            //        //                     RegistrationType = e.RegistrationType,
            //        //                 }).ToList();
            //        return BasicDetailList.ToList();

            //    }

            //}
            //catch (Exception ex)
            //{

            //}
            //// return _context.UserProfile.Where(P => P.ArmyNo == ArmyNo).SingleOrDefault();
            ////var ret = (from user in _context.UserProfile
            ////           join basicd in _context.BasicDetails on user.ArmyNo equals basicd.ServiceNo
            ////           join icardreq in _context.TrnICardRequest on basicd.BasicDetailId equals icardreq.BasicDetailId
            ////           join Uni in _context.MUnit on user.UnitId equals Uni.UnitId
            ////           join app in _context.MAppointment on user.ApptId equals app.ApptId
            ////           join forma in _context.MFormation on app.FormationId equals forma.FormationId
            ////           join rank in _context.MRank on user.RankId equals rank.RankId
            ////           join map in _context.MMappingProfile on user.UserId equals map.UserId
            ////           join userio in _context.UserProfile on map.IOId equals userio.UserId
            ////           join UniO in _context.MUnit on userio.UnitId equals UniO.UnitId
            ////           join rankIO in _context.MRank on userio.RankId equals rankIO.RankId
            ////           join usergso in _context.UserProfile on map.GSOId equals usergso.UserId
            ////           join UnGSO in _context.MUnit on userio.UnitId equals UnGSO.UnitId
            ////           join rankGSO in _context.MRank on usergso.RankId equals rankGSO.RankId
            ////             where user.ArmyNo == ArmyNo //&&  user.Updatedby == UserId
            ////           select new DTOUserProfileResponse
            ////           {
            ////               MapId = map.Id,
            ////               ArmyNo = user.ArmyNo, 
            ////               UserId = user.UserId,
            ////               FormationId = forma.FormationId,
            ////               FormationName = forma.FormationName,
            ////               ApptId = app.ApptId,
            ////               AppointmentName = app.AppointmentName,
            ////               Rank = rank.RankName,
            ////               Name = user.Name,
            ////               UnitId = user.UnitId,
            ////               UnitName = Uni.UnitName,
            ////               SusNo = Uni.Sus_no + Uni.Suffix,
            ////               IntOffr = user.IntOffr,
            ////               RequestId= icardreq.RequestId,

            ////               IOArmyNo = userio.ArmyNo,
            ////               IOName = rankIO.RankAbbreviation + " " + userio.Name,
            ////               IOUserId = userio.UserId,
            ////               UnitIdIo = UniO.UnitId,
            ////               UnitIo = UniO.UnitName,
            ////               IOSusNo = UniO.Sus_no + UniO.Suffix,

            ////               GSOArmyNo = usergso.ArmyNo,
            ////               GSOName = rankGSO.RankAbbreviation + " " + usergso.Name,
            ////               GSOUserId = usergso.UserId,
            ////               UnitIdGSO = UnGSO.UnitId,
            ////               UnitGSO = UnGSO.UnitName,
            ////               GSOSusNo = UnGSO.Sus_no + UnGSO.Suffix


            ////           }
            ////         ).Distinct().SingleOrDefault();




            //return null;
            #endregion end old code write by Kapoor Sir
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

            }
            return null;
        }
        
        public async Task<List<DTOFwdICardResponse>> GetOffrsByUnitMapId(int UnitId, int ISIO, int ISCO, int RO,int ORO, int BasicDetailsId,int DomainMapId)
        {
            #region old code write by Kapoor Sir
         //   string query = "";
         //   if (ISIO == 1)
         //   {
         //       query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain" +
         //     " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
         //     " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
         //     " inner join UserProfile usep on usep.UserId=trndomain.UserId" +
         //     " inner join MRank ran on ran.RankId=usep.RankId" +
         //     " where trndomain.UnitId =@UnitId and trndomain.AspNetUsersId !=@DomainMapId order by ran.Orderby";
         //       //trndomain.IsIO=@ISIO
         //   }
         //   else if (ISCO == 1)
         //   {
         //       query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain" +
         //    " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
         //    " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
         //    " inner join UserProfile usep on usep.UserId=trndomain.UserId" +
         //    " inner join MRank ran on ran.RankId=usep.RankId" +
         //    " where trndomain.UnitId =@UnitId and trndomain.AspNetUsersId !=@DomainMapId order by ran.Orderby";
         //       //and trndomain.IsCO=@ISCO
         //   }
         //   else if (RO == 1)
         //   {
         //       //   query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain" +
         //       //" inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
         //       //" inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
         //       //" inner join UserProfile usep on usep.UserId=trndomain.UserId" +
         //       //" inner join MRank ran on ran.RankId=usep.RankId" +
         //       //" where usep.IsRO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId))";
         //       query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain" +
         //               " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id " +
         //               " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId " +
         //               " inner join UserProfile usep on usep.UserId=trndomain.UserId " +
         //               " inner join MRank ran on ran.RankId=usep.RankId " +
         //               " inner join MRecordOffice rec on trndomain.id=rec.TDMId " +
         //               " inner join BasicDetails bas on bas.ArmedId=rec.ArmedId" +
         //               " where  bas.BasicDetailId=@BasicDetailsId";///usep.IsRO=1 and
         //   }
         //   else if (ORO == 1)
         //   {
         //       query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain" +
         //               " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id " +
         //               " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId " +
         //               " inner join UserProfile usep on usep.UserId=trndomain.UserId " +
         //               " inner join MRank ran on ran.RankId=usep.RankId " +
         //               " inner join MRecordOffice rec on trndomain.id=rec.TDMId and rec.ArmedId=56";
         //       //" inner join BasicDetails bas on bas.ArmedId=rec.ArmedId"+
         //       //" where  bas.BasicDetailId=@BasicDetailsId";
         //       //" where usep.IsORO=1";
         //       //   query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain" +
         //       //" inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
         //       //" inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
         //       //" inner join UserProfile usep on usep.UserId=trndomain.UserId" +
         //       //" inner join MRank ran on ran.RankId=usep.RankId" +
         //       //" where usep.IsORO=1 and trndomain.UnitId in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId=@UnitId))";
         //   }
         //   else
         //   {
         //       query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain" +
         //" inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
         //" inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
         //" inner join UserProfile usep on usep.UserId=trndomain.UserId" +
         //" inner join MRank ran on ran.RankId=usep.RankId" +
         //" where trndomain.UnitId =@UnitId";

         //       // in (Select UnitMapId from MapUnit where ComdId in (Select ComdId from MapUnit where UnitMapId = @UnitId))
         //   }
         //   using (var connection = _contextDP.CreateConnection())
         //   {
         //       var BasicDetailList = await connection.QueryAsync<DTOFwdICardResponse>(query, new { UnitId, ISIO, ISCO, RO, BasicDetailsId, DomainMapId });

         //       return BasicDetailList.ToList();
         //   }
            #endregion end old code write by Kapoor Sir
            string query = "";
            string subquery = "";
            string finalquery = "";
            if (RO == 1)
            {
                query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain"+ 
                        " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id "+
                        " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId "+
                        " inner join UserProfile usep on usep.UserId=trndomain.UserId "+
                        " inner join MRank ran on ran.RankId=usep.RankId "+
                        " inner join MRecordOffice rec on trndomain.id=rec.TDMId "+
                        " inner join BasicDetails bas on bas.ArmedId=rec.ArmedId"+
                        " where  bas.BasicDetailId=@BasicDetailsId";///usep.IsRO=1 and
            }
            else if (ORO == 1)
            {
                subquery = "Select bd.ServiceNo,bd.ArmedId,ran.Orderby from BasicDetails bd" +
                            " inner join MRank ran on ran.RankId=bd.RankId " +
                            " where bd.BasicDetailId =@BasicDetailsId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var subqueryResult = await connection.QuerySingleOrDefaultAsync<DTOFwdSubqueryResponse>(subquery, new { BasicDetailsId });

                    if(subqueryResult!=null)    
                    {
                        string ini = subqueryResult.ServiceNo.Substring(0, 2).ToUpper();
                        string MP6F = "MP 6F";
                        if (ini == "SL")
                        {
                            finalquery = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from MRecordOffice mrec" +
                                         " inner join OROMapping oromap on oromap.RecordOfficeId=mrec.RecordOfficeId " +
                                         " inner join TrnDomainMapping trndomain on trndomain.Id=oromap.TDMId " +
                                         " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id " +
                                         " inner join UserProfile usep on usep.UserId=trndomain.UserId " +
                                         " inner join MRank ran on ran.RankId=usep.RankId " +
                                         " where mrec.Name = @MP6F";
                            var final = await connection.QueryAsync<DTOFwdICardResponse>(finalquery, new { MP6F });
                            
                            if(final.Count()==0)
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
                                return final.ToList();
                            }
                            
                        }
                        else if(subqueryResult.Orderby <=4)
                        {
                            finalquery = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from OROMapping oromap" +
                                         " inner join TrnDomainMapping trndomain on trndomain.Id=oromap.TDMId " +
                                         " inner join AspNetUsers users on users.Id=trndomain.AspNetUsersId " +
                                         " inner join UserProfile usep on usep.UserId=trndomain.UserId " +
                                         " inner join MRank ran on ran.RankId=usep.RankId "+
                                         " where oromap.RankId is not null";
                            var final = await connection.QueryAsync<DTOFwdICardResponse>(finalquery);
                            
                            if (final.Count() == 0)
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
                                return final.ToList();
                            }
                        }
                        else
                        {
                            byte ArmedId;
                            ArmedId = subqueryResult.ArmedId;
                            finalquery = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from OROMapping oromap" +
                                         " inner join TrnDomainMapping trndomain on trndomain.Id=oromap.TDMId " +
                                         " inner join AspNetUsers users on users.Id=trndomain.AspNetUsersId " +
                                         " inner join UserProfile usep on usep.UserId=trndomain.UserId " +
                                         " inner join MRank ran on ran.RankId=usep.RankId " +
                                         " where @ArmedId in (select value from string_split(oromap.ArmedIdList,','))";
                            var final = await connection.QueryAsync<DTOFwdICardResponse>(finalquery,new { ArmedId });
                            
                            if (final.Count() == 0)
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
                                return final.ToList();
                            }

                        }
                    }
                    else
                    {

                    }

                }
            }
            else
            {
                query = "Select trndomain.AspNetUsersId,ISNULL(usep.UserId,0) UserId,users.DomainId,usep.ArmyNo,usep.Name,ran.RankAbbreviation from TrnDomainMapping trndomain" +
                         " inner join AspNetUsers users on trndomain.AspNetUsersId=users.Id" +
                         " inner join MapUnit mapu on mapu.UnitMapId=trndomain.UnitId" +
                         " inner join UserProfile usep on usep.UserId=trndomain.UserId" +
                         " inner join MRank ran on ran.RankId=usep.RankId" +
                         " where trndomain.UnitId =@UnitId and trndomain.AspNetUsersId !=@DomainMapId order by ran.Orderby";
            }
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailList = await connection.QueryAsync<DTOFwdICardResponse>(query, new { UnitId, ISIO, ISCO, RO,BasicDetailsId, DomainMapId });

                return BasicDetailList.ToList();
            }
        }

        public async Task<MUserProfile> GetByIsWithoutTokenApply(int UserId)
        {
            try { 
            string query = "SELECT prof.IsWithoutTokenApply" +
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
                int sno = 1;

                return BasicDetailList.FirstOrDefault();

            }

        }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfileDB->GetByIsWithoutTokenApply");
                return null; 
            }
}
    }
}
