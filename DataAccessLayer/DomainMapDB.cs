using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLayer
{
    public class DomainMapDB : GenericRepositoryDL<TrnDomainMapping>, IDomainMapDB
    {
        protected readonly ApplicationDbContext _context;
        public DomainMapDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TrnDomainMapping?> GetByAspnetUserIdBy(int AspNetUsersId)
        {
            var ret =await _context.TrnDomainMapping.Where(p => p.AspNetUsersId == AspNetUsersId).SingleOrDefaultAsync();
            return ret;
        }
        public async Task<TrnDomainMapping?> GetTrnDomainMappingByUserId(int UserId)
        {
            var ret = await _context.TrnDomainMapping.Where(p => p.UserId == UserId).SingleOrDefaultAsync();
            return ret;
        }

        public async Task<bool> GetByDomainId(TrnDomainMapping Data)
        {
            var ret = await _context.TrnDomainMapping.AnyAsync(p => p.AspNetUsersId == Data.AspNetUsersId);
            return ret;
        }

        public async Task<TrnDomainMapping?> GetByDomainIdbyUnit(TrnDomainMapping Data)
        {
            return await _context.TrnDomainMapping.Where(p => p.AspNetUsersId == Data.AspNetUsersId).SingleOrDefaultAsync();
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
                      }).SingleOrDefaultAsync();
            return  ret;
        }
        public async Task<TrnDomainMapping?> GetAllRelatedDataByDomainId(string DomainId)
        {
            try
            {
                var result = await (from au in _context.Users where au.DomainId == DomainId
                                    join tdm in _context.TrnDomainMapping on au.Id equals tdm.AspNetUsersId into autdm_jointable
                                    from xtdm in autdm_jointable.DefaultIfEmpty()
                                    join up in _context.UserProfile on xtdm.UserId equals up.UserId into tdmup_jointable
                                    from xup in tdmup_jointable.DefaultIfEmpty()
                                    select new TrnDomainMapping
                                    {
                                        Id = xtdm != null? xtdm.Id:0,
                                        AspNetUsersId = au != null ? au.Id:0,
                                        UserId = xtdm != null ? xtdm.UserId:0,
                                        UnitId = xtdm != null ? xtdm.UnitId:0,
                                        ApplicationUser = au != null ? au:null,
                                        MUserProfile = xup!=null? xup:null,
                                    }).SingleOrDefaultAsync();
                //var result = await (from au in _context.Users
                //                    join tdm in _context.TrnDomainMapping on au.Id equals tdm.AspNetUsersId into autdm_jointable
                //                    from column in autdm_jointable.DefaultIfEmpty()
                //                    join up in _context.UserProfile on column.UserId equals up.UserId
                //                    where au.DomainId == DomainId
                //                    select new TrnDomainMapping
                //                    {
                //                        Id = column.Id,
                //                        AspNetUsersId = column.AspNetUsersId,
                //                        UserId = column.UserId,
                //                        UnitId = column.UnitId,
                //                        ApplicationUser = au,
                //                        MUserProfile = up,
                //                    }).SingleOrDefaultAsync();
                return (TrnDomainMapping?)result;
            }
            catch(Exception ex)
            {
                return null;
            }

        }
    }
}
