using BusinessLogicsLayer.FaultyStage;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.FaultyCard
{
    public class FaultyCardBL : GenericRepositoryDL<TrnFaultyCard>, IFaultyCardBL
    {
        public FaultyCardBL(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
