using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
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
        public async Task<List<DTOReportReturnCount>> GetMstepCount(int UserId)
        {
            string query = " select COUNT(req.RequestId) Total,Mstep.StepId,Mstep.Name from MStepCounterStep Mstep " +
                           " left join TrnStepCounter step on Mstep.StepId=step.StepId "+
                           " left join TrnICardRequest req on step.RequestId=req.RequestId and req.Status=0"+
                           " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId"+
                           " left join TrnFwds fwd on req.RequestId=fwd.RequestId and IsComplete=0"+
                           " where Mstep.IsDashboard=1" +
                           " group by Mstep.StepId,Mstep.Name";
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { UserId });
                return ret.ToList();
            }
        }

        public async Task<List<DTOReportReturnCount>> GetRecordOffOffers()
        {
            string query = "select RecordOfficeId Id,Name from MRecordOffice where ArmedId=56";
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnCount>(query);
                return ret.ToList();
            }
        }

        public async Task<List<DTOReportReturnCount>> GetRecordOffOffersCount(int UserId)
        {
            string query = "select COUNT(req.RequestId) Total, fwdsts.Name,mrec.RecordOfficeId StepId from MTrnFwdStatus fwdsts" +
                           " left join TrnFwds fwd on fwdsts.FwdStatusId=fwd.FwdStatusId " +
                           " left join TrnICardRequest req on fwd.RequestId=req.RequestId and req.Status=0" +
                           " left join TrnDomainMapping map on fwd.FromAspNetUsersId=map.AspNetUsersId " +
                           " left join MRecordOffice mrec on map.Id=mrec.TDMId and mrec.ArmedId=56" +
                           " group by fwdsts.Name,mrec.RecordOfficeId";
                            
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = await connection.QueryAsync<DTOReportReturnCount>(query);
                return ret.ToList();
            }
        }
    }
}
