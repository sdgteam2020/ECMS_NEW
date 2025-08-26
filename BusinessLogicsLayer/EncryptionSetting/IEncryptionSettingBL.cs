using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.EncryptionSetting
{
    public interface IEncryptionSettingBL : IGenericRepositoryDL<MEncryptionSetting>
    {
    }
}
