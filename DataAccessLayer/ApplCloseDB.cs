using Azure.Core;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ApplCloseDB : GenericRepositoryDL<TrnApplClose>, IApplCloseDB
    {
        private readonly DapperContext _contextDP;
        public ApplCloseDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _contextDP = contextDP;
        }

        public async Task<bool> RequestIdExists(TrnApplClose DTo)
        {
            string query = "select count(*) from TrnApplClose where RequestId=@RequestId";
            using (var connection = _contextDP.CreateConnection())
            {
                int chk = await connection.QueryFirstAsync<int>(query, new { DTo.RequestId });
                if (chk > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
