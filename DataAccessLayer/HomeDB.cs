using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Response;
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
        public HomeDB(DapperContext contextDP)
        {
            _contextDP = contextDP;
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
        public async Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId)
        {
            string query = "declare @ToSubmittedOffrs int=0 declare @ToSubmittedJCO int=0 declare @ToDraftedOffrs int=0 declare @ToDraftedJCO int=0" +
                " select @ToDraftedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain"+
                " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id "+
                " inner join MRegistration mreg on mreg.RegistrationId=req.RegistrationId where domain.AspNetUsersId=@UserId and mreg.ApplyForId=1" +
                " select @ToDraftedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain"+
                " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id "+
                " inner join MRegistration mreg on mreg.RegistrationId=req.RegistrationId where domain.AspNetUsersId=@UserId and mreg.ApplyForId=2" +
                " select @ToSubmittedOffrs=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id "+
                " inner join TrnFwds trnfwd on trnfwd.RequestId=req.RequestId "+
                " inner join MRegistration mreg on mreg.RegistrationId=req.RegistrationId where domain.AspNetUsersId=@UserId and mreg.ApplyForId=1" +
                " select @ToSubmittedJCO=COUNT(distinct req.RequestId) from TrnDomainMapping domain"+
                " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id "+
                " inner join TrnFwds trnfwd on trnfwd.RequestId=req.RequestId "+
                " inner join MRegistration mreg on mreg.RegistrationId=req.RegistrationId where domain.AspNetUsersId=@UserId and mreg.ApplyForId=2" +
                " select @ToSubmittedOffrs ToSubmittedOffrs,@ToSubmittedJCO ToSubmittedJCO,@ToSubmittedOffrs ToSubmittedOffrs,@ToSubmittedJCO ToSubmittedJCO";


            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<DTORequestDashboardCountResponse>(query, new { UserId });
                return ret.SingleOrDefault();
            }
        }
    }
}
