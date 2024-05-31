using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataAccessLayer
{
    public class ReportReturnDB : IReportReturnDB
    {
        private readonly DapperContext _contextDP;
        private readonly ILogger<HomeDB> _logger;
        public ReportReturnDB(DapperContext contextDP, ILogger<HomeDB> logger)
        {

            _contextDP = contextDP;
            _logger = logger;
        }
        public async Task<List<DTOReportReturnCount>> GetMstepCount(DTOMHierarchyRequest Data, int ApplyForId)
        {
            string query = " select COUNT(req.RequestId) Total,Mstep.StepId,Mstep.Name,Mstep.GroupId,Mstep.OrderBy from MStepCounterStep Mstep " +
                           " left join TrnStepCounter step on Mstep.StepId=step.StepId and step.ApplyForId=@ApplyForId" +
                           " left join TrnICardRequest req on step.RequestId=req.RequestId and req.Status=0" +
                           " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId" +
                           " left join MapUnit unit on basi.UnitId=unit.UnitMapId" +
                           " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           " and unit.BdeId=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           " and unit.BdeId=ISNULL(@PsoId,unit.PsoId)" +
                           " and unit.BdeId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                           " left join TrnFwds fwd on req.RequestId=fwd.RequestId and IsComplete=0" +
                           " where Mstep.IsDashboard=1" +
                           " group by Mstep.StepId,Mstep.Name,Mstep.GroupId,Mstep.OrderBy order by Mstep.GroupId,Mstep.OrderBy";
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { ApplyForId, Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId });
                return ret.ToList();
            }
        }


        public async Task<List<DTOReportReturnCount>> GetRecordOffOffers()
        {
            string query = "select RecordOfficeId,Name from MRecordOffice where ArmedId=56";
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnCount>(query);
                return ret.ToList();
            }
        }

        public async Task<List<DTOReportReturnCount>> GetRecordOffOffersCount(DTOMHierarchyRequest Data)
        {
            //string query = " select count(req.RequestId) Total ,recf.RecordOfficeId,recf.Name from MRecordOffice recf" +
            //               " left join TrnDomainMapping map on recf.TDMId=map.Id" +
            //               " left join TrnFwds fwd on map.AspNetUsersId=fwd.FromAspNetUsersId and fwd.FwdStatusId=1" +
            //               " left join TrnICardRequest req on fwd.RequestId=req.RequestId and req.Status=0  " +
            //               " left join MRecordOffice mrec on map.Id=mrec.TDMId and mrec.ArmedId=56 " +
            //               " left join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
            //               " left join MapUnit unit on basi.UnitId=unit.UnitMapId" +
            //               " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
            //               " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
            //               " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
            //               " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
            //               " and unit.BdeId=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
            //               " and unit.BdeId=ISNULL(@PsoId,unit.PsoId)" +
            //               " and unit.BdeId=ISNULL(@SubDteId,unit.SubDteId)" +
            //               " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
            //               " where recf.ArmedId=56 group by recf.RecordOfficeId,recf.Name";

            string query = " select COUNT(req.RequestId) Total, fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId, 3 GroupId from MTrnFwdStatus fwdsts" +
                           " inner join TrnFwds fwd on fwdsts.FwdStatusId=fwd.FwdStatusId  " +
                           " inner join TrnICardRequest req on fwd.RequestId=req.RequestId and req.Status=0 " +
                           " inner join TrnStepCounter step on step.ApplyForId=1 and req.RequestId=step.RequestId" +
                           " inner join TrnDomainMapping map on fwd.ToAspNetUsersId=map.AspNetUsersId  " +
                           " inner join OROMapping mrec on map.Id=mrec.TDMId " +
                           " inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                           " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
                           " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           " and unit.BdeId=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           " and unit.BdeId=ISNULL(@PsoId,unit.PsoId)" +
                           " and unit.BdeId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                           " where fwd.IsComplete=0" +
                           " group by fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId";

            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId });
                return ret.ToList();
            }
        }
        public async Task<List<DTOReportReturnCount>> GetRecordJco()
        {
            string query = "select RecordOfficeId ,Name from MRecordOffice where ArmedId!=56";
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnCount>(query);
                return ret.ToList();
            }
        }

        public async Task<List<DTOReportReturnCount>> GetRecordJcoCount(DTOMHierarchyRequest Data, int IsComplete)
        {
            string query = " select count(req.RequestId) Total ,recf.RecordOfficeId,recf.Name,step.StepId from MRecordOffice recf" +
                           " left join TrnDomainMapping map on recf.TDMId=map.Id" +
                           " left join TrnFwds fwd on map.AspNetUsersId=fwd.FromAspNetUsersId and fwd.IsComplete=@IsComplete and fwd.FwdStatusId!=1" +
                           " left join TrnICardRequest req on fwd.RequestId=req.RequestId and req.Status=0  " +
                           " left join TrnStepCounter step on req.RequestId=step.RequestId and step.StepId=3" +
                           " left join MRecordOffice mrec on map.Id=mrec.TDMId and mrec.ArmedId!=56 " +
                           " left join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                           " left join MapUnit unit on basi.UnitId=unit.UnitMapId" +
                           " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           " and unit.BdeId=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           " and unit.BdeId=ISNULL(@PsoId,unit.PsoId)" +
                           " and unit.BdeId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                           " where recf.ArmedId!=56   group by recf.RecordOfficeId,recf.Name,step.StepId";

            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { IsComplete, Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId });
                return ret.ToList();
            }
        }

        public async Task<List<DTOReportReturnListResponse>> GetRecordHistory(DTOMHierarchyRequest Data, int ApplyForId, int StepId)
        {
            string query = "";
            if (StepId != 99)
            {
                query = " select req.RequestId,Mstep.StepId,basi.Name,ServiceNo,DOB,ranks.RankAbbreviation RankName,TrackingId," +
                        " aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo," +
                        " aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom" +
                        " from MStepCounterStep Mstep  " +
                        " inner join TrnStepCounter step on Mstep.StepId=step.StepId " +
                        " left join TrnICardRequest req on step.RequestId=req.RequestId and req.Status=0 " +
                        " left join TrnFwds fwd on req.RequestId=fwd.RequestId and fwd.IsComplete=0" +
                        " left join UserProfile userto on fwd.ToUserId=userto.UserId" +
                        "  LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId" +
                        "  LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id" +
                        " left join MRank ranksto on ranksto.RankId=userto.RankId" +
                        " left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId" +
                        " left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId" +
                        "  LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId" +
                        "  LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id" +
                        " left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId" +
                        " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                        " left join MRank ranks on ranks.RankId=basi.RankId" +
                        " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
                        " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                        " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                        " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                        " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                        " and unit.BdeId=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                        " and unit.BdeId=ISNULL(@PsoId,unit.PsoId)" +
                        " and unit.BdeId=ISNULL(@SubDteId,unit.SubDteId)" +
                        " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                        " where step.ApplyForId=@ApplyForId and step.StepId=@StepId";

            }
            else
            {
                query = " select req.RequestId,Mstep.StepId,basi.Name,ServiceNo,DOB,ranks.RankAbbreviation RankName,TrackingId, " +
                        " aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo, " +
                        " aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom,fwdsts.Name Status" +
                        " from MStepCounterStep Mstep   " +
                        " inner join TrnStepCounter step on Mstep.StepId=step.StepId  " +
                        " inner join TrnICardRequest req on step.RequestId=req.RequestId and req.Status=0  " +
                        " inner join TrnFwds fwd on req.RequestId=fwd.RequestId " +
                        " inner join TrnDomainMapping map on fwd.ToAspNetUsersId=map.AspNetUsersId  " +
                        " inner join OROMapping mrec on map.Id=mrec.TDMId " +
                        " left join UserProfile userto on fwd.ToUserId=userto.UserId  " +
                        " LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId  " +
                        " LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id " +
                        " left join MRank ranksto on ranksto.RankId=userto.RankId " +
                        " left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId " +
                        " left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId  " +
                        " LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId  " +
                        " LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id " +
                        " left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId " +
                        " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId  " +
                        " left join MRank ranks on ranks.RankId=basi.RankId " +
                        " left join MapUnit unit on basi.UnitId=unit.UnitMapId  " +
                        "  and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                          " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                          " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                          " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                          " and unit.BdeId=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                          " and unit.BdeId=ISNULL(@PsoId,unit.PsoId)" +
                          " and unit.BdeId=ISNULL(@SubDteId,unit.SubDteId)" +
                          " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                        " where step.ApplyForId=1 and step.StepId=3 and mrec.RecordOfficeId=@ApplyForId";

            }
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnListResponse>(query, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId, ApplyForId, StepId });
                return ret.ToList();
            }
        }
    
            
           
        
    }
}



//select count(fwd.TypeId) Total, fwd.TypeId GroupId, Mfsts.Name, mstep.StepId from MTrnFwdStatus Mfsts  
//   inner join TrnFwds fwd on Mfsts.FwdStatusId=fwd.FwdStatusId
//   inner join TrnICardRequest req on fwd.RequestId=req.RequestId and req.Status=0 
//  inner join MFwdType Mtype on Mtype.TypeId=fwd.TypeId
//  inner join MStepCounterStep mstep on Mtype.TypeId=mstep.GroupId
//  left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId 
//  left join MapUnit unit on basi.UnitId=unit.UnitMapId 


//  group by fwd.TypeId, Mfsts.Name, mstep.StepId order by fwd.TypeId, Mfsts.Name


