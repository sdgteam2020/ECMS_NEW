using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class MapUnitChangeDB : GenericRepositoryDL<TrnMapUnitChangeRequest>, IMapUnitChangeDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<MapUnitChangeDB> _logger;
        public MapUnitChangeDB(ApplicationDbContext context, DapperContext contextDP, ILogger<MapUnitChangeDB> logger, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _logger = logger;
            _contextDP = contextDP;
            _context = context;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<bool> FindUnitIdMapped(int UnitMapId)
        {
            try
            {
                return await _context.TrnMapUnitChangeRequest.AnyAsync(f => f.UnitMapId == UnitMapId && f.IsComplete == false);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitChangeDB->FindUnitIdMapped");
                return false;
            }
        }
        public async Task<DTOCommonSaveResponse> UpdateMapUnitChangeRequest(DTOSaveMapUnitChangeRequest dTO, TrnMapUnitChangeRequest trnMapUnit)
        {
            DTOCommonSaveResponse saveResponse = new DTOCommonSaveResponse();
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            string update1 = "";
            string update2 = "";

            try
            {
                //Accept
                if (dTO.Choice == 2)
                {
                    update1 = @"UPDATE MapUnit set UnitType = @UnitType,ComdId = @ComdId,CorpsId = @CorpsId, DivId = @DivId, BdeId = @BdeId, FmnBranchID = @FmnBranchID, PsoId =@PsoId, SubDteId = @SubDteId  WHERE UnitMapId=@UnitMapId";

                    var parameters = new DynamicParameters();
                    parameters.Add("@UnitMapId", dTO.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitType", dTO.UnitType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", dTO.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", dTO.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", dTO.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", dTO.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", dTO.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", dTO.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", dTO.SubDteId, DbType.Byte, ParameterDirection.Input);

                    await db.ExecuteAsync(update1, parameters, transaction: transaction);
                }

                update2 = @"UPDATE TrnMapUnitChangeRequest set AdminRemark = @AdminRemark, IsComplete = @IsComplete, IsEditAction = @IsEditAction, RequestStatus =@RequestStatus, ApproverUpdatedby = @ApproverUpdatedby, ApproverUpdatedOn = @ApproverUpdatedOn, ApproverUserId = @ApproverUserId  WHERE MapUnitChangeRequestId = @MapUnitChangeRequestId";
                var parameters2 = new DynamicParameters();
                parameters2.Add("@MapUnitChangeRequestId", dTO.MapUnitChangeRequestId, DbType.Int32, ParameterDirection.Input);
                parameters2.Add("@AdminRemark", dTO.AdminRemark, DbType.String, ParameterDirection.Input, 100);
                parameters2.Add("@IsComplete", trnMapUnit.IsComplete, DbType.Boolean, ParameterDirection.Input);
                parameters2.Add("@IsEditAction", trnMapUnit.IsEditAction, DbType.Boolean, ParameterDirection.Input);
                parameters2.Add("@RequestStatus", trnMapUnit.RequestStatus, DbType.Boolean, ParameterDirection.Input);
                parameters2.Add("@ApproverUpdatedby", trnMapUnit.ApproverUpdatedby, DbType.Int32, ParameterDirection.Input);
                parameters2.Add("@ApproverUpdatedOn", trnMapUnit.ApproverUpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters2.Add("@ApproverUserId", trnMapUnit.ApproverUserId, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync(update2, parameters2, transaction: transaction);

                saveResponse.Id = dTO.MapUnitChangeRequestId.ToString();
                saveResponse.Message = "Data Updated";

                // Commit the transaction if all operations succeed
                transaction.Commit();
                saveResponse.CurrentTime = trnMapUnit.ApproverUpdatedOn ?? DateTime.Now;
                saveResponse.Result = true;
                return saveResponse;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "FaultyCardDB->SaveFaultyCard");
                saveResponse.Result = false;
                saveResponse.Message = ex.Message;
                return saveResponse;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }
        public async Task<DTOMapUnitDetailsResponse> GetUnitMoveHistory(DTOMapUnitDetailsResponse dTO)
        {
            string query = "";
            query = @"Select mrak.RankAbbreviation,up.Name as RequestBy,up.ArmyNo,UnitChReq.Remark,UnitChReq.AdminRemark,UnitChReq.IsComplete,UnitChReq.IsEditAction,UnitChReq.RequestStatus,UnitChReq.ApproverUpdatedOn,
                        up_apro.Name as AprovedBy,up_apro.ArmyNo as AproverArmyNo,mrak_apro.RankAbbreviation as AproverRankAbbreviation,
                        mcom.ComdName AS RequestComdName,mcor.CorpsName  As RequestCorpsName,mdiv.DivName as RequestDivName,mbde.BdeName as RequestBdeName,mfmnb.BranchName as RequestBranchName,mpso.PSOName as RequestPSOName,msubd.SubDteName as RequestSubDteName
                        from TrnMapUnitChangeRequest UnitChReq
                        inner join UserProfile up on up.UserId = UnitChReq.FromUserId
                        inner join MRank mrak on mrak.RankId = up.RankId
                        left join UserProfile up_apro on up_apro.UserId = UnitChReq.ApproverUserId
                        left join MRank mrak_apro on mrak_apro.RankId = up_apro.RankId
                        inner join MComd mcom on mcom.ComdId = @ComdId
                        inner join MCorps mcor on mcor.CorpsId = @CorpsId
                        inner join MDiv mdiv on mdiv.DivId = @DivId
                        inner join MBde mbde on mbde.BdeId = @BdeId
                        inner join MFmnBranches mfmnb on mfmnb.FmnBranchID = @FmnBranchID
                        inner join MPso mpso on mpso.PsoId = @PsoId
                        inner join MSubDte msubd on msubd.SubDteId = @SubDteId
                        where UnitChReq.MapUnitChangeRequestId=@MapUnitChangeRequestId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MapUnitChangeRequestId", dTO.MapUnitChangeRequestId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", dTO.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", dTO.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", dTO.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", dTO.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", dTO.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", dTO.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", dTO.SubDteId, DbType.Byte, ParameterDirection.Input);

                    return await connection.QuerySingleAsync<DTOMapUnitDetailsResponse>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitChangeDB->GetUnitMoveHistory");
                return new DTOMapUnitDetailsResponse();
            }
        }
        public async Task<DTODataTablesResponse<DTOMapUnitChangeResponse>> GetAllMapUnitChange(DTODataTablesRequestForMapUnitChange request)
        {
            try
            {
                var queryableData = (from unitch in _context.TrnMapUnitChangeRequest.OrderByDescending(x=>x.MapUnitChangeRequestId)
                                     join upfrom in _context.UserProfile on unitch.FromUserId equals upfrom.UserId
                                     join rkfrom in _context.MRank on upfrom.RankId equals rkfrom.RankId
                                     join ufrom in _context.Users on unitch.Updatedby equals ufrom.Id
                                     join mapunit in _context.MapUnit on unitch.UnitMapId equals mapunit.UnitMapId
                                     join munit in _context.MUnit on mapunit.UnitId equals munit.UnitId
                                     select new DTOMapUnitChangeResponse()
                                     {
                                         MapUnitChangeRequestId = unitch.MapUnitChangeRequestId,
                                         EncryptedId = protector.Protect(unitch.MapUnitChangeRequestId.ToString()),
                                         UnitMapId = unitch.UnitMapId,
                                         ExistingCh = unitch.ExistingCh,
                                         RequestCh = unitch.RequestCh,
                                         Remark= unitch.Remark,
                                         AdminRemark = unitch.AdminRemark,
                                         IsComplete = unitch.IsComplete,
                                         IsActive = unitch.IsActive,
                                         IsEditAction = unitch.IsEditAction,
                                         RequestStatus= unitch.RequestStatus,
                                         FromUpdatedby = unitch.Updatedby ?? 0,
                                         FromUpdatedOn = unitch.UpdatedOn ?? DateTime.Now,
                                         FromUserId = unitch.FromUserId,
                                         ApproverUpdatedby = unitch.Updatedby,
                                         ApproverUpdatedOn = unitch.UpdatedOn,
                                         ApproverUserId = unitch.ApproverUserId,
                                         UnitName = munit.UnitName,
                                         FromDID = ufrom.DomainId,
                                         FromRankAbbreviation= rkfrom.RankAbbreviation,
                                         FromArmyNo = upfrom.ArmyNo,
                                         FromName = upfrom.Name,
                                     }).AsQueryable();
                if (request.RoleName != "admin")
                {
                    queryableData = queryableData.Where(x => x.UnitMapId == request.UnitMapId);
                }

                // Total records without filtering
                var totalRecords = queryableData.Count();

                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();

                    //queryableData = queryableData.Where(x =>  x.UserId.ToString().ToLower().Contains(searchValue) ||
                    //                          x.DomainId.ToLower().Contains(searchValue)||
                    //                          x.ArmyNo.ToLower().Contains(searchValue));

                    queryableData = queryableData.Where(x => x.FromArmyNo.ToLower().Contains(searchValue));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    //queryableData = queryableData.OrderBy(request.SortColumn + " " + request.SortColumnDirection);
                    queryableData = request.sortDirection.ToLower() == "asc"
                    ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                    : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                }

                // Total records after filtering
                var filteredRecords = queryableData.Count();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                var responseData = new DTODataTablesResponse<DTOMapUnitChangeResponse>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords, // Total records without filtering
                    recordsFiltered = filteredRecords, // Total records after filtering
                    data = paginatedData
                };

                return responseData;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitChangeDB->GetAllMapUnitChange");
                List<DTOMapUnitChangeResponse> dTOUserRegnResponses = new List<DTOMapUnitChangeResponse>();
                var responseData = new DTODataTablesResponse<DTOMapUnitChangeResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }
    }
}
