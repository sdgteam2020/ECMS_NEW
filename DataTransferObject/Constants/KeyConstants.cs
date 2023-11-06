using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperRepo.Core.Constants
{
    public static class KeyConstants
    {
        #region MSSQL

        public const string Mssql = "Mssql";

        public const string MssqlMasterKey = "MssqlMasterKey";

       
        // public const string MssqlSecondConnKey = "MssqlSecondConnKey";

        #endregion

        #region MYSQL

        public const string Mysql = "Mysql";

        public const string MysqlMasterKey = "MysqlMasterKey";

        #endregion

        #region ORACLE

        public const string Oracle = "Oracle";

        public const string OracleMasterKey = "OracleMasterKey";

        #endregion

        #region Return To Front End

        public const int Success = 200;
        public const int BadRequest = 400;
        public const int InternalServerError = 500;
        public const int Save = 1;
        public const int Update = 2;
        public const int Exists = 3;
        public const int IncorrectData = 4;


        #endregion
    }
}
