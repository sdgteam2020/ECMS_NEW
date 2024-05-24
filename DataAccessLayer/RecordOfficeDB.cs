using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Identity;
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
            //else if (mRecordOffices.Any(x => x.ArmedId == Dto.ArmedId && x.RecordOfficeId != Dto.RecordOfficeId))
            //{
            //    return 3;
            //}
            //else if (mRecordOffices.Any(x => x.TDMId == Dto.TDMId && x.RecordOfficeId != Dto.RecordOfficeId))
            //{
            //    return 4;
            //}
            else
            {
                return 1;
            }
        }
        public async Task<bool> GetByTDMId(int UnitId,int? TDMId)
        {
            List<MRecordOffice> mRecordOffices = await _context.MRecordOffice.AsNoTracking().ToListAsync();
            if(TDMId!=null)
            {
                var result = mRecordOffices.Any(x => x.TDMId == TDMId && x.UnitId== UnitId);
                return result;
            }
            else
            {
                var result = mRecordOffices.Any(x => x.UnitId == UnitId);
                return result;
            }

        }
        public async Task<DTOGetROByTDMIdResponse?> GetROByTDMId(int TDMId)
        {
            try
            {
                string query = "";
                query = "Select tdm.IsRO, tdm.IsORO, mrecord.TDMId,mrecord.RecordOfficeId,mrecord.UnitId from TrnDomainMapping tdm" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId = tdm.UnitId" +
                        " inner join MRecordOffice mrecord on mrecord.UnitId = mapunit.UnitMapId" +
                        " where tdm.Id=@TDMId ";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetROByTDMIdResponse>(query, new { TDMId });
                    return allrecord.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RecordOfficeDB->GetByUserId");
                return null;
            }

        }
        public async Task<List<DTORecordOfficeResponse>?> GetAllData()
        {
            try
            {
                string query = "";
                query = "Select mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,mrecord.Message,mrecord.Abbreviation,mrecord.TDMId,mrecord.UnitId,marmed.ArmedId,marmed.ArmedName,users.DomainId,usep.ArmyNo,ra.RankAbbreviation,usep.Name, munit.Sus_no,munit.Suffix,munit.UnitName from MRecordOffice mrecord" +
                        " inner join MArmedType marmed on marmed.ArmedId=mrecord.ArmedId" +
                        " left join TrnDomainMapping trndomain on trndomain.Id=mrecord.TDMId" +
                        " left join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " left join UserProfile usep on usep.UserId=trndomain.UserId" +
                        " left join MRank ra on ra.RankId=usep.RankId " +
                        " left join MapUnit mapunit on mapunit.UnitMapId = mrecord.UnitId " +
                        " left join MUnit munit on munit.UnitId =mapunit.UnitId order by mrecord.RecordOfficeId desc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTORecordOfficeResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RecordOfficeDB->GetAllData");
                return null;
            }

        }
        public async Task<DTOGetUpdateRecordOfficeResponse?> GetUpdateRecordOffice(int RecordOfficeId)
        {
            try
            {
                string query = "";
                query = "Select mrecord.RecordOfficeId,mrecord.Name as RecordOfficeName,mrecord.Abbreviation,mrecord.Message,marmed.ArmedName from TrnDomainMapping trndomain" +
                        " inner join AspNetUsers users on users.Id=trndomain.AspNetUsersId" +
                        " inner join MRecordOffice mrecord on mrecord.TDMId=trndomain.Id" +
                        " inner join MArmedType marmed on marmed.ArmedId=mrecord.ArmedId" +
                        " where mrecord.RecordOfficeId=@RecordOfficeId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOGetUpdateRecordOfficeResponse>(query, new { RecordOfficeId });
                    return allrecord.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "RecordOfficeDB->GetMappedForRecord");
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
                _logger.LogError(1001, ex, "RecordOfficeDB->GetDDMappedForRecord");
                return null;
            }

        }
        public async Task<bool?> UpdateROValue(DTOUpdateROValueRequest dTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var roUpdate = await _context.MRecordOffice.FindAsync(dTO.RecordOfficeId); 
                    if(roUpdate == null)
                    {
                        return false;
                    }
                    else
                    {
                        roUpdate.TDMId = dTO.TDMId;
                        roUpdate.Message = dTO.Message;
                        roUpdate.Updatedby = dTO.Updatedby;
                        roUpdate.UpdatedOn = dTO.UpdatedOn;
                        _context.MRecordOffice.Update(roUpdate);
                        await _context.SaveChangesAsync();
                        
                        transaction.Commit();
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(1001, ex, "RecordOfficeDB->UpdateROValue");
                    return null;
                }
            }
        }
    }
}
