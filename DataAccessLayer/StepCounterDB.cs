using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response.User;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class StepCounterDB : GenericRepositoryDL<MStepCounter>, IStepCounterDB
    {
        private readonly DapperContext _contextDP;
        public StepCounterDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _contextDP = contextDP;
        }

        public async Task<MStepCounter> UpdateStepCounter(MStepCounter Data)
        {
            string query = "Update TrnStepCounter set StepId=@StepId,Updatedby=@Updatedby where Id=@Id";

            int StepId=Data.StepId;
            int Updatedby=Data.Updatedby;
            int id=Data.Id;
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = connection.Execute(query, new { StepId, Updatedby, id });



                return Data;
            }
        }
    }
}
