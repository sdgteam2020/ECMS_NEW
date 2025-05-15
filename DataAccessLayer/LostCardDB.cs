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
    public class LostCardDB : GenericRepositoryDL<TrnLostCard> , ILostCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<LostCardDB> _logger;

        public LostCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<LostCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
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
                return _context.TrnLostCards
                                .Any(f => f.RequestId == RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "LostCardDB->FindAnyRequestId");
                return false;
            }
        }

        public async Task<DTODataTablesResponse<DTOLostCardGetResponse>> GetAllLost(DTODataTablesRequest dTO)
        {
            List<DTOLostCardGetResponse> dTOLostCardGetResponses = new List<DTOLostCardGetResponse>();
            var responseData = new DTODataTablesResponse<DTOLostCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOLostCardGetResponses
            };
            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ModifiedServiceNo"] = "bas.ServiceNo",
                    ["UpdatedOn"] = "lost.UpdatedOn",
                    ["LostOn"] = "lost.LostOn",
                    ["Remark"] = "lost.Remark"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "lost.UpdatedOn";

                var sortOrder = dTO.sortDirection;

                string query = "";
                query = @"appl.Name ApplyFor,
                            req.RequestId,lost.LostCardId,
                            bas.ServiceNo,ranks.RankAbbreviation RankName,
                            bas.FName,bas.LName,
                            Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                            lost.UpdatedOn,lost.Remark,lost.IsActive,
                            bas.NameAsPerRecord,lost.LostOn,
                            regi.Abbreviation RegimentalName,
                            CASE
                            WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN
                            CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2))
                            ELSE
                            bas.ServiceNo
                            END AS ModifiedServiceNo,lost.IsFIRLogged,lost.SupportDocName
                            from TrnLostCards lost
                            inner join TrnICardRequest req on req.RequestId = lost.RequestId
                            inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                            inner join MRank ranks on ranks.RankId=bas.RankId
                            inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                            inner join MUnit Muni on Muni.UnitId=uni.UnitId
                            inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                            left join MRegimental regi on regi.RegId=bas.RegimentalId
                            Where bas.ServiceNo like '%' + @SearchTerm + '%'";

                var multiQuery = query = $@"
                            WITH RecordCTE AS (
                                select ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query}
                            )
                            SELECT * FROM RecordCTE
                            WHERE RowNum BETWEEN @Offset AND @Limit;
                            Select Count(*) from TrnLostCards;
                        ";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryMultipleAsync(query, new { Offset = dTO.Start, Limit = dTO.Length, SearchTerm = string.IsNullOrWhiteSpace(dTO.searchValue) ? "" : dTO.searchValue });
                    var records = (await ret.ReadAsync<DTOLostCardGetResponse>()).ToList();
                    var totalRecords = (await ret.ReadAsync<int>()).Single();
                    responseData = new DTODataTablesResponse<DTOLostCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = records.Count(), // Total records after filtering
                        data = (from e in records
                                select new DTOLostCardGetResponse()
                                {
                                    EncryptedId = protector.Protect(e.LostCardId.ToString()),
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
                                    LostCardId = e.LostCardId,
                                    LostOn = e.LostOn,
                                    Remark = e.Remark,
                                    IsActive = e.IsActive,
                                    SupportDocName = e.SupportDocName,
                                    IsFIRLogged = e.IsFIRLogged
                                }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "LostCardDB->GetAllLost");
            }
            return responseData;
        }

        public async Task<List<DTOLostCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            var records = new List<DTOLostCardExportResponse>();
            try
            {
                string query = @"select req.RequestId,lost.LostCardId,bas.ServiceNo as ArmyNo,
	                                ranks.RankAbbreviation,bas.FName,bas.LName,Muni.Abbreviation Unit,
	                                lost.UpdatedOn as DateAndTime,lost.Remark,lost.IsActive as IsActiveBool,
	                                req.CardSerialNo,req.ChipNo,lost.LostOn
	                                from TrnLostCards lost
	                                inner join TrnICardRequest req on req.RequestId = lost.RequestId
	                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId=bas.RankId
	                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
	                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                  Where req.RequestId in @Ids";
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Data.Ids);
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOLostCardExportResponse>(query, parameters);
                    records = ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "LostCardDB->GetBesicdetailsByRequestId");
            }
            return records;
        }
    }
}
