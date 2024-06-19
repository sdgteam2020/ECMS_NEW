using BusinessLogicsLayer.IssuingAuthority;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BloodGroup
{
    public class BloodGroupBL : GenericRepositoryDL<MBloodGroup>, IBloodGroupBL
    {
        public BloodGroupBL(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
