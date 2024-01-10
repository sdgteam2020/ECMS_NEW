using BusinessLogicsLayer.Bde;
using DapperRepo.Core.Constants;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.BdeCate
{
    public class StepCounterBL : GenericRepository<MStepCounter>, IStepCounterBL
    {

        private readonly IStepCounterDB _iStepCounterDB;
        public StepCounterBL(IStepCounterDB stepCounterDB)
        {
            _iStepCounterDB = stepCounterDB;
        }

        public async Task<MStepCounter> UpdateStepCounter(MStepCounter Data)
        {
            return await _iStepCounterDB.UpdateStepCounter(Data);
        }
    }
}
