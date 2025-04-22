using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class HotlistCardDB : GenericRepositoryDL<TrnHotlistCard>, IHotlistCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<FaultyCardDB> _logger;

        public HotlistCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<FaultyCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            try
            {
                return _context.TrnHotlistCards
                                .Any(f => f.RequestId == RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HotlistCardDB->FindRequestId");
                return false;
            }
        }

        public async Task<List<DTOHotlistCardGetResponse>?> GetAllHotlist()
        {
            try
            {
                string query = "";
                    query = @"SELECT appl.Name ApplyFor,
                                req.RequestId,hotlist.HotlistCardId,
                                bas.ServiceNo,ranks.RankAbbreviation RankName,
                                bas.FName,bas.LName,
                                Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                                hotlist.UpdatedOn,hotlist.RemarksIds,hotlist.Remark,hotlist.IsActive,
                                bas.NameAsPerRecord,
                                regi.Abbreviation RegimentalName,
                                CASE
                                WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN
                                CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2))
                                ELSE
                                bas.ServiceNo
                                END AS ModifiedServiceNo,
                                (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(hotlist.RemarksIds,','))) RemarksNameList
                                from TrnHotlistCards hotlist
                                inner join TrnICardRequest req on req.RequestId = hotlist.RequestId
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                                inner join MRank ranks on ranks.RankId=bas.RankId
                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                                left join MRegimental regi on regi.RegId=bas.RegimentalId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecordList = await connection.QueryAsync<DTOHotlistCardGetResponse>(query);
                    var allrecord = (from e in allrecordList
                                     select new DTOHotlistCardGetResponse()
                                     {
                                         EncryptedId = protector.Protect(e.HotlistCardId.ToString()),
                                         NameAsPerRecord = e.NameAsPerRecord,
                                         FName = e.FName,
                                         LName = e.LName,
                                         ServiceNo = e.ServiceNo,
                                         ModifiedServiceNo = e.ModifiedServiceNo,
                                         UnitName = e.UnitName,
                                         UnitAbbreviation = e.UnitAbbreviation,
                                         RankName = e.RankName,
                                         ArmedName = e.ArmedName,
                                         RequestId = e.RequestId,
                                         UpdatedOn = e.UpdatedOn,
                                         ApplyFor = e.ApplyFor,
                                         HotlistCardId = e.HotlistCardId,
                                         RemarksIds = e.RemarksIds,
                                         RemarksNameList = e.RemarksNameList,
                                         Remark = e.Remark,
                                         IsActive = e.IsActive,
                                     }).ToList();
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "FaultyCardDB->GetAllFaulty");
                return null;
            }

        }
    }
}
