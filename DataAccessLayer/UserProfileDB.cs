using Azure.Core;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class UserProfileDB : GenericRepositoryDL<MUserProfile>, IUserProfileDB
    {
        protected readonly ApplicationDbContext _context;
        public UserProfileDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;
        public async Task<bool> GetByArmyNo(MUserProfile Data, int UserId)
        { 
            var ret = _context.UserProfile.Any(p => p.ArmyNo.ToUpper() == Data.ArmyNo.ToUpper() && p.UserId!=Data.UserId);
            return ret;
        }
        public async Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo, int UserId)
        {
            var ret = _context.UserProfile.Where(P=>P.ArmyNo.ToUpper().Contains(ArmyNo.ToUpper())).ToList();
            return ret;
        }
        public async Task<DTOUserProfileResponse> GetByArmyNo(string ArmyNo, int UserId)
        {
            // return _context.UserProfile.Where(P => P.ArmyNo == ArmyNo).SingleOrDefault();
            try
            {
                var ret = await (from user in _context.UserProfile

                                 join Uni in _context.MUnit on user.UnitId equals Uni.UnitId
                                 join app in _context.MAppointment on user.ApptId equals app.ApptId
                                 join forma in _context.MFormation on app.FormationId equals forma.FormationId
                                 join rank in _context.MRank on user.RankId equals rank.RankId

                                 where user.ArmyNo == ArmyNo //&&  user.Updatedby == UserId
                                 select new DTOUserProfileResponse
                                 {

                                     ArmyNo = user.ArmyNo,
                                     UserId = user.UserId,
                                     FormationId = forma.FormationId,
                                     FormationName = forma.FormationName,
                                     ApptId = app.ApptId,
                                     AppointmentName = app.AppointmentName,
                                     Rank = rank.RankName,
                                     RankId = rank.RankId,
                                     Name = user.Name,
                                     UnitId = user.UnitId,
                                     UnitName = Uni.UnitName,
                                     IntOffr = user.IntOffr,

                                 }
                         ).Distinct().SingleOrDefaultAsync();
                return ret;

            }
            catch ( Exception ex ) { }

            return null;
        }
        public Task<List<DTOUserProfileResponse>> GetAll(string DomainId, int UserId)
        {
            // return _context.UserProfile.Where(P => P.ArmyNo == ArmyNo).SingleOrDefault();
            var ret = (from user in _context.UserProfile

                       join Uni in _context.MUnit on user.UnitId equals Uni.UnitId
                       join app in _context.MAppointment on user.ApptId equals app.ApptId
                       join forma in _context.MFormation on app.FormationId equals forma.FormationId
                       join rank in _context.MRank on user.RankId equals rank.RankId
                       join map in _context.MMappingProfile on user.UserId equals map.UserId
                       join userio in _context.UserProfile on map.IOId equals userio.UserId
                       join UniO in _context.MUnit on userio.UnitId equals UniO.UnitId
                       join rankIO in _context.MRank on userio.RankId equals rankIO.RankId
                       join usergso in _context.UserProfile on map.GSOId equals usergso.UserId
                       join UnGSO in _context.MUnit on userio.UnitId equals UnGSO.UnitId
                       join rankGSO in _context.MRank on usergso.RankId equals rankGSO.RankId
                         where user.Updatedby == UserId
                       select new DTOUserProfileResponse
                       {
                           MapId=map.Id,
                           ArmyNo = user.ArmyNo,
                           UserId = user.UserId,
                           FormationId=forma.FormationId,
                           FormationName = forma.FormationName,
                           ApptId=app.ApptId,
                           AppointmentName = app.AppointmentName,
                           Rank = rank.RankName,
                           Name = user.Name,
                           UnitId = user.UnitId,
                           UnitName = Uni.UnitName,
                           SusNo = Uni.Sus_no+Uni.Suffix,
                           IntOffr=user.IntOffr,


                           IOArmyNo = userio.ArmyNo,
                           IOName= rankIO.RankName+" "+userio.Name,
                           IOUserId = userio.UserId,
                           UnitIdIo = UniO.UnitId,
                           UnitIo=UniO.UnitName,
                           IOSusNo = UniO.Sus_no + UniO.Suffix,

                           GSOArmyNo = usergso.ArmyNo,
                           GSOName = rankGSO.RankName+" "+ usergso.Name,
                           GSOUserId = usergso.UserId,
                           UnitIdGSO = UnGSO.UnitId,
                           UnitGSO = UnGSO.UnitName,
                           GSOSusNo= UnGSO.Sus_no+ UnGSO.Suffix
                       }
                     ).Distinct().ToList();




            return Task.FromResult(ret);
        }
        public Task<DTOUserProfileResponse> GetAllByArmyNo(string ArmyNo, int UserId)
        {
            // return _context.UserProfile.Where(P => P.ArmyNo == ArmyNo).SingleOrDefault();
            var ret = (from user in _context.UserProfile
                       join basicd in _context.BasicDetails on user.ArmyNo equals basicd.ServiceNo
                       join icardreq in _context.TrnICardRequest on basicd.BasicDetailId equals icardreq.BasicDetailId
                       join Uni in _context.MUnit on user.UnitId equals Uni.UnitId
                       join app in _context.MAppointment on user.ApptId equals app.ApptId
                       join forma in _context.MFormation on app.FormationId equals forma.FormationId
                       join rank in _context.MRank on user.RankId equals rank.RankId
                       join map in _context.MMappingProfile on user.UserId equals map.UserId
                       join userio in _context.UserProfile on map.IOId equals userio.UserId
                       join UniO in _context.MUnit on userio.UnitId equals UniO.UnitId
                       join rankIO in _context.MRank on userio.RankId equals rankIO.RankId
                       join usergso in _context.UserProfile on map.GSOId equals usergso.UserId
                       join UnGSO in _context.MUnit on userio.UnitId equals UnGSO.UnitId
                       join rankGSO in _context.MRank on usergso.RankId equals rankGSO.RankId
                         where user.ArmyNo == ArmyNo //&&  user.Updatedby == UserId
                       select new DTOUserProfileResponse
                       {
                           MapId = map.Id,
                           ArmyNo = user.ArmyNo, 
                           UserId = user.UserId,
                           FormationId = forma.FormationId,
                           FormationName = forma.FormationName,
                           ApptId = app.ApptId,
                           AppointmentName = app.AppointmentName,
                           Rank = rank.RankName,
                           Name = user.Name,
                           UnitId = user.UnitId,
                           UnitName = Uni.UnitName,
                           SusNo = Uni.Sus_no + Uni.Suffix,
                           IntOffr = user.IntOffr,
                           RequestId= icardreq.RequestId,

                           IOArmyNo = userio.ArmyNo,
                           IOName = rankIO.RankAbbreviation + " " + userio.Name,
                           IOUserId = userio.UserId,
                           UnitIdIo = UniO.UnitId,
                           UnitIo = UniO.UnitName,
                           IOSusNo = UniO.Sus_no + UniO.Suffix,

                           GSOArmyNo = usergso.ArmyNo,
                           GSOName = rankGSO.RankAbbreviation + " " + usergso.Name,
                           GSOUserId = usergso.UserId,
                           UnitIdGSO = UnGSO.UnitId,
                           UnitGSO = UnGSO.UnitName,
                           GSOSusNo = UnGSO.Sus_no + UnGSO.Suffix
                           

                       }
                     ).Distinct().SingleOrDefault();




            return Task.FromResult(ret);
        }
    }
}
