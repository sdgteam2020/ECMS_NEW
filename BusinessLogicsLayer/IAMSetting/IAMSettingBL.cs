using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.IAMSetting
{
    public class IAMSettingBL:GenericRepositoryDL<DataTransferObject.Domain.Model.IAMSetting>,IIAMSettingBL
    {
        public IAMSettingBL(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
