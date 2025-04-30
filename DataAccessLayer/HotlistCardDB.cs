using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class HotlistCardDB : GenericRepositoryDL<TrnHotlistCard>, IHotlistCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<HotlistCardDB> _logger;

        public HotlistCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<HotlistCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
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
                _logger.LogError(1001, ex, "HotlistCardDB->FindAnyRequestId");
                return false;
            }
        }

        public async Task<DTODataTablesResponse<DTOHotlistCardGetResponse>> GetAllHotlist(DTODataTablesRequest dTO)
        {
            List<DTOHotlistCardGetResponse> dTOHotlistCardGetResponses = new List<DTOHotlistCardGetResponse>();
            var responseData = new DTODataTablesResponse<DTOHotlistCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOHotlistCardGetResponses
            };
            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ModifiedServiceNo"] = "bas.ServiceNo",
                    ["UpdatedOn"] = "hotlist.UpdatedOn",
                    ["RequestId"] = "req.RequestId",
                    ["Remark"] = "hotlist.Remark"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "hotlist.UpdatedOn";

                var sortOrder = dTO.sortDirection;

                string query = "";
                    query = @"appl.Name ApplyFor,
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
                                left join MRegimental regi on regi.RegId=bas.RegimentalId
                                Where bas.ServiceNo like '%' + @SearchTerm + '%' ";

                var multiQuery = query = $@"
                            WITH RecordCTE AS (
                                select ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query}
                            )
                            SELECT * FROM RecordCTE
                            WHERE RowNum BETWEEN @Offset AND @Limit;
                            Select Count(*) from TrnHotlistCards;
                        ";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryMultipleAsync(query, new { Offset = dTO.Start, Limit = dTO.Length, SearchTerm = string.IsNullOrWhiteSpace(dTO.searchValue) ? "" : dTO.searchValue });
                    var records = (await ret.ReadAsync<DTOHotlistCardGetResponse>()).ToList();
                    var totalRecords = (await ret.ReadAsync<int>()).Single();
                    responseData = new DTODataTablesResponse<DTOHotlistCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = records.Count(), // Total records after filtering
                        data = (from e in records
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
                                }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HotlistCardDB->GetAllHotlist");
            }
            return responseData;
        }

        public async Task<List<DTOHotlistCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            var records = new List<DTOHotlistCardExportResponse>();
            try
            {
                string query = @"select req.RequestId,hotlist.HotlistCardId,bas.ServiceNo as ArmyNo,
	                                ranks.RankAbbreviation,bas.FName,bas.LName,Muni.Abbreviation Unit,
	                                hotlist.UpdatedOn as DateAndTime,hotlist.Remark,hotlist.IsActive as IsActiveBool,
	                                (select STRING_AGG(Remarks,' | ') from MRemarks where RemarksId in (select value from string_split(hotlist.RemarksIds,','))) Reasons,
	                                req.CardSerialNo,req.ChipNo
	                                from TrnHotlistCards hotlist
	                                inner join TrnICardRequest req on req.RequestId = hotlist.RequestId
	                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId=bas.RankId
	                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
	                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                  Where req.RequestId in @Ids";
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Data.Ids);
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOHotlistCardExportResponse>(query, parameters);
                    records = ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HotlistCardDB->GetBesicdetailsByRequestId");
            }
            return records;
        }
    }
}
