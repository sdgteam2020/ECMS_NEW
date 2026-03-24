using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    public class HomeDB : IHomeDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<HomeDB> _logger;
        public HomeDB(ApplicationDbContext context,DapperContext contextDP, ILogger<HomeDB> logger)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }


        /// <summary>
        /// Asynchronously retrieves task board counts based on the provided parameters, including counts for dispatch cards, faulty cards, unit change requests, and more.
        /// The method executes a complex SQL query with conditional logic based on the <paramref name="Claim"/> parameter, returning a <see cref="DTOTaskCountResponse"/> with the results.
        /// </summary>
        /// <param name="MapUnitId">The unit map ID for filtering the results related to the unit.</param>
        /// <param name="Claim">The claim type used to conditionally modify the query (e.g., 1 for general, 2 for specific record office, etc.).</param>
        /// <param name="TDM_Id">The TDM ID used for filtering based on the record office mapping when <paramref name="Claim"/> equals 2.</param>
        /// <returns>A <see cref="DTOTaskCountResponse"/> containing counts for various task categories such as dispatch cards, hotlist cards, and unit change requests.</returns>
        /// <exception cref="Exception">Throws an exception if there is an error during query execution.</exception>
        public async Task<DTOTaskCountResponse> GetTaskBoardCount(int MapUnitId, byte Claim, int TDM_Id)
        {
            // SQL query to get task board counts for various categories
            string query = @"declare @TotHotlistCards int=0 declare @TotMisprintedCard int=0 declare @TotUnitChangeRequest int=0 declare @TotDistCards int=0 declare @TotDestCards int=0 declare @TotDispatchCards int=0 declare @TotLostCards int=0

                            BEGIN 
                            IF @Claim=1
                            BEGIN 
                            Select @TotDispatchCards=COUNT(disca.DispatchCardId) from TrnDispatchCard disca
                            WHERE disca.Step=1
                            END 
                            ELSE IF @Claim=2 
                            BEGIN 
                            Select @TotDispatchCards=COUNT(disca.DispatchCardId) from TrnDispatchCard disca
                            INNER JOIN MRecordOffice mrec ON disca.RecordOfficeId = mrec.RecordOfficeId
                            INNER JOIN OROMapping oro on  mrec.RecordOfficeId =oro.RecordOfficeId
                            WHERE oro.TDMId=@TDM_Id
                            END 
                            ELSE IF @Claim=3
                            BEGIN 
                            Select @TotDispatchCards=COUNT(disca.DispatchCardId) from TrnDispatchCard disca
                            INNER JOIN MRegimental regi on disca.RegId=regi.RegId
                            WHERE regi.UnitId=@MapUnitId
                            END 
                            ELSE 
                            BEGIN 
                            Select @TotDispatchCards=COUNT(disca.DispatchCardId) from TrnDispatchCard disca
                            WHERE disca.ToUnitId=@MapUnitId and disca.Step=2
                            END 
                            END 

                            select @TotMisprintedCard=COUNT(TrnFaultyCardId) from TrnFaultyCard flt
                            INNER JOIN TrnICardRequest  trnicard on flt.RequestId = trnicard.RequestId
                            INNER JOIN BasicDetails bd on trnicard.BasicDetailId = bd.BasicDetailId
                            INNER JOIN  MapUnit munit on bd.UnitId = munit.UnitMapId
                            where (@Claim = 1 OR (munit.UnitMapId=@MapUnitId))

                            select @TotUnitChangeRequest=COUNT(MapUnitChangeRequestId) from TrnMapUnitChangeRequest
                            where UnitMapId=@MapUnitId

                            select @TotLostCards=COUNT(LostCardId) from TrnLostCards
                            select @TotHotlistCards=COUNT(HotlistCardId) from TrnHotlistCards
                            select @TotDestCards=COUNT(DestructedCardId) from TrnDestructionCards                            
                            
                            select @TotDistCards=COUNT(dist.DistributeCardId) from TrnDistributeCards dist
                            INNER JOIN TrnICardRequest  trnicard on dist.RequestId = trnicard.RequestId
                            INNER JOIN BasicDetails bd on trnicard.BasicDetailId = bd.BasicDetailId
                            WHERE bd.UnitId=@MapUnitId
                            

                            select @TotLostCards TotLostCards,@TotHotlistCards TotHotlistCards,@TotUnitChangeRequest TotUnitChangeRequest,@TotMisprintedCard TotMisprintedCard,@TotDistCards TotDistCards,@TotDestCards TotDestCards,@TotDispatchCards TotDispatchCards";

            try
            {
                // Use the connection to execute the query and return the first result
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MapUnitId", MapUnitId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TDM_Id", TDM_Id, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Claim", Claim, DbType.Byte, ParameterDirection.Input);

                    // Execute the query asynchronously and map the result to the response DTO
                    var ret = (await connection.QueryAsync<DTOTaskCountResponse>(query, parameters)).FirstOrDefault();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                // Log the exception if there is an error during the query execution
                _logger.LogError(1001, ex, "HomeDB->GetDashBoardCount");
                return null;
            }
        }


        /// <summary>
        /// Asynchronously retrieves dashboard count data based on user, application forward condition, and armed ID for ORO. 
        /// It performs complex SQL queries with conditional logic based on the user’s role and filtering conditions for requests, lost cards, 
        /// inaccurate data, and observations raised. Returns a <see cref="DTODashboardCountResponse"/> containing the task counts.
        /// </summary>
        /// <param name="UserId">The user ID for filtering the task board data.</param>
        /// <param name="dTOApplFwdCondition">The data transfer object containing the application forward condition, including details for filtering by ArmedAbbreviation, RankOrderby, etc.</param>
        /// <param name="ArmedIdForORO">The Armed ID used for filtering in specific ORO mappings.</param>
        /// <returns>A <see cref="DTODashboardCountResponse"/> containing counts for tasks such as requests, lost cards, and observations raised.</returns>
        /// <exception cref="Exception">Throws an exception if there is an error during query execution.</exception>
        public async Task<DTODashboardCountResponse> GetDashBoardCount(int AspNetUsersId, bool claim)
        {
            string query;
            string query2;
            try
            {
                if (claim)
                {
                    query2 = @"SELECT 
                                        CASE 
                                            WHEN RECO.TDMId IS NOT NULL THEN RECO.TDMId
                                            ELSE ORO.TDMId
                                        END AS TDMId,
                                        TDM.AspNetUsersId,
                                        RECO.RecordOfficeId,
                                        RECO.Name,
                                        RECO.ArmedId
                                    FROM MRecordOffice RECO
                                    LEFT JOIN OROMapping ORO 
                                        ON RECO.RecordOfficeId = ORO.RecordOfficeId
                                    LEFT JOIN TrnDomainMapping TDM
                                        ON TDM.Id = CASE 
                                                        WHEN RECO.TDMId IS NOT NULL THEN RECO.TDMId
                                                        ELSE ORO.TDMId
                                                    END
                                    WHERE TDM.AspNetUsersId = @AspNetUsersId";

                    query = @"declare @TotReq int=0 
                            declare @TotInaccurateData int=0
                            declare @TotObservationRaised int=0
                            select @TotReq=COUNT(distinct req.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id
                            where domain.AspNetUsersId=@AspNetUsersId


                            select @TotInaccurateData=COUNT(BasicDetailTempId) from BasicDetailTemps
                            where Updatedby=@AspNetUsersId AND IsActive=1 

                            select @TotObservationRaised=COUNT(BasicDetailTempId) from BasicDetailTemps
                            where RecordOfficeId=@RecordOfficeId AND IsActive=1 

                            select @TotReq TotReq,@TotInaccurateData TotInaccurateData,@TotObservationRaised TotObservationRaised";
                    
                    // Create a connection to the database and execute the query
                    using (var connection = _contextDP.CreateConnection())
                    {
                        byte RecordOfficeId=0;
                        DTOBasicDetailTempObsRequest? dTOBasicDetailTempObsRequest = await connection.QueryFirstOrDefaultAsync<DTOBasicDetailTempObsRequest>(query2, new { AspNetUsersId });
                        if (dTOBasicDetailTempObsRequest == null)
                        {
                            RecordOfficeId = 0;

                        }
                        else
                        {
                            RecordOfficeId = dTOBasicDetailTempObsRequest.RecordOfficeId;
                        }
                        var parameters = new DynamicParameters();
                        parameters.Add("@AspNetUsersId", AspNetUsersId);
                        parameters.Add("@RecordOfficeId", RecordOfficeId);

                        // Execute the query and retrieve the result as a single response
                        var ret = await connection.QueryFirstOrDefaultAsync<DTODashboardCountResponse>(query, parameters);
                        return ret ?? new DTODashboardCountResponse();

                    }
                }
                else
                {
                    query = @"declare @TotReq int=0 
                            declare @TotInaccurateData int=0
                            declare @TotObservationRaised int=0
                            SET @TotObservationRaised = 0;
                            
                            select @TotReq=COUNT(distinct req.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id
                            where domain.AspNetUsersId=@AspNetUsersId


                            select @TotInaccurateData=COUNT(BasicDetailTempId) from BasicDetailTemps
                            where Updatedby=@AspNetUsersId AND IsActive=1 

                            select @TotReq TotReq,@TotInaccurateData TotInaccurateData,@TotObservationRaised TotObservationRaised";

                    // Create a connection to the database and execute the query
                    using (var connection = _contextDP.CreateConnection())
                    {
                        // Execute the query and retrieve the result as a single response
                        var ret = await connection.QueryFirstOrDefaultAsync<DTODashboardCountResponse>(query, new { AspNetUsersId });
                        return ret ?? new DTODashboardCountResponse();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HomeDB->GetDashBoardCount");
                return new DTODashboardCountResponse();
            }
        }


        /// <summary>
        /// Retrieves the count of various request categories (Posting Out, Posting In) based on the provided type for a specific user and unit.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user for whom the request counts are to be retrieved.</param>
        /// <param name="Type">The type of request to filter by, which can be either "Posting Out" or "Posting In".</param>
        /// <param name="UnitMapId">The unique identifier of the unit mapping to filter the request counts.</param>
        /// <returns>
        /// A <see cref="DTORequestDashboardCountResponse"/> object containing the counts for the selected request type (either "Posting Out" or "Posting In").
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is any error during the database query execution.</exception>
        public async Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId,string Type,int UnitMapId)
        {
            string query="";
            // Build the query based on the Type parameter to get counts for various request categories
            switch (Type)
            {
                case "Posting Out":
                    query = @"declare @ToPostingOutOffrs int=0 declare @ToPostingOutJCO int=0  
                            select @ToPostingOutOffrs=COUNT(distinct pout.Id) from TrnPostingOut pout
                            inner join BasicDetails basic on basic.BasicDetailId=pout.BasicDetailId where pout.FromUnitId=@UnitMapId and basic.ApplyForId=1 
                          
                            select @ToPostingOutJCO=COUNT(distinct pout.Id) from TrnPostingOut pout
                            inner join BasicDetails basic on basic.BasicDetailId=pout.BasicDetailId where pout.FromUnitId=@UnitMapId and basic.ApplyForId=2 
                            select @ToPostingOutOffrs ToPostingOutOffrs,@ToPostingOutJCO ToPostingOutJCO";
                    break;
                case "Posting In":
                    query = @"declare @ToPostingInOffrs int=0 declare @ToPostingInJCO int=0 
                            select @ToPostingInOffrs=COUNT(distinct pout.Id) from TrnPostingOut pout 
                            inner join BasicDetails basic on basic.BasicDetailId=pout.BasicDetailId where pout.ToUnitId=@UnitMapId and basic.ApplyForId=1 
                          
                            select @ToPostingInJCO=COUNT(distinct pout.Id) from TrnPostingOut pout 
                            inner join BasicDetails basic on basic.BasicDetailId=pout.BasicDetailId where pout.ToUnitId=@UnitMapId and basic.ApplyForId=2 
                            select @ToPostingInOffrs ToPostingInOffrs,@ToPostingInJCO ToPostingInJCO";
                    break;
            }

            try
            {
                // Create a connection to the database and execute the query asynchronously
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query and map the result to DTORequestDashboardCountResponse
                    var ret = await connection.QueryAsync<DTORequestDashboardCountResponse>(query, new { UserId, UnitMapId });
                    return ret.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the query execution
                _logger.LogError(1001, ex, "HomeDB->GetRequestDashboardCount");
                return null;
            }
        }


        /// <summary>
        /// Retrieves the count of various request categories (Drafted, Submitted, Rejected, Closed, Completed) 
        /// for a specific user and unit mapping from the database.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user for whom the request counts are to be retrieved.</param>
        /// <param name="UnitMapId">The unique identifier of the unit mapping for which the counts are required.</param>
        /// <returns>
        /// A <see cref="DTORequestSubDashboardCountResponse"/> object containing the counts for the following categories:
        /// - Drafted (for Officers and JCOs)
        /// - Submitted (for Officers and JCOs)
        /// - Closed (for Officers and JCOs)
        /// - Completed (for Officers and JCOs)
        /// - Rejected (for Officers and JCOs)
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is any error during the database query execution.</exception>
        public async Task<DTORequestSubDashboardCountResponse> GetSubDashboardCount(int UserId,int UnitMapId)
        {
            // SQL query to calculate counts for various request categories (Drafted, Submitted, Rejected, Closed, Completed)
            string query = @"
                            --Drafted--
                            declare @ToDraftedOffrs int=0 declare @ToDraftedJCO int=0
                            select @ToDraftedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and req.StatusId=1 and trnstepcout.ApplyForId=1 
                          
                            select @ToDraftedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and req.StatusId=1 and trnstepcout.ApplyForId=2 

                            --Submitted--
                            declare @ToSubmittedOffrs int=0 declare @ToSubmittedJCO int=0
                            select @ToSubmittedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>1 and trnstepcout.ApplyForId=1 
                          
                            select @ToSubmittedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>1 and trnstepcout.ApplyForId=2 

                            --Closed--
                            declare @ToClosedOffrs int=0 declare @ToClosedJCO int=0
                            select @ToClosedOffrs=COUNT(appcl.Id) from TrnApplClose appcl
                            inner join BasicDetails bs on bs.BasicDetailId=appcl.BasicDetailId and bs.UnitId =@UnitMapId 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= appcl.RequestId and trnstepcout.ApplyForId=1

                            select @ToClosedJCO=COUNT(appcl.Id) from TrnApplClose appcl
                            inner join BasicDetails bs on bs.BasicDetailId=appcl.BasicDetailId and bs.UnitId =@UnitMapId 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= appcl.RequestId and trnstepcout.ApplyForId=2 

                            --Completed--
                            declare @ToCompletedOffrs int=0 declare @ToCompletedJCO int=0
                            select @ToCompletedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and req.StatusId=2 and trnstepcout.ApplyForId=1 
                          
                            select @ToCompletedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and req.StatusId=2 and trnstepcout.ApplyForId=2 

                            --Rejected--
                            declare @ToRejectedOffrs int=0 declare @ToRejectedJCO int=0
                            select @ToRejectedOffrs=COUNT(distinct fwd.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId
                            inner join TrnFwds fwd on fwd.RequestId= trnstepcout.RequestId where fwd.ToAspNetUsersId=@UserId and req.StatusId=1 and trnstepcout.StepId in(7,8,9,10) and trnstepcout.ApplyForId=1 and fwd.FwdStatusId=3 

                            select @ToRejectedJCO=COUNT(distinct fwd.RequestId) from TrnDomainMapping domain
                            inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id 
                            inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId
                            inner join TrnFwds fwd on fwd.RequestId= trnstepcout.RequestId where fwd.ToAspNetUsersId=@UserId and req.StatusId=1 and trnstepcout.StepId in(7,8,9,10) and trnstepcout.ApplyForId=2 and fwd.FwdStatusId=3 

                            select @ToDraftedOffrs ToDraftedOffrs,@ToDraftedJCO ToDraftedJCO ,@ToSubmittedOffrs ToSubmittedOffrs,@ToSubmittedJCO ToSubmittedJCO,@ToClosedOffrs ToClosedOffrs,@ToClosedJCO ToClosedJCO ,@ToCompletedOffrs ToCompletedOffrs,@ToCompletedJCO ToCompletedJCO,@ToRejectedOffrs ToRejectedOffrs,@ToRejectedJCO ToRejectedJCO";
            try
            {
                // Create a connection to the database and execute the query asynchronously
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query and map the result to DTORequestSubDashboardCountResponse
                    var ret = await connection.QueryAsync<DTORequestSubDashboardCountResponse>(query, new { UserId, UnitMapId });
                    return ret.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the query execution
                _logger.LogError(1001, ex, "HomeDB->GetRequestDashboardCount");
                return null;
            }
        }


        /// <summary>
        /// Asynchronously retrieves all registered users for a specific unit based on the provided unit ID.
        /// The method fetches user details such as domain ID, appointment name, rank, army number, and user name 
        /// by joining multiple tables: `TrnDomainMapping`, `Users`, `MAppointment`, and `UserProfile`.
        /// </summary>
        /// <param name="UnitId">The unit ID used to filter the registered users for a specific unit.</param>
        /// <returns>A list of <see cref="DTORegisterUserResponse"/> objects containing the details of all registered users for the given unit.</returns>
        public async Task<List<DTORegisterUserResponse>> GetAllRegisterUser(int UnitId)
        {
            // Perform an asynchronous query to retrieve the user details by joining multiple tables
            var allrecord = await (from tdm in _context.TrnDomainMapping.Where(x=>x.UnitId == UnitId)
                                   join u in _context.Users on tdm.AspNetUsersId  equals u.Id
                                   join app in _context.MAppointment on tdm.ApptId equals app.ApptId
                                   join up in _context.UserProfile on tdm.UserId equals up.UserId into tdmup_jointable
                                   from xup in tdmup_jointable.DefaultIfEmpty()

                                   select new DTORegisterUserResponse()
                                   {
                                       DomainId = u.DomainId,
                                       AppointmentName = app.AppointmentName,
                                       ArmyNo = xup!=null ? xup.ArmyNo:null,
                                       Rank = tdm.UserId!=null ? (from r in _context.MRank.Where(x=>x.RankId == xup.RankId) select r.RankName).FirstOrDefault():null,
                                       Name = xup != null ? xup.Name : null,
                                   }).ToListAsync();
            return allrecord; // Return the list of registered user responses
        }


        /// <summary>
        /// Asynchronously retrieves dashboard count data for user management in a specific unit.
        /// The method calculates counts for registered users, posting-in, and posting-out records based on the provided unit ID.
        /// </summary>
        /// <param name="UnitId">The unit ID for filtering the records related to a specific unit.</param>
        /// <param name="UserId">The user ID used for filtering user-related data (though it is not currently used in the query).</param>
        /// <returns>A <see cref="DTORequestDashboardUserMgtCountResponse"/> containing counts for registered users, posting-in, and posting-out records.</returns>
        /// <exception cref="Exception">Throws an exception if there is an error during query execution.</exception>
        public async Task<DTORequestDashboardUserMgtCountResponse> GetDashboardUserMgtCount(int UnitId, int UserId)
        {
            // SQL query to calculate counts for registered users, posting-in, and posting-out records
            string query = "declare @TotRegisterUser int=0 declare @TotPostingIn int=0 declare @TotPostingOut int=0 " +
                            " select @TotRegisterUser=COUNT(Id) from TrnDomainMapping where UnitId=@UnitId " +
                            " select @TotPostingIn=COUNT(Id) from TrnPostingOut where ToUnitId=@UnitId " +
                            " select @TotPostingOut=COUNT(Id) from TrnPostingOut where FromUnitId=@UnitId " +
                            " select @TotRegisterUser TotRegisterUser,@TotPostingIn TotPostingIn,@TotPostingOut TotPostingOut";
            try
            {
                // Open a database connection and execute the query asynchronously
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query and map the result to DTORequestDashboardUserMgtCountResponse
                    var ret = await connection.QueryAsync<DTORequestDashboardUserMgtCountResponse>(query, new { UnitId, UserId });
                    return ret.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log the error if any exception occurs during query execution
                _logger.LogError(1001, ex, "HomeDB->GetRequestDashboardCount");
                return null;
            }
        }
    }
}
