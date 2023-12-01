using BusinessLogicsLayer.ArmedCat;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Registration
{
    public class RegistrationBL : GenericRepositoryDL<DataTransferObject.Domain.Master.MRegistration>, IRegistrationBL
    {
        public RegistrationBL(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
