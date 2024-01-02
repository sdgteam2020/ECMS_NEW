using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.Intrinsics.Arm;

namespace DataAccessLayer
{
    public class DomainMapDB : GenericRepositoryDL<TrnDomainMapping>, IDomainMapDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly ILogger<DomainMapDB> _logger;
        public DomainMapDB(ApplicationDbContext context, ILogger<DomainMapDB> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TrnDomainMapping?> GetByAspnetUserIdBy(int AspNetUsersId)
        {
            var ret =await _context.TrnDomainMapping.Where(p => p.AspNetUsersId == AspNetUsersId).FirstOrDefaultAsync();
            return ret;
        }
        public async Task<TrnDomainMapping?> GetTrnDomainMappingByUserId(int UserId)
        {
            var ret = await _context.TrnDomainMapping.Where(p => p.UserId == UserId).FirstOrDefaultAsync();
            return ret;
        }

        public async Task<bool> GetByDomainId(TrnDomainMapping Data)
        {
            var ret = await _context.TrnDomainMapping.AnyAsync(p => p.AspNetUsersId == Data.AspNetUsersId);
            return ret;
        }

        public async Task<TrnDomainMapping?> GetByDomainIdbyUnit(TrnDomainMapping Data)
        {
            return await _context.TrnDomainMapping.Where(p => p.AspNetUsersId == Data.AspNetUsersId).FirstOrDefaultAsync();
        }

        public async Task<TrnDomainMapping> GetByRequestId(int RequestId)
        {
            //SELECT trndom.AspNetUsersId,trndom.UserId from TrnDomainMapping trndom
            //inner join TrnICardRequest trncard on trndom.id = trncard.TrnDomainMappingId
            //where trncard.RequestId = 16
            var ret = await (from trndomap in _context.TrnDomainMapping
                      join trnicardreq in _context.TrnICardRequest on trndomap.Id equals trnicardreq.TrnDomainMappingId
                      where trnicardreq.RequestId == RequestId
                      select new TrnDomainMapping
                      {
                         AspNetUsersId=trndomap.AspNetUsersId,
                          UserId= trndomap.UserId,
                      }).FirstOrDefaultAsync();
            return  ret;
        }
        public async Task<TrnDomainMapping?> GetAllRelatedDataByDomainId(string DomainId)
        {
            try
            {
                var result = await (from au in _context.Users 
                                    join tdm in _context.TrnDomainMapping on au.Id equals tdm.AspNetUsersId into autdm_jointable
                                    from xtdm in autdm_jointable.DefaultIfEmpty()
                                    join up in _context.UserProfile on xtdm.UserId equals up.UserId into tdmup_jointable
                                    from xup in tdmup_jointable.DefaultIfEmpty()
                                    where au.DomainId == DomainId
                                    select new TrnDomainMapping
                                    {
                                        Id = xtdm != null? xtdm.Id:0,
                                        UnitId = xtdm != null ? xtdm.UnitId : 0,
                                        MapUnit = xtdm != null ? xtdm.MapUnit : null,
                                        ApptId = (short)(xtdm != null ? xtdm.ApptId : 0),
                                        AspNetUsersId = au != null ? au.Id:0,
                                        UserId = xup != null ? xup.UserId : null,
                                        ApplicationUser = au != null ? au : null,
                                        MUserProfile = xup != null ? xup : null,
                                    }).FirstOrDefaultAsync();
                return (TrnDomainMapping?)result;
            }
            catch(Exception ex)
            {
                _logger.LogInformation(1001, ex, "GetAllRelatedDataByDomainId");
                return null;
            }

        }
        public async Task<TrnDomainMapping?> GetProfileDataByAspNetUserId(int Id)
        {
            try
            {
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
                return (TrnDomainMapping?)result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(1001, ex, "GetProfileDataByAspNetUserId");
                return null;
            }

        }
    }
}
