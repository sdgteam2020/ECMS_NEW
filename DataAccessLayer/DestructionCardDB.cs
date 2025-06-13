using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class DestructionCardDB : GenericRepositoryDL<TrnDestructionCard>, IDestructionCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<DestructionCardDB> _logger;

        public DestructionCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<DestructionCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
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
                return await _context.TrnDestructionCards
                                .AnyAsync(f => f.RequestId == RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DestructionCardDB->FindAnyRequestId");
                return false;
            }
        }

        public async Task<DTODataTablesResponse<DTODestructionCardGetResponse>> GetAllDestruction(DTODataTablesRequest dTO)
        {
            List<DTODestructionCardGetResponse> dTODestructionCardGetResponses = new List<DTODestructionCardGetResponse>();
            var responseData = new DTODataTablesResponse<DTODestructionCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTODestructionCardGetResponses
            };
            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ModifiedServiceNo"] = "bas.ServiceNo",
                    ["UpdatedOn"] = "tdc.UpdatedOn",
                    ["RequestId"] = "req.RequestId",
                    ["Remark"] = "tdc.Remark"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "tdc.UpdatedOn";

                var sortOrder = dTO.sortDirection;

                string query = "";
                    query = @"appl.Name ApplyFor,
                                req.RequestId,tdc.DestructedCardId,
                                bas.ServiceNo,ranks.RankAbbreviation RankName,
                                bas.FName,bas.LName,
                                Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                                tdc.UpdatedOn,tdc.Remark,tdc.IsActive,
                                bas.NameAsPerRecord,
                                regi.Abbreviation RegimentalName,
                                CASE
                                WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN
                                CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2))
                                ELSE
                                bas.ServiceNo
                                END AS ModifiedServiceNo,tdc.DestructedOn,
                                (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(tdc.RemarksIds,','))) RemarksNameList,
                                tdc.RemarksIds
                                from TrnDestructionCards tdc
                                inner join TrnICardRequest req on req.RequestId = tdc.RequestId
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                                inner join MRank ranks on ranks.RankId=bas.RankId
                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                                left join MRegimental regi on regi.RegId=bas.RegimentalId
                                Where bas.ServiceNo like '%' + @SearchTerm + '%' ";

                var multiQuery = query = $@"
                            WITH RecordCTE AS (
                                select ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query}
                            )
                            SELECT * FROM RecordCTE
                            WHERE RowNum BETWEEN @Offset AND @Limit;

                            Select Count(*) from TrnDestructionCards;
                        ";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryMultipleAsync(query, new { Offset = dTO.Start, Limit = dTO.Length, SearchTerm = string.IsNullOrWhiteSpace(dTO.searchValue) ? "" : dTO.searchValue });
                    var records = (await ret.ReadAsync<DTODestructionCardGetResponse>()).ToList();
                    var totalRecords = (await ret.ReadAsync<int>()).Single();
                    responseData = new DTODataTablesResponse<DTODestructionCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = records.Count(), // Total records after filtering
                        data = (from e in records
                                select new DTODestructionCardGetResponse()
                                {
                                    EncryptedId = protector.Protect(e.DestructedCardId.ToString()),
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
                                    DestructedCardId = e.DestructedCardId,
                                    Remark = e.Remark,
                                    RemarksIds = e.RemarksIds,
                                    RemarksNameList = e.RemarksNameList,
                                    IsActive = e.IsActive,
                                    DestructedOn = e.DestructedOn
                                }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DestructionCardDB->GetAllDestruction");
            }
            return responseData;
        }

        public async Task<List<DTODestructionCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            var records = new List<DTODestructionCardExportResponse>();
            try
            {
                string query = @"select req.RequestId,tdc.DestructedCardId,bas.ServiceNo as ArmyNo,
	                                ranks.RankAbbreviation,bas.FName,bas.LName,Muni.Abbreviation Unit,
	                                tdc.UpdatedOn as DateAndTime,tdc.Remark,tdc.IsActive as IsActiveBool,
	                                req.CardSerialNo,req.ChipNo,tdc.DestructedOn,
                                    (select STRING_AGG(Remarks,' | ') from MRemarks where RemarksId in (select value from string_split(tdc.RemarksIds,','))) Reasons
	                                from TrnDestructionCards tdc
	                                inner join TrnICardRequest req on req.RequestId = tdc.RequestId
	                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId=bas.RankId
	                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
	                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                  Where req.RequestId in @Ids";
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Data.Ids);
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTODestructionCardExportResponse>(query, parameters);
                    records = ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DestructionCardDB->GetDetailsByRequestIds");
            }
            return records;
        }
    }
}
