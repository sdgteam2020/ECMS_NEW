using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class HomeDB : IHomeDB
    {
        private readonly DapperContext _contextDP;
        private readonly ILogger<HomeDB> _logger;
        public HomeDB(DapperContext contextDP, ILogger<HomeDB> logger)
        {
            _contextDP = contextDP;
            _logger = logger;
        }
        public async Task<DTODashboardCountResponse> GetDashBoardCount(int UserId)
        {
            string query = "declare @TotReq int=0 declare @TotReject int=0 declare @TotSelfPen int=0 declare @TotIOPen int=0 declare @TotGsoPen int=0 declare @TotM11Pen int=0 declare @TotGQ54Pen int=0 declare @TotPrintPen int=0" +
                            " select @TotReq=COUNT(distinct req.RequestId) from TrnDomainMapping domain inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id where domain.AspNetUsersId=@UserId" +
                            " select @TotReject=COUNT(distinct req.RequestId) from TrnDomainMapping domain inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnFwds fwd on fwd.ToAspNetUsersId= domain.AspNetUsersId and fwd.IsComplete=0 and fwd.[Status]=0 and fwd.RequestId=req.RequestId where domain.AspNetUsersId=@UserId" +
                            " select @TotSelfPen=COUNT(distinct req.RequestId)   from TrnDomainMapping domain inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter step on step.RequestId=req.RequestId where domain.AspNetUsersId=@UserId and StepId=1" +
                            " select @TotIOPen=COUNT(distinct req.RequestId)  from TrnDomainMapping domain inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter step on step.RequestId=req.RequestId where domain.AspNetUsersId=@UserId and StepId=2" +
                            " select @TotGsoPen=COUNT(distinct req.RequestId)   from TrnDomainMapping domain inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter step on step.RequestId=req.RequestId where domain.AspNetUsersId=@UserId and StepId=3" +
                            " select @TotM11Pen=COUNT(distinct req.RequestId)  from TrnDomainMapping domain inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter step on step.RequestId=req.RequestId where domain.AspNetUsersId=@UserId and StepId=4" +
                            " select @TotGQ54Pen=COUNT(distinct req.RequestId)   from TrnDomainMapping domain inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter step on step.RequestId=req.RequestId where domain.AspNetUsersId=@UserId and StepId=5" +
                            " select @TotPrintPen=COUNT(distinct req.RequestId)  from TrnDomainMapping domain inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter step on step.RequestId=req.RequestId where domain.AspNetUsersId=@UserId and StepId=6" +
                            " select @TotReq TotReq,@TotReject TotReject,@TotSelfPen TotSelfPen,@TotIOPen TotIOPen,@TotGsoPen TotGsoPen,@TotM11Pen TotM11Pen,@TotGQ54Pen TotGQ54Pen,@TotPrintPen TotPrintPen";


            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<DTODashboardCountResponse>(query, new { UserId });
                return ret.SingleOrDefault();
            }
        }
        public async Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId,string Type)
        {
            string query="";
            switch (Type)
            {
                case "Drafted":
                    query = "declare @ToDraftedOffrs int=0 declare @ToDraftedJCO int=0" +
                            " select @ToDraftedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and trnstepcout.ApplyForId=1 " +
                            
                            " select @ToDraftedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and trnstepcout.ApplyForId=2 " +
                            " select @ToDraftedOffrs ToDraftedOffrs,@ToDraftedJCO ToDraftedJCO";
                    break;
                case "Submitted":
                    query = "declare @ToSubmittedOffrs int=0 declare @ToSubmittedJCO int=0" +
                            " select @ToSubmittedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>2 and trnstepcout.ApplyForId=1 " +
                            " select @ToSubmittedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>2 and trnstepcout.ApplyForId=2 " +
                            " select @ToSubmittedOffrs ToSubmittedOffrs,@ToSubmittedJCO ToSubmittedJCO";
                    break;
                case "Rejected":
                    query = "declare @ToRejectedOffrs int=0 declare @ToRejectedJCO int=0" +
                            " select @ToRejectedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId in(7,8,9,10) and trnstepcout.ApplyForId=1 " +
                            " select @ToRejectedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId in(7,8,9,10) and trnstepcout.ApplyForId=2 " +
                            " select @ToRejectedOffrs ToRejectedOffrs,@ToRejectedJCO ToRejectedJCO";
                    break;
            }

            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTORequestDashboardCountResponse>(query, new { UserId });
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
            string query = "declare @TotDrafted int=0 declare @TotSubmitted int=0 declare @TotRejected int=0" +
                            " select @TotDrafted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 " +

                            " select @TotSubmitted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>2" +

                            " select @TotRejected=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                            " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                            " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId in(7,8,9,10) " +

                            " select @TotDrafted TotDrafted,@TotSubmitted TotSubmitted,@TotRejected TotRejected";
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
    }
}
