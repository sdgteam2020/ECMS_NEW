using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response.User;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.ViewModels;

namespace DataAccessLayer
{
    public class TrnLoginLogDB : GenericRepositoryDL<TrnLogin_Log>, ITrnLoginLogDB
    {
        private readonly DapperContext _contextDP;
        public TrnLoginLogDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _contextDP = contextDP;
        }

        public async Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId)
        {
            string query = "select users.Id [AspNetUsersId],users.DomainId,roles.Name RoleName,"+
                           " ran.RankAbbreviation RankName,prof.ArmyNo,prof.Name from AspNetUsers users"+
                           " inner join TrnDomainMapping map on map.AspNetUsersId=users.Id"+
                           " inner join AspNetUserRoles urole on urole.UserId=users.Id"+
                           " inner join AspNetRoles roles on roles.Id=urole.[RoleId]"+
                           " inner join UserProfile prof on prof.UserId=users.Id"+
                           " inner join MRank ran on ran.RankId=prof.RankId"+
                           " and map.UnitId=@UnitId";

            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { UnitId });



                return Ret.ToList();
            }
        }

        public async Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId)
        {
            string query = "select logs.[Id],logs.[AspNetUsersId],logs.[UserId],logs.[IP],logs.[Updatedby],logs.[UpdatedOn],logs.[RoleId],users.DomainId,roles.Name RoleName,"+
                           " ran.RankAbbreviation RankName,prof.ArmyNo,prof.Name from TrnLogin_Log logs"+
                           " inner join AspNetUsers users on users.Id=logs.AspNetUsersId"+
                           " inner join TrnDomainMapping map on map.AspNetUsersId=users.Id"+
                           " inner join AspNetRoles roles on roles.Id=logs.[RoleId]"+
                           " inner join UserProfile prof on prof.UserId=logs.UserId"+
                           " inner join MRank ran on ran.RankId=prof.RankId"+
                           " and map.AspNetUsersId=@AspnetUserId and logs.[UpdatedOn] BETWEEN DATEADD(MONTH, -1, GETDATE())  AND GETDATE() order by logs.[UpdatedOn] desc";

            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { AspnetUserId });



                return Ret.ToList();
            }
        }
    }
}
