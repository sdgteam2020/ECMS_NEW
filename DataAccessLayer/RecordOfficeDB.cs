using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class RecordOfficeDB : GenericRepositoryDL<MRecordOffice>, IRecordOfficeDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<RecordOfficeDB> _logger;
        public RecordOfficeDB(ApplicationDbContext context, DapperContext contextDP, ILogger<RecordOfficeDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        private readonly IConfiguration configuration;

        public async Task<int> GetByName(MRecordOffice Dto)
        {
            List<MRecordOffice> mRecordOffices = await _context.MRecordOffice.AsNoTracking().ToListAsync();
            if (mRecordOffices.Any(x => (x.Name.ToUpper() == Dto.Name.ToUpper() || x.Abbreviation.ToUpper() == Dto.Abbreviation.ToUpper()) && x.RecordOfficeId != Dto.RecordOfficeId))
            {
                return 2;
            }
            else if (mRecordOffices.Any(x => x.ArmedId == Dto.ArmedId && x.RecordOfficeId != Dto.RecordOfficeId))
            {
                return 3;
            }
            else if (mRecordOffices.Any(x => x.TDMId == Dto.TDMId && x.RecordOfficeId != Dto.RecordOfficeId))
            {
                return 4;
            }
            else
            {
                return 1;
            }
        }
        public async Task<bool> GetByTDMId(int TDMId)
        {
            List<MRecordOffice> mRecordOffices = await _context.MRecordOffice.AsNoTracking().ToListAsync();
            var result = mRecordOffices.Any(x => x.TDMId == TDMId);
            return result;
        }

        public Task<List<DTORecordOfficeResponse>> GetAllData()
        {
            var Corps = (from c in _context.MRecordOffice
                         join d in _context.MArmedType
                         on c.ArmedId equals d.ArmedId

                         select new DTORecordOfficeResponse
                         {
                             RecordOfficeId = c.RecordOfficeId,
                             Name = c.Name,
                             Abbreviation = c.Abbreviation,
                             ArmedId = c.ArmedId,
                             ArmedName = d.ArmedName,
                             TDMId = c.TDMId,
                         }).ToList();



            return Task.FromResult(Corps);
        }
        public async Task<DTOGetUpdateRecordOfficeResponse?> GetUpdateRecordOffice(int TDMId)
        {
            try
            {
                string query = "";
                query = "Select trndomain.UnitId as UnitMapId,users.DomainId,ra.RankAbbreviation,usep.Name,usep.ArmyNo,mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName ,mrecord.Abbreviation,mrecord.TDMId,marmed.ArmedName from TrnDomainMapping trndomain" +
                        " inner join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " inner join MRecordOffice mrecord on mrecord.TDMId=trndomain.Id" +
                        " inner join MArmedType marmed on marmed.ArmedId=mrecord.ArmedId" +
                        " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " left join MRank ra on ra.RankId=usep.RankId " +
                        " where trndomain.Id=@TDMId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetUpdateRecordOfficeResponse>(query, new { TDMId });
                    return allrecord.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MasterDB->GetMappedForRecord");
                return null;
            }
        }
        public async Task<List<DTOGetMappedForRecordResponse>?> GetDDMappedForRecord(int UnitMapId)
        {
            try
            {
                string query = "";
                query = "Select users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name,trndomain.Id as TDMId from AspNetUsers users" +
                        " inner join TrnDomainMapping trndomain on users.Id=trndomain.AspNetUsersId" +
                        " inner join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " inner join MRank ra on ra.RankId=usep.RankId " +
                        " where trndomain.UnitId=@UnitMapId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetMappedForRecordResponse>(query, new { UnitMapId });
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MasterDB->GetDDMappedForRecord");
                return null;
            }

        }
    }
}
