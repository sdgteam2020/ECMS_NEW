using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
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
    public class AfsacCellMappingDB : GenericRepositoryDL<AfsacCellMapping>, IAfsacCellMappingDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<AfsacCellMappingDB> _logger;
        public AfsacCellMappingDB(ApplicationDbContext context, DapperContext contextDP, ILogger<AfsacCellMappingDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        public async Task<bool> GetByName(AfsacCellMapping Dto)
        {
            var ret = await _context.AfsacCellMapping.AnyAsync(x => x.AfsacCellMappingId != Dto.AfsacCellMappingId);
            return ret;
        }
        public async Task<List<DTOAfsacCellMappingResponse>?> GetAllAfsacCellMapping()
        {
            try
            {
                string query = "";
                query = "Select acmap.AfsacCellMappingId,acmap.TDMId,acmap.UnitId,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName from AfsacCellMapping acmap" +
                        " left join TrnDomainMapping trndomain on trndomain.Id=acmap.TDMId" +
                        " left join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " left join MRank ra on ra.RankId=usep.RankId " +
                        " left join MapUnit mapunit on mapunit.UnitMapId = acmap.UnitId " +
                        " left join MUnit munit on munit.UnitId =mapunit.UnitId order by acmap.AfsacCellMappingId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOAfsacCellMappingResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AfsacCellMappingDB->GetAllAfsacCellMapping");
                return null;
            }

        }
    }
}