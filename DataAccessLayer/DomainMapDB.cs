using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DomainMapDB : GenericRepositoryDL<TrnDomainMapping>, IDomainMapDB
    {
        protected readonly ApplicationDbContext _context;
        public DomainMapDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TrnDomainMapping> GetByAspnetUserIdBy(TrnDomainMapping Data)
        {
            var ret =await _context.TrnDomainMapping.Where(p => p.AspNetUsersId == Data.AspNetUsersId).SingleOrDefaultAsync();
            return ret;
        }

        public async Task<bool> GetByDomainId(TrnDomainMapping Data)
        {
            var ret = _context.TrnDomainMapping.Any(p => p.AspNetUsersId == Data.AspNetUsersId);
            return ret;
        }

        public async Task<TrnDomainMapping> GetByDomainIdbyUnit(TrnDomainMapping Data)
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
    }
}
