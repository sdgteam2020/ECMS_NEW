using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;

namespace DataAccessLayer
{
    public class StepCounterDB : GenericRepositoryDL<MStepCounter>, IStepCounterDB
    {
        private readonly DapperContext _contextDP;
        public StepCounterDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _contextDP = contextDP;
        }

        /// <summary>
        /// Updates the step counter for a given request and updates the BasicDetails if the step ID is 3.
        /// </summary>
        /// <param name="Data">The MStepCounter object containing the data to be updated.</param>
        /// <returns>The updated MStepCounter object.</returns>
        public async Task<MStepCounter> UpdateStepCounter(MStepCounter Data)
        {
            string query = "";
            if (Data.StepId==3)
            {
                query = "Update TrnStepCounter set StepId=@StepId,Updatedby=@Updatedby where RequestId=@Id" +
               " Update BasicDetails set DateOfIssue=GETDATE() where BasicDetailId=(select BasicDetailId from TrnICardRequest where RequestId=@Id)";
            }
            else
            {
                query = "Update TrnStepCounter set StepId=@StepId,Updatedby=@Updatedby where RequestId=@Id";
            }
           

            int StepId=Data.StepId;
            int Updatedby=Data.Updatedby ?? 0;
            int id=Data.RequestId;
            string UnitName = Data.UnitName;
            using (var connection = _contextDP.CreateConnection())
            {
                var ret = connection.Execute(query, new { StepId, Updatedby, id,UnitName });
                return Data;
            }
        }
    }
}
