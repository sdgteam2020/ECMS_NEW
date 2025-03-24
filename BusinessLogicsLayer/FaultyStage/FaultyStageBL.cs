using BusinessLogicsLayer.BloodGroup;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.FaultyStage
{
    public class FaultyStageBL : GenericRepositoryDL<MFaultyStage>, IFaultyStageBL
    {
        public FaultyStageBL(ApplicationDbContext context) : base(context)
        {
                
        }
    }
}
