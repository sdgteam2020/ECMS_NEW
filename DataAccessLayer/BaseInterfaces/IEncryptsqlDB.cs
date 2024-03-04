using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IEncryptsqlDB
    {
        public Task<Byte[]> GetEncryptString(string Key, string parmas);
    }
}
