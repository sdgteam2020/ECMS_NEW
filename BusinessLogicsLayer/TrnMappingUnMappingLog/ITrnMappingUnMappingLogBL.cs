using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.TrnMappingUnMappingLog
{
    public interface ITrnMappingUnMappingLogBL
    {
        public Task<bool> Add(TrnMappingUnMapping_Log Data);
    }
}
