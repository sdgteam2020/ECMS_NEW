using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.TrnMappingUnMappingLog
{
    public class TrnMappingUnMappingLogBL:ITrnMappingUnMappingLogBL
    {
        private readonly ITrnMappingUnMappingLogDB _iTrnMappingUnMappingLogDB;
        public TrnMappingUnMappingLogBL(ITrnMappingUnMappingLogDB iTrnMappingUnMappingLogDB)
        {
            _iTrnMappingUnMappingLogDB=iTrnMappingUnMappingLogDB;
        }
        public async Task<bool> Add(TrnMappingUnMapping_Log Data)
        {
            return await _iTrnMappingUnMappingLogDB.Add(Data);
        }

    }
}
