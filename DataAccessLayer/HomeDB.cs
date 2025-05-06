using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class HomeDB : IHomeDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<HomeDB> _logger;
        public HomeDB(ApplicationDbContext context,DapperContext contextDP, ILogger<HomeDB> logger)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        public async Task<DTODashboardCountResponse> GetDashBoardCount(int UserId, DTOApplFwdConditionRequest dTOApplFwdCondition, short ArmedIdForORO, int MapUnitId, bool Claim)
        {
            string query = "declare @TotReq int=0 declare @TotInaccurateData int=0 declare @TotLostCards int=0 declare @TotHotlistCards int=0 declare @TotMisprintedCard int=0 declare @TotUnitChangeRequest int=0 " +
                            " select @TotReq=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id" +
                            " where domain.AspNetUsersId=@UserId" +

                            " select @TotMisprintedCard=COUNT(TrnFaultyCardId) from TrnFaultyCard" +
                            " where (@Claim = 1 OR (Updatedby=@UserId))" +

                            " select @TotUnitChangeRequest=COUNT(MapUnitChangeRequestId) from TrnMapUnitChangeRequest" +
                            " where UnitMapId=@MapUnitId" +

                            " select @TotInaccurateData=COUNT(BasicDetailTempId) from BasicDetailTemps" +
                            " where Updatedby=@UserId AND IsActive=1 " +

                            " select @TotLostCards=COUNT(LostCardId) from TrnLostCards" +
                            " select @TotHotlistCards=COUNT(HotlistCardId) from TrnHotlistCards" +

                            " declare @AspNetUsersId int=0 declare @ArmedId int=0 declare @Name varchar(25) declare @TotObservationRaised int=0 declare @TDMId int=0  " +
                            " SELECT CASE WHEN ISNULL(RECO.TDMId,0) >0 THEN RECO.TDMId ELSE ORO.TDMId END TDMId, " +
                            " (select AspNetUsersId from TrnDomainMapping where id =(CASE WHEN ISNULL(RECO.TDMId,0) >0 THEN RECO.TDMId ELSE ORO.TDMId END )) as AspNetUsersId,RECO.Name,RECO.ArmedId  " +
                            " into #temp " +
                            " FROM MRecordOffice RECO " +
                            " LEFT JOIN OROMapping ORO ON RECO.RecordOfficeId=ORO.RecordOfficeId " +
                            " SELECT  @TDMId=TDMId, @AspNetUsersId=AspNetUsersId,@Name=Name,@ArmedId=ArmedId from #temp where AspNetUsersId=@UserId " +
                            " drop table #temp " +
                            " IF @AspNetUsersId=NULL " +
                            " BEGIN " +
                            " SET @TotObservationRaised=0 " +
                            " END " +
                            " ELSE IF @ArmedId != @ArmedIdForORO " +
                            " BEGIN " +
                            " SELECT @TotObservationRaised=COUNT(tdm.Id) FROM TrnDomainMapping tdm " +
                            " inner join MRecordOffice mrec on mrec.TDMId = tdm.Id " +
                            " inner join BasicDetailTemps Temps on Temps.ArmedId = mrec.ArmedId " +
                            " WHERE tdm.AspNetUsersId=@UserId AND Temps.ApplyForId = 2 AND Temps.IsActive = 1 " +
                            " END " +
                            " ELSE " +
                            " BEGIN " +
                            " IF @Name=@MPRSO_Name " +
                            " BEGIN " +
                            " SELECT @TotObservationRaised=COUNT(Temps.BasicDetailTempId)  FROM BasicDetailTemps Temps " +
                            " inner join MArmedType at on at.ArmedId = Temps.ArmedId " +
                            " left join OROMapping oro on oro.TDMId = @TDMId " +
                            " WHERE Temps.ApplyForId=1 AND Temps.IsActive=1 AND at.Abbreviation in @MPRSO_ArmedAbbreviation " +
                            " END " +
                            " ELSE IF @Name=@MP6A_Name " +
                            " BEGIN " +
                            " SELECT @TotObservationRaised=COUNT(Temps.BasicDetailTempId) FROM BasicDetailTemps Temps " +
                            " inner join MRank ranks1 on ranks1.RankId = Temps.RankId " +
                            " WHERE Temps.ApplyForId=1 AND ranks1.Orderby <=@MP6A_RankOrderby AND SUBSTRING(UPPER(Temps.ServiceNo),1,2) != @MP6F_ArmyNoPrefix  AND Temps.IsActive=1 " +
                            " END " +
                            " ELSE IF @Name=@MP6F_Name " +
                            " BEGIN " +
                            " SELECT @TotObservationRaised=COUNT(Temps.BasicDetailTempId)  FROM BasicDetailTemps Temps " +
                            " left join OROMapping oro on oro.TDMId = @TDMId " +
                            " WHERE Temps.ApplyForId=1 AND SUBSTRING(UPPER(Temps.ServiceNo),1,2) = @MP6F_ArmyNoPrefix OR Temps.ArmedId in (select value from string_split(oro.ArmedIdList,','))  AND Temps.IsActive=1 " +
                            " END " +
                            " ELSE " +
                            " BEGIN " +
                            " SELECT @TotObservationRaised=COUNT(Temps.BasicDetailTempId)  FROM BasicDetailTemps Temps " +
                            " inner join MRank ranks1 on ranks1.RankId = Temps.RankId " +
                            " left join OROMapping oro on oro.TDMId = @TDMId " +
                            " WHERE Temps.ApplyForId=1 AND ranks1.Orderby > @MP6A_RankOrderby AND SUBSTRING(UPPER(Temps.ServiceNo),1,2) != @MP6F_ArmyNoPrefix AND Temps.ArmedId in (select value from string_split(oro.ArmedIdList,','))  AND Temps.IsActive=1 " +
                            " END " +
                            " END " +
                            " select @TotReq TotReq,@TotInaccurateData TotInaccurateData,@TotObservationRaised TotObservationRaised,@TotLostCards TotLostCards,@TotHotlistCards TotHotlistCards,@TotUnitChangeRequest TotUnitChangeRequest,@TotMisprintedCard TotMisprintedCard ";

            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", UserId);
                    parameters.Add("@ArmedIdForORO", ArmedIdForORO);
                    
                    parameters.Add("@MPRSO_ArmedAbbreviation", dTOApplFwdCondition.MPRSO.ArmedAbbreviation);
                    parameters.Add("@MPRSO_Name", dTOApplFwdCondition.MPRSO.Name);

                    parameters.Add("@MP6F_ArmyNoPrefix", dTOApplFwdCondition.MP6F.ArmyNoPrefix);
                    parameters.Add("@MP6F_Name", dTOApplFwdCondition.MP6F.Name);

                    parameters.Add("@MP6A_RankOrderby", dTOApplFwdCondition.MP6A.RankOrderby);
                    parameters.Add("@MP6A_Name", dTOApplFwdCondition.MP6A.Name);

                    parameters.Add("@MapUnitId", MapUnitId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Claim", Claim, DbType.Boolean, ParameterDirection.Input);

                    var ret = await connection.QueryAsync<DTODashboardCountResponse>(query, parameters);
                    return ret.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HomeDB->GetDashBoardCount");
                return null;
            }
        }
        public async Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId,int UnitId,string Type)
        {
            string query="";
            switch (Type)
            {
                case "Drafted":
                    query = "declare @ToDraftedOffrs int=0 declare @ToDraftedJCO int=0" +
                            " select @ToDraftedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and req.StatusId=1 and trnstepcout.ApplyForId=1 " +
                            
                            " select @ToDraftedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and req.StatusId=1 and trnstepcout.ApplyForId=2 " +
                            " select @ToDraftedOffrs ToDraftedOffrs,@ToDraftedJCO ToDraftedJCO";
                    break;
                case "Submitted":
                    query = "declare @ToSubmittedOffrs int=0 declare @ToSubmittedJCO int=0" +
                            " select @ToSubmittedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>1 and trnstepcout.ApplyForId=1 " +
                            
                            " select @ToSubmittedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>1 and trnstepcout.ApplyForId=2 " +
                            " select @ToSubmittedOffrs ToSubmittedOffrs,@ToSubmittedJCO ToSubmittedJCO";
                    break;
                case "Closed":
                    query = "declare @ToClosedOffrs int=0 declare @ToClosedJCO int=0" +
                            " select @ToClosedOffrs=COUNT(appcl.Id) from TrnApplClose appcl" +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= appcl.RequestId where appcl.Updatedby=@UserId and trnstepcout.ApplyForId=1 " +

                            " select @ToClosedJCO=COUNT(appcl.Id) from TrnApplClose appcl" +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= appcl.RequestId where appcl.Updatedby=@UserId and trnstepcout.ApplyForId=2 " +
                            " select @ToClosedOffrs ToClosedOffrs,@ToClosedJCO ToClosedJCO";
                    break;
                case "Completed":
                    query = "declare @ToCompletedOffrs int=0 declare @ToCompletedJCO int=0" +
                            " select @ToCompletedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and req.StatusId=2 and trnstepcout.ApplyForId=1 " +
                            
                            " select @ToCompletedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and req.StatusId=2 and trnstepcout.ApplyForId=2 " +
                            " select @ToCompletedOffrs ToCompletedOffrs,@ToCompletedJCO ToCompletedJCO";
                    break;
                case "Rejected":
                    query = "declare @ToRejectedOffrs int=0 declare @ToRejectedJCO int=0" +
                            " select @ToRejectedOffrs=COUNT(distinct fwd.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId" +
                            " inner join TrnFwds fwd on fwd.RequestId= trnstepcout.RequestId where fwd.ToAspNetUsersId=@UserId and req.StatusId=1 and trnstepcout.StepId in(7,8,9,10) and trnstepcout.ApplyForId=1 " +

                            " select @ToRejectedJCO=COUNT(distinct fwd.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId" +
                            " inner join TrnFwds fwd on fwd.RequestId= trnstepcout.RequestId where fwd.ToAspNetUsersId=@UserId and req.StatusId=1 and trnstepcout.StepId in(7,8,9,10) and trnstepcout.ApplyForId=2 " +
                            " select @ToRejectedOffrs ToRejectedOffrs,@ToRejectedJCO ToRejectedJCO";
                    break;
                case "Posting Out":
                    query = "declare @ToPostingOutOffrs int=0 declare @ToPostingOutJCO int=0 " + 
                            " select @ToPostingOutOffrs=COUNT(distinct pout.Id) from TrnPostingOut pout "+
                            " inner join BasicDetails basic on basic.BasicDetailId=pout.BasicDetailId where pout.FromUnitId=@UnitId and basic.ApplyForId=1 " +
                            
                            " select @ToPostingOutJCO=COUNT(distinct pout.Id) from TrnPostingOut pout "+
                            " inner join BasicDetails basic on basic.BasicDetailId=pout.BasicDetailId where pout.FromUnitId=@UnitId and basic.ApplyForId=2 " +
                            " select @ToPostingOutOffrs ToPostingOutOffrs,@ToPostingOutJCO ToPostingOutJCO";
                    break;
                case "Posting In":
                    query = "declare @ToPostingInOffrs int=0 declare @ToPostingInJCO int=0 " +
                            " select @ToPostingInOffrs=COUNT(distinct pout.Id) from TrnPostingOut pout " +
                            " inner join BasicDetails basic on basic.BasicDetailId=pout.BasicDetailId where pout.ToUnitId=@UnitId and basic.ApplyForId=1 " +
                            
                            " select @ToPostingInJCO=COUNT(distinct pout.Id) from TrnPostingOut pout " +
                            " inner join BasicDetails basic on basic.BasicDetailId=pout.BasicDetailId where pout.ToUnitId=@UnitId and basic.ApplyForId=2 " +
                            " select @ToPostingInOffrs ToPostingInOffrs,@ToPostingInJCO ToPostingInJCO";
                    break;
            }

            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTORequestDashboardCountResponse>(query, new { UserId,UnitId });
                    return ret.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HomeDB->GetRequestDashboardCount");
                return null;
            }
        } 
        public async Task<DTORequestSubDashboardCountResponse> GetSubDashboardCount(int UserId)
        {
            string query = "declare @TotDrafted int=0 declare @TotSubmitted int=0 declare @TotRejected int=0 declare @TotClosed int=0 declare @TotCompleted int=0 " +
                            " select @TotDrafted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and req.StatusId=1 " +

                            " select @TotSubmitted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>1" +

                            " select @TotClosed=COUNT(Id) from TrnApplClose where Updatedby=@UserId" +

                            " select @TotCompleted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id where domain.AspNetUsersId=@UserId and req.StatusId=2 " +

                            " select @TotRejected=COUNT(distinct fwd.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId" +
                            " inner join TrnFwds fwd on fwd.RequestId= trnstepcout.RequestId where fwd.ToAspNetUsersId=@UserId and req.StatusId=1 and trnstepcout.StepId in(7,8,9,10)" +

                            " select @TotDrafted TotDrafted,@TotSubmitted TotSubmitted,@TotCompleted TotCompleted,@TotClosed TotClosed,@TotRejected TotRejected ";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTORequestSubDashboardCountResponse>(query, new { UserId });
                    return ret.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HomeDB->GetRequestDashboardCount");
                return null;
            }
        }
        public async Task<List<DTORegisterUserResponse>> GetAllRegisterUser(int UnitId)
        {
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
            return allrecord;
        }
        public async Task<DTORequestDashboardUserMgtCountResponse> GetDashboardUserMgtCount(int UnitId, int UserId)
        {
            string query = "declare @TotRegisterUser int=0 declare @TotPostingIn int=0 declare @TotPostingOut int=0 " +
                            " select @TotRegisterUser=COUNT(Id) from TrnDomainMapping where UnitId=@UnitId " +
                            " select @TotPostingIn=COUNT(Id) from TrnPostingOut where ToUnitId=@UnitId " +
                            " select @TotPostingOut=COUNT(Id) from TrnPostingOut where FromUnitId=@UnitId " +
                            " select @TotRegisterUser TotRegisterUser,@TotPostingIn TotPostingIn,@TotPostingOut TotPostingOut";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTORequestDashboardUserMgtCountResponse>(query, new { UnitId, UserId });
                    return ret.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HomeDB->GetRequestDashboardCount");
                return null;
            }
        }
    }
}
