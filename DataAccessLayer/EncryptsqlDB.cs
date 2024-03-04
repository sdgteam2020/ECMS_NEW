using Dapper;
using DapperRepo.Core.Constants;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class EncryptsqlDB : IEncryptsqlDB
    {
        private readonly DapperContext _contextDP;
        public EncryptsqlDB(DapperContext contextDP)
        {
            _contextDP = contextDP;
        }
        public class DToEncrypt
        {
            public Byte[] ret { get; set; }
        }
        public async Task<Byte[]> GetEncryptString(string Key, string parmas)
        {
           
            try
            {
                string query = "select EncryptByPassPhrase(@Key,CONVERT(nvarchar(MAX) ,@parmas)) ret";


                using (var connection = _contextDP.CreateConnection())
                {
                    //data.MRank.RankAbbreviation
                    //data.MArmedType.Abbreviation
                    var ret = await connection.QueryFirstAsync<DToEncrypt>(query, new { Key, parmas });


                    return ret.ret;
                }
            }catch (Exception ex) { return null; }
        }
    }
}
