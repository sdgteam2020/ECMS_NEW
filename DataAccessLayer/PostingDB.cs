using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    public class PostingDB : IPostingDB
    {
        protected readonly DapperContext _contextDP;
        private readonly ILogger<PostingDB> _logger;
        private readonly IDataProtector _protector;
        public PostingDB(DapperContext contextDP, ILogger<PostingDB> logger, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) 
        {
            _contextDP = contextDP;
            _logger = logger;
            _protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        /// <summary>
        /// Retrieves posting detail by Id.
        /// </summary>
        /// <param name="Id">The posting Id to retrieve details for.</param>
        /// <returns>A <see cref="DTOPostingOutDetailByIdResponse"/> object containing the posting details.</returns>
        /// <exception cref="Exception">Throws exception if any error occurs while retrieving the posting details.</exception>
        public async Task<DTOPostingOutDetailByIdResponse> GetPostingDetailById(string Id)
        {
            var response = new DTOPostingOutDetailByIdResponse();
            try
            {
                string query = @"select res.Reason,SOSDate,Authority,unit.UnitName ToUnitName,prof.ArmyNo TOArmyNO
	                                ,ranks.RankAbbreviation ToRankName,prof.Name FromName,us.DomainId TODomainId,appt.AppointmentName as ToApptName
                                 from TrnPostingOut pout
                                 inner join MPostingReason res on pout.ReasonId=res.Id
                                 inner join UserProfile prof on prof.UserId=pout.ToUserID
                                 inner join MRank ranks on ranks.RankId=prof.RankId 
                                 inner join MapUnit mapunit on mapunit.UnitMapId=pout.ToUnitID 
                                 inner join MUnit unit on unit.UnitId=mapunit.UnitId 
                                 inner join AspNetUsers us on us.Id=pout.ToAspNetUsersId 
                                 inner join TrnDomainMapping trnd  on trnd.UserId = pout.ToUserID
                                 inner join MAppointment appt on appt.ApptId=trnd.ApptId 
                                 where pout.Id = @Id";
                using (var connection = _contextDP.CreateConnection())
                {
                    response = await connection.QuerySingleAsync<DTOPostingOutDetailByIdResponse>(query, new { Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "PostingDB->GetPostingDetailById");
            }
            return response;
        }

        /// <summary>
        /// Retrieves a list of posting out details with additional filtering and sorting options.
        /// </summary>
        /// <param name="dTO">The data transfer object containing pagination, search, and sorting options.</param>
        /// <param name="AspNetUsersId">The AspNetUsersId to filter the posting history by.</param>
        /// <param name="UnitMapId">The UnitMapId to filter the posting history by.</param>
        /// <param name="Type">The type of posting to filter.</param>
        /// <param name="PostingTy">The posting type, either "PostingIn" or "PostingOut".</param>
        /// <returns>A <see cref="DTODataTablesResponse{DTOPostingOutDetilsResponse}"/> containing the filtered posting details.</returns>
        /// <exception cref="Exception">Throws exception if any error occurs while retrieving the filtered posting out details.</exception>
        public async Task<DTODataTablesResponse<DTOPostingOutDetilsResponse>> GetPostingOutWithType(DTODataTablesRequest dTO, int AspNetUsersId,int UnitMapId,int Type, string PostingTy)
        {
            List<DTOPostingOutDetilsResponse> dTOPostingOutDetilsResponses = new List<DTOPostingOutDetilsResponse>();
            var responseData = new DTODataTablesResponse<DTOPostingOutDetilsResponse>
            {
                draw = dTO.Draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOPostingOutDetilsResponses
            };
            try
            {
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Authority"] = "Authority",
                    ["UpdatedOn"] = "pout.UpdatedOn",
                    ["SOSDate"] = "pout.SOSDate"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "pout.UpdatedOn";

                var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

                string CanAddDispatchDetailQr = @$"{(PostingTy == "PostingIn" ? "0" : "isnull((Select 1 from TrnPostingOut where Id = (Select MAX(Id) from TrnPostingOut where RequestId = pout.RequestId and FromUnitID = pout.FromUnitID) and Id = pout.Id and DispatchedOn is null),0)")}";

                string query = @$"pout.Id,res.Reason,Authority,SOSDate,pout.UpdatedOn,user1.DomainId FromDomainId,user2.DomainId TODomainId,
                                unit1.UnitName FromUnitName,unit2.UnitName ToUnitName,prof1.ArmyNo FromArmyNO,prof2.ArmyNo TOArmyNO,ranks.RankAbbreviation FromRankName,prof1.Name FromName,ISNULL(basic.ServiceNo, basic_2.ServiceNo) AS ServiceNo,basic.FName AS FName_1,basic.LName AS LName_1,basic_2.FName AS FName_2,basic_2.LName AS LName_2,ranksmain.RankAbbreviation Rank 
                                ,user3.DomainId DispatchUpdatedBy,pout.DispatchedOn,pout.DispatchUpdatedOn,pout.RefNo,{CanAddDispatchDetailQr} CanAddDispatchDetail
							    from TrnPostingOut pout
                                inner join MPostingReason res on pout.ReasonId=res.Id 
                                inner join AspNetUsers user1 on user1.Id=pout.FromAspNetUsersId 
                                inner join AspNetUsers user2 on user2.Id=pout.ToAspNetUsersId 
                                LEFT join AspNetUsers user3 on user3.Id=pout.DispatchUpdatedBy
                                inner join MapUnit mapunit1 on mapunit1.UnitMapId=pout.FromUnitID 
                                inner join MUnit unit1 on unit1.UnitId=mapunit1.UnitId 
                                inner join MapUnit mapunit2 on mapunit2.UnitMapId=pout.ToUnitID 
                                inner join MUnit unit2 on unit2.UnitId=mapunit2.UnitId 
                                inner join UserProfile prof1 on prof1.UserId=pout.FromUserID 
                                inner join MRank ranks on ranks.RankId=prof1.RankId 
                                inner join UserProfile prof2 on prof2.UserId=pout.ToUserID
                                inner join TrnICardRequest trnicardr on trnicardr.RequestId=pout.RequestId
                                LEFT join BasicDetails basic on basic.BasicDetailId=trnicardr.BasicDetailId AND basic.ApplyForId = @Type
                                LEFT join AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId=trnicardr.BasicDetailId AND basic_2.ApplyForId = @Type
                                inner join MRank ranksmain on ranksmain.RankId= ISNULL(basic_2.RankId,basic.RankId)
                                where pout.{(PostingTy == "PostingIn" ? "ToUnitID" : "FromUnitID")} = @MapUnitId  AND (@SearchTerm IS NULL OR basic.ServiceNo like @SearchTerm OR basic_2.ServiceNo like @SearchTerm )";

                query = $@"
                            WITH RecordCTE AS (
                                select ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query}
                            )
                            SELECT * FROM RecordCTE
                            WHERE RowNum BETWEEN @Offset AND @Limit;

                            select count(1) from TrnPostingOut pout
                            inner join TrnICardRequest trnicardr on trnicardr.RequestId=pout.RequestId
                            LEFT join BasicDetails basic on basic.BasicDetailId=trnicardr.BasicDetailId AND basic.ApplyForId = @Type
                            LEFT join AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId=trnicardr.BasicDetailId AND basic_2.ApplyForId = @Type
                            where pout.{(PostingTy == "PostingIn" ? "ToUnitID" : "FromUnitID")} = @MapUnitId ;
                        ";
                using (var connection = _contextDP.CreateConnection())
                {
                    // Parameters for SQL query
                    var searchTerm = string.IsNullOrEmpty(dTO.searchValue) ? null : $"%{dTO.searchValue.Trim()}%";

                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", searchTerm, DbType.String, ParameterDirection.Input);
                    parameters.Add("@MapUnitId", UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Type", Type, DbType.Int32, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTOPostingOutDetilsResponse>()).ToList();

                    foreach (var item in records)
                    {
                        item.Id = _protector.Protect(item.Id.ToString());
                        item.FName = item.FName_2 ?? item.FName_1 ?? string.Empty;
                        item.LName = item.LName_2 ?? item.LName_1;
                    }

                    var totalRecords = (await ret.ReadAsync<int>()).Single();
                    responseData.data = records;
                    responseData.draw = dTO.Draw;
                    responseData.recordsTotal = totalRecords;
                    responseData.recordsFiltered = records.Count();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "PostingDB->GetPostingOutWithType");
            }
            return responseData;
        }


        /// <summary>
        /// Retrieves army data for a specific posting request based on the Army Number.
        /// </summary>
        /// <param name="ArmyNo">The Army number to retrieve posting data for.</param>
        /// <returns>A <see cref="DTOPostingInResponse"/> object containing the army data for posting out.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the database query execution.</exception>
        public async Task<DTOPostingInResponse> GetArmyDataForPostingOut(string ArmyNo)
        {
            try
            {

                string query = @"SELECT basi.BasicDetailId,trnicardr.RequestId,basi.FName,basi.LName,basi.ServiceNo,ranks.RankAbbreviation RankName,appl.ApplyForId,appl.Name ApplyFor,
                                trnicardr.StatusId,uplod.PhotoImagePath
                                ,users.DomainId Users_DomainId,pro.ArmyNo Users_ArmyNo,pro.Name Users_Name,ranks1.RankAbbreviation Users_RankName,app.AppointmentName Users_AppointmentName
                                ,muni.UnitName,muni.Suffix,muni.Sus_no,mapunit.UnitMapId FromUnitID,users.Id FromAspNetUsersId,pro.userId FromUserID,
                                COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId
                                from BasicDetails basi
                                inner join TrnICardRequest trnicardr on trnicardr.BasicDetailId=basi.BasicDetailId AND trnicardr.StatusId=1
                                inner join TrnDomainMapping trndom on trndom.id=trnicardr.TrnDomainMappingId
                                inner join MRank ranks on ranks.RankId=basi.RankId
                                inner join MApplyFor appl on appl.ApplyForId=basi.ApplyForId
                                inner join TrnUpload uplod on uplod.BasicDetailId=basi.BasicDetailId
                                inner join AspNetUsers users on users.Id=trndom.AspNetUsersId
                                inner join UserProfile pro on pro.UserId=trndom.UserId
                                inner join MRank ranks1 on ranks1.RankId=pro.RankId
                                inner join MAppointment app on app.ApptId=trndom.ApptId
                                inner join MapUnit mapunit on mapunit.UnitMapId=basi.UnitId
                                inner join MUnit muni on muni.UnitId=mapunit.UnitId
                                left join TrnFwds fwd on fwd.RequestId=trnicardr.RequestId
                                where basi.ServiceNo=@ArmyNo 
                                GROUP BY 
                                    basi.BasicDetailId,
                                    trnicardr.RequestId,
                                    basi.FName,
                                    basi.LName,
                                    basi.ServiceNo,
                                    ranks.RankAbbreviation,
                                    appl.ApplyForId,
                                    appl.Name,
                                    trnicardr.StatusId,
                                    uplod.PhotoImagePath,
                                    users.DomainId,
                                    pro.ArmyNo,
                                    pro.Name,
                                    ranks1.RankAbbreviation,
                                    app.AppointmentName,
                                    muni.UnitName,
                                    muni.Suffix,
                                    muni.Sus_no,
                                    mapunit.UnitMapId,
                                    users.Id,
                                    pro.userId;
                                ";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOPostingInResponse>(query, new { ArmyNo });

                    return ret.FirstOrDefault() ?? new DTOPostingInResponse();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "PostingDB->GetArmyDataForPostingOut");
                return  new DTOPostingInResponse(); 
            }
        }

        /// <summary>
        /// Updates posting details for a specific posting request.
        /// </summary>
        /// <param name="Data">The <see cref="TrnPostingOut"/> data to update in the database.</param>
        /// <returns><c>true</c> if the update was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the update process.</exception>
        public async Task<DTOGenericResponse<int>> UpdateForPosting(TrnPostingOut Data, DTOBeforePostingOutCheckedInputDataResponse closeResponse)
        {
            DTOGenericResponse<int> response = new DTOGenericResponse<int>();
            response.Result = false;
            response.Value = 0;
            // Initialize transaction for multiple database operations
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();

            try
            {
                var sql = @" 
                                SET NOCOUNT ON;

                                DECLARE @NewId TABLE
                                (
                                    Id INT
                                );
                                INSERT INTO TrnPostingOut (ReasonId, Authority, FromAspNetUsersId, FromUnitID, FromUserID, ToAspNetUsersId, ToUnitID, ToUserID, IsActive, UpdatedOn, Updatedby, SOSDate, RequestId, TrnFwdId,DispatchUpdatedBy,DispatchUpdatedOn,DispatchedOn,RefNo)
                                OUTPUT INSERTED.Id INTO @NewId
                                VALUES (@ReasonId, @Authority, @FromAspNetUsersId, @FromUnitID, @FromUserID, @ToAspNetUsersId, @ToUnitID, @ToUserID, @IsActive, @UpdatedOn, @Updatedby, @SOSDate, @RequestId, @TrnFwdId,
                                        CASE
                                            WHEN @DispatchedOn IS NOT NULL
                                            THEN @Updatedby
                                            ELSE NULL
                                        END,

                                        CASE
                                            WHEN @DispatchedOn IS NOT NULL
                                            THEN @UpdatedOn
                                            ELSE NULL
                                        END,

                                        @DispatchedOn,
                                        @RefNo);
                                
                                UPDATE TrnICardRequest SET TrnDomainMappingId = @TrnDomainMappingId WHERE RequestId = @RequestId;
                                UPDATE BasicDetails SET UnitId = @ToUnitID,PlaceOfIssue = @PlaceOfIssue WHERE BasicDetailId = @BasicDetailId;
                                UPDATE TrnFwds SET ToAspNetUsersId = @ToAspNetUsersId WHERE FwdStatusId = 3 AND IsComplete = 0 AND RequestId = @RequestId AND @TrnFwdId IS NOT NULL;

                                SELECT Id
                                FROM @NewId;
                                ";
                var parameters = new DynamicParameters();
                parameters.Add("@ReasonId", Data.ReasonId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@Authority", Data.Authority, DbType.String, ParameterDirection.Input,50);
                parameters.Add("@FromAspNetUsersId", Data.FromAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FromUnitID", Data.FromUnitID, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FromUserID", Data.FromUserID, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToAspNetUsersId", Data.ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToUnitID", Data.ToUnitID, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToUserID", Data.ToUserID, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@SOSDate", Data.SOSDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@TrnFwdId", Data.TrnFwdId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@BasicDetailId", closeResponse.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@TrnDomainMappingId", closeResponse.TrnDomainMappingId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@PlaceOfIssue", closeResponse.PlaceOfIssue, DbType.AnsiString, ParameterDirection.Input,50);
                parameters.Add("@DispatchedOn", Data.DispatchedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@RefNo", Data.RefNo, DbType.String, ParameterDirection.Input);

                // Insert the new posting record and get its ID
                var Id = await db.QuerySingleAsync<int>(sql, parameters, transaction:transaction);

                
                // Commit the transaction if all operations succeed
                transaction.Commit();
                
                response.Result = true;
                response.Message = "Posting Out successfully";
                response.Value = Id;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "PostingDB->UpdateForPosting");
                response.Message = "Failed to update posting details.";
            }
            finally
            {
                transaction.Dispose();
                db.Dispose();
            }
            return response;
        }


        /// <summary>
        /// Retrieves a list of closed application records for a specific unit map and apply type.
        /// </summary>
        /// <param name="UnitMapId">The unit map ID to filter the closed applications by.</param>
        /// <param name="apply">The apply type ID to filter the closed applications by.</param>
        /// <returns>A list of <see cref="DTOAppClosedListResponse"/> objects representing the closed applications.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs while retrieving the closed applications.</exception>
        public async Task<DTODataTablesResponse<DTOAppClosedListResponse>> GetAppClosedList(DTODataTableRequestForAppCloseList dTO)
        {
            List<DTOAppClosedListResponse> dTOAppCloseds = new List<DTOAppClosedListResponse>();
            var responseData = new DTODataTablesResponse<DTOAppClosedListResponse>
            {
                draw = dTO.Draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOAppCloseds
            };

            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "basic_2.ServiceNo",
                    ["UpdatedOn"] = "appcl.UpdatedOn",
                    ["Authority"] = "appcl.Authority",
                    ["Remarks"] = "appcl.Remarks"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "") ? allowedSortColumns[dTO.sortColumn!] : "appcl.UpdatedOn";

                var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";


                string selectFields = @"appcl.UpdatedOn,basic_2.ServiceNo,mr.RankAbbreviation as RankName,basic_2.FName,basic_2.LName,mpr.Reason,mappl.Name as ApplyFor,appcl.Remarks,appcl.Authority";
                string fromJoinClause = @"from TrnApplClose appcl
                                        inner join TrnICardRequest trnicardr on trnicardr.RequestId=appcl.RequestId
                                        inner join AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId=trnicardr.BasicDetailId and basic_2.UnitId =@UnitMapId
                                        inner join MRank mr on mr.RankId = basic_2.RankId
                                        inner join MApplyFor mappl on mappl.ApplyForId = basic_2.ApplyForId
                                        inner join MPostingReason mpr on mpr.Id= appcl.ReasonId";
                string whereClause = @"where
                                       mappl.ApplyForId=@apply
                                       AND (
                                            @SearchTerm IS NULL OR 
                                            basic_2.ServiceNo LIKE @SearchTerm OR
                                            appcl.Authority LIKE @SearchTerm
                                        )";

                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";


                using (var connection = _contextDP.CreateConnection())
                {
                    // Parameters for SQL query
                    var searchTerm = string.IsNullOrEmpty(dTO.searchValue) ? null : $"%{dTO.searchValue.Trim()}%";

                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", searchTerm, DbType.String, ParameterDirection.Input);
                    parameters.Add("@UnitMapId", dTO.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@apply", dTO.apply, DbType.Int32, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTOAppClosedListResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    responseData = new DTODataTablesResponse<DTOAppClosedListResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = records,
                    };

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "PostingDB->GetAppClosedList");
            }
            return responseData;
        }
        public async Task<DTOBeforePostingOutCheckedInputDataResponse> BeforePostingOutCheckedInputData(TrnPostingOut trnPostingOut)
        {
            DTOBeforePostingOutCheckedInputDataResponse dTOBeforePostingOut = new DTOBeforePostingOutCheckedInputDataResponse();

            string query = @"Select req.BasicDetailId,req.StatusId,munit.Abbreviation as PlaceOfIssue,tdm.Id as TrnDomainMappingId,basi.UnitId,tdm.AspNetUsersId as ToAspNetUsersId, tdm.UserId as ToUserID,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId from TrnICardRequest req
                            LEFT JOIN BasicDetails basi on basi.BasicDetailId = req.BasicDetailId
                            LEFT JOIN TrnDomainMapping tdm on tdm.AspNetUsersId = @ToAspNetUsersId and tdm.UnitId =@ToUnitID and tdm.UserId =@ToUserID
                            LEFT JOIN MapUnit mapunit on mapunit.UnitMapId = @ToUnitID
                            LEFT JOIN MUnit munit on munit.UnitId = mapunit.UnitId
                            LEFT JOIN TrnFwds fwd on fwd.RequestId = req.RequestId
                            where req.RequestId=@RequestId
                            GROUP BY 
                                req.BasicDetailId,
                                req.StatusId,
                                munit.Abbreviation,
                                tdm.Id,
                                basi.UnitId,
                                tdm.AspNetUsersId,
                                tdm.UserId";

            using (var connection = _contextDP.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RequestId", trnPostingOut.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToUnitID", trnPostingOut.ToUnitID, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToAspNetUsersId", trnPostingOut.ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToUserID", trnPostingOut.ToUserID, DbType.Int32, ParameterDirection.Input);


                dTOBeforePostingOut = await connection.QueryFirstAsync<DTOBeforePostingOutCheckedInputDataResponse>(query, parameters);

                if (dTOBeforePostingOut != null)
                {
                    if (dTOBeforePostingOut.StatusId == 1 && dTOBeforePostingOut.UnitId == trnPostingOut.FromUnitID && dTOBeforePostingOut.UnitId != trnPostingOut.ToUnitID && trnPostingOut.TrnFwdId == dTOBeforePostingOut.MaxTrnFwdId &&  dTOBeforePostingOut.ToAspNetUsersId != null && dTOBeforePostingOut.ToUserID != null)
                    {
                        dTOBeforePostingOut.Result = true;
                        dTOBeforePostingOut.Message = "Ok";
                        return dTOBeforePostingOut;
                    }
                    else
                    {
                        if (dTOBeforePostingOut.StatusId != 1)
                        {
                            dTOBeforePostingOut.Message = "Appl Allready Complete / Closed!";
                        }
                        else if (dTOBeforePostingOut.UnitId != trnPostingOut.FromUnitID)
                        {
                            dTOBeforePostingOut.Message = "You are not authorized to postingout this request.";
                        }
                        else if (dTOBeforePostingOut.UnitId == trnPostingOut.ToUnitID)
                        {
                            dTOBeforePostingOut.Message = "The source unit and the destination unit are not the same.";
                        }
                        else if (dTOBeforePostingOut.MaxTrnFwdId != trnPostingOut.TrnFwdId)
                        {
                            dTOBeforePostingOut.Message = "Invalid Movement ID.";
                        }
                        else if (dTOBeforePostingOut.ToAspNetUsersId == null)
                        {
                            dTOBeforePostingOut.Message = "The Receiver ID is invalid.";
                        }
                        else if (dTOBeforePostingOut.ToUserID == null)
                        {
                            dTOBeforePostingOut.Message = "Invalid User ID.";
                        }
                        dTOBeforePostingOut.Result = false;
                        return dTOBeforePostingOut;
                    }
                }
                else
                {
                    dTOBeforePostingOut.Result = false;
                    dTOBeforePostingOut.Message = "Invalid Input";
                    return dTOBeforePostingOut;
                }
            }
        }
    }
}
