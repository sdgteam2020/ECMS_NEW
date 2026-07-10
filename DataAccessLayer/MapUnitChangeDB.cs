using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

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

        /// <summary>
        /// Checks if the specified UnitMapId has any incomplete change requests.
        /// </summary>
        /// <param name="UnitMapId">The UnitMapId to check for pending change requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if there are any incomplete requests, false otherwise.</returns>
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

        /// <summary>
        /// Updates the unit change request with the specified details.
        /// </summary>
        /// <param name="dTO">The DTO containing the details for updating the unit change request.</param>
        /// <param name="trnMapUnit">The existing unit change request entity to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a DTO response indicating the success or failure of the operation.</returns>
        public async Task<DTOCommonSaveResponse> UpdateMapUnitChangeRequest(DTOSaveMapUnitChangeRequest dTO, TrnMapUnitChangeRequest trnMapUnit)
        {
            DTOCommonSaveResponse saveResponse = new DTOCommonSaveResponse();
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            string update1 = "";
            string update2 = "";

            try
            {
                // Accept the change request if the choice is 2
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

                    // Execute the update command
                    await db.ExecuteAsync(update1, parameters, transaction: transaction);
                }

                // Update the request status and other fields for the MapUnitChangeRequest entity
                update2 = @"UPDATE TrnMapUnitChangeRequest set AdminRemark = @AdminRemark, IsComplete = @IsComplete, IsEditAction = @IsEditAction, RequestStatus =@RequestStatus, AdminUpdatedby = @AdminUpdatedby, AdminUpdatedOn = @AdminUpdatedOn, AdminUserId = @AdminUserId  WHERE MapUnitChangeRequestId = @MapUnitChangeRequestId";
                var parameters2 = new DynamicParameters();
                parameters2.Add("@MapUnitChangeRequestId", dTO.MapUnitChangeRequestId, DbType.Int32, ParameterDirection.Input);
                parameters2.Add("@AdminRemark", dTO.AdminRemark, DbType.String, ParameterDirection.Input, 100);
                parameters2.Add("@IsComplete", trnMapUnit.IsComplete, DbType.Boolean, ParameterDirection.Input);
                parameters2.Add("@IsEditAction", trnMapUnit.IsEditAction, DbType.Boolean, ParameterDirection.Input);
                parameters2.Add("@RequestStatus", trnMapUnit.RequestStatus, DbType.Boolean, ParameterDirection.Input);
                parameters2.Add("@AdminUpdatedby", trnMapUnit.AdminUpdatedby, DbType.Int32, ParameterDirection.Input);
                parameters2.Add("@AdminUpdatedOn", trnMapUnit.AdminUpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters2.Add("@AdminUserId", trnMapUnit.AdminUserId, DbType.Int32, ParameterDirection.Input);

                // Execute the second update command
                await db.ExecuteAsync(update2, parameters2, transaction: transaction);

                // Set response data
                saveResponse.Id = dTO.MapUnitChangeRequestId.ToString();
                saveResponse.Message = "Data Updated";

                // Commit the transaction if all operations succeed
                transaction.Commit();
                saveResponse.CurrentTime = trnMapUnit.AdminUpdatedOn ?? DateTime.Now;
                saveResponse.Result = true;
                return saveResponse;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "MapUnitChangeDB->UpdateMapUnitChangeRequest");
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

        /// <summary>
        /// Retrieves the history of unit move requests for a given MapUnitChangeRequestId.
        /// </summary>
        /// <param name="dTO">The DTO containing the details for retrieving the unit move history.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unit move history details.</returns>
        public async Task<DTOMapUnitDetailsResponse> GetUnitMoveHistory(DTOMapUnitDetailsResponse dTO)
        {
            string query = "";
            query = @"Select mrak.RankAbbreviation,up.Name as RequestBy,up.ArmyNo,UnitChReq.Remark,UnitChReq.AdminRemark,UnitChReq.IsComplete,UnitChReq.IsEditAction,UnitChReq.RequestStatus,UnitChReq.AdminUpdatedOn,
                        up_apro.Name as AprovedBy,up_apro.ArmyNo as AproverArmyNo,mrak_apro.RankAbbreviation as AproverRankAbbreviation,
                        mcom.ComdName AS RequestComdName,mcor.CorpsName  As RequestCorpsName,mdiv.DivName as RequestDivName,mbde.BdeName as RequestBdeName,mfmnb.BranchName as RequestBranchName,mpso.PSOName as RequestPSOName,msubd.SubDteName as RequestSubDteName
                        from TrnMapUnitChangeRequest UnitChReq
                        inner join UserProfile up on up.UserId = UnitChReq.FromUserId
                        inner join MRank mrak on mrak.RankId = up.RankId
                        left join UserProfile up_apro on up_apro.UserId = UnitChReq.AdminUserId
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

        /// <summary>
        /// Retrieves a paginated list of unit change requests for the specified parameters.
        /// </summary>
        /// <param name="request">The request object containing parameters for filtering, sorting, and pagination.</param>
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
                                         AdminUpdatedby = unitch.Updatedby,
                                         AdminUpdatedOn = unitch.UpdatedOn,
                                         AdminUserId = unitch.AdminUserId,
                                         UnitName = munit.UnitName,
                                         Sus_no = munit.Sus_no,
                                         Suffix = munit.Suffix,
                                         FromDID = ufrom.DomainId,
                                         FromRankAbbreviation= rkfrom.RankAbbreviation,
                                         FromArmyNo = upfrom.ArmyNo,
                                         FromName = upfrom.Name,
                                     }).AsQueryable();

                // Filter by UnitMapId if the role is not admin
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

                    queryableData = queryableData.Where(x => x.Sus_no.ToLower().Contains(searchValue));
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
                    draw = request.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }
    }
}
