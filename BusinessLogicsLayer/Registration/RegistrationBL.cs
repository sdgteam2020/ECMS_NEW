using BusinessLogicsLayer.ArmedCat;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Registration
{
    public class RegistrationBL : GenericRepositoryDL<DataTransferObject.Domain.Master.MRegistration>, IRegistrationBL
    {
        private readonly IRegistrationDB _registrationDB;
        public RegistrationBL(ApplicationDbContext context, IRegistrationDB registrationDB) : base(context)
        {
            _registrationDB= registrationDB;
        }

        public async Task<List<MRegistration>> GetByApplyFor(MRegistration Data)
        {
            return await _registrationDB.GetByApplyFor(Data);
        }
    }
}
