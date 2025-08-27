using BusinessLogicsLayer.AfsacCellMapp;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.EncryptionSetting
{
    public class EncryptionSettingBL : GenericRepositoryDL<MEncryptionSetting>, IEncryptionSettingBL
    {
        public EncryptionSettingBL(ApplicationDbContext context) : base(context)
        {
        }
    }
}
