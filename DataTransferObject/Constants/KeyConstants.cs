namespace DataTransferObject.Constants
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

        #region Filter Request / Army No

        public const byte ApplicantPostingOut = 50;
        public const byte ApplicantClose = 51;
        public const byte FaultyCardRequest = 52;
        public const byte HoltlistCardRequest = 53;
        public const byte LostCardRequest = 54;
        public const byte DistributeCardRequest = 55;
        public const byte DestructionCardRequest = 56;
        
        #endregion
    }
}
