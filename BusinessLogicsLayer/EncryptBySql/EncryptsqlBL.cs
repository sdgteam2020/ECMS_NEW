using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.EncryptBySql
{
    public class EncryptsqlBL : IEncryptsqlBL
    {
        private readonly IEncryptsqlDB _iEncryptsqlDB;

        public EncryptsqlBL(IEncryptsqlDB iEncryptsqlDB)
        {
            _iEncryptsqlDB = iEncryptsqlDB;
        }
        public Task<Byte[]> GetEncryptString(string Key, string parmas)
        {
            return _iEncryptsqlDB.GetEncryptString(Key, parmas);
        }
    }
}
