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

    }
}
