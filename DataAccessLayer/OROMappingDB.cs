using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
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
    public class OROMappingDB : GenericRepositoryDL<OROMapping>, IOROMappingDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<OROMappingDB> _logger;
        public OROMappingDB(ApplicationDbContext context, DapperContext contextDP, ILogger<OROMappingDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        public async Task<bool> GetByName(OROMapping Dto)
        {
            var ret =  await _context.OROMapping.AnyAsync(x => x.OROMappingId != Dto.OROMappingId);
            return ret;
        }
        public async Task<List<DTOOROMappingResponse>?> GetAllOROMapping()
        {
            try
            {
                string query = "";
                query = "Select oromap.OROMappingId,oromap.ArmedIdList,oromap.RankId,mrak.RankName,mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,oromap.TDMId,oromap.UnitId,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName" +
                        " ,(select STRING_AGG(ArmedName,'#') from MArmedType where ArmedId in (select value from string_split(oromap.ArmedIdList,','))) ArmNameList from OROMapping oromap" +
                        " inner join MRecordOffice mrecord on mrecord.RecordOfficeId=oromap.RecordOfficeId" +
                        " left join MRank mrak on mrak.RankId=oromap.RankId" +
                        " left join TrnDomainMapping trndomain on trndomain.Id=oromap.TDMId" +
                        " left join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " left join MRank ra on ra.RankId=usep.RankId " +
                        " left join MapUnit mapunit on mapunit.UnitMapId = oromap.UnitId " +
                        " left join MUnit munit on munit.UnitId =mapunit.UnitId order by oromap.OROMappingId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOOROMappingResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "OROMappingDB->GetAllOROMapping");
                return null;
            }

        }
    }
}
