using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.EncryptBySql
{
   public interface IEncryptsqlBL
    {
        public Task<Byte[]> GetEncryptString(string Key, string parmas);

    }
}
