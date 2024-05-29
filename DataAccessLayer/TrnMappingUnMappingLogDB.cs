using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class TrnMappingUnMappingLogDB:ITrnMappingUnMappingLogDB
    {
        private readonly DapperContextDb2 _contextDB2;
        private readonly DapperContext _context;
        public TrnMappingUnMappingLogDB(DapperContextDb2 contextDB2, DapperContext context)
        {
            _contextDB2 = contextDB2;
            _context = context;
        }
        public Task<bool> Add(TrnMappingUnMapping_Log Data)
        {
            using (var connection = _contextDB2.CreateConnection())
            {
                connection.Execute("INSERT INTO [dbo].[TrnMappingUnMapping_Log]([TDMId],[UserId],[DeregisterUserId],[IsActive],[Updatedby],[UpdatedOn]) VALUES (@AspNetUsersId,@UserId,@IP,@IsActive,@Updatedby,@UpdatedOn,@RoleId)", new { Data.TDMId, Data.UserId, Data.DeregisterUserId, Data.IsActive, Data.Updatedby, Data.UpdatedOn });
                return Task.FromResult(true);
            }
        }
    }
}
