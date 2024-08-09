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
using DataTransferObject.Requests;
using Azure.Core;

namespace DataAccessLayer
{
    public class TrnLoginLogDB : ITrnLoginLogDB
    {
        private readonly DapperContextDb2 _contextDP2;
        private readonly DapperContext _context;
        public TrnLoginLogDB(DapperContextDb2 contextDP2, DapperContext context) 
        {
            _contextDP2 = contextDP2;
            _context = context;
        }

        public async Task<bool> Add(TrnLogin_Log Data)
        {
            string query = "";

            using (var connection = _contextDP2.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                //var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { UnitId });
                connection.Execute("INSERT INTO [dbo].[TrnLogin_Log]([AspNetUsersId],[UserId],[IP],[IsActive],[Updatedby],[UpdatedOn],[RoleId]) VALUES (@AspNetUsersId,@UserId,@IP,@IsActive,@Updatedby,@UpdatedOn,@RoleId)", new { Data.AspNetUsersId,Data.UserId,Data.IP,Data.IsActive,Data.Updatedby,Data.UpdatedOn,Data.RoleId });


                return true;
            }
        }

        public async Task<bool> XmlFileDigitalSign(DTOXmlFilesFwdLogRequest Data)
        {
            string query = "";

            using (var connection = _contextDP2.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                //var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { UnitId });
                if(Data.Id==0)
                connection.Execute("INSERT INTO [dbo].[XmlFilesFwdLog]([XmlFiles],[RequestId],[Updatedby],[UpdatedOn],[IsActive]) VALUES (@XmlFiles,@RequestId,@Updatedby,@UpdatedOn,@IsActive)", new { Data.XmlFiles, Data.RequestId, Data.Updatedby, Data.UpdatedOn, Data.IsActive });
                else
                connection.Execute("UPDATE [dbo].[XmlFilesFwdLog] SET [XmlFiles] =@XmlFiles ,[RequestId] = @RequestId,[Updatedby] = @Updatedby,[UpdatedOn] = @UpdatedOn,[IsActive] =  @IsActive WHERE [Id]= @Id", new { Data.XmlFiles, Data.RequestId, Data.Updatedby, Data.UpdatedOn, Data.IsActive,Data.Id });

                return true;
            }
        }
      
        public async Task<DTOXmlFilesFwdLogRequest> XmlFileDigitalSignFromData(int[] RequestId)
        {
          
            string query = "select Id,[XmlFiles],[RequestId],[Updatedby],[UpdatedOn],[IsActive] from XmlFilesFwdLog where RequestId in @RequestId";

            using (var connection = _contextDP2.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var Ret = await connection.QueryAsync<DTOXmlFilesFwdLogRequest>(query, new { RequestId });



                return Ret.FirstOrDefault();
            }
        }
        public async Task<List<DTOLoginLogResponse>> GetAllUserByUnitId(int UnitId)
        {
            string query = "select users.Id [AspNetUsersId],users.DomainId,roles.Name RoleName,"+
                           " ran.RankAbbreviation RankName,prof.ArmyNo,prof.Name from AspNetUsers users"+
                           " inner join TrnDomainMapping map on map.AspNetUsersId=users.Id"+
                           " inner join AspNetUserRoles urole on urole.UserId=users.Id"+
                           " inner join AspNetRoles roles on roles.Id=urole.[RoleId]"+
                           " inner join UserProfile prof on prof.UserId=map.UserId " +
                           " inner join MRank ran on ran.RankId=prof.RankId"+
                           " and map.UnitId=@UnitId";

            using (var connection = _context.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { UnitId });



                return Ret.ToList();
            }
        }

        public async Task<List<DTOLoginLogResponse>> GetLoginLogByUserId(int AspnetUserId, DateTime? FmDate, DateTime? ToDate)
        {
            string query = "select logs.[Id],logs.[AspNetUsersId],logs.[UserId],logs.[IP],logs.[Updatedby],logs.[UpdatedOn],logs.[RoleId],users.DomainId,roles.Name RoleName," +
                           " ran.RankAbbreviation RankName,prof.ArmyNo,prof.Name from [AFSAC2].[dbo].TrnLogin_Log logs" +
                           " inner join AspNetUsers users on users.Id=logs.AspNetUsersId" +
                           " inner join TrnDomainMapping map on map.AspNetUsersId=users.Id" +
                           " inner join AspNetRoles roles on roles.Id=logs.[RoleId]" +
                           " inner join UserProfile prof on prof.UserId=logs.UserId" +
                           " inner join MRank ran on ran.RankId=prof.RankId" +
                           " and map.AspNetUsersId=@AspnetUserId and CAST(logs.[UpdatedOn] as Date) BETWEEN CAST(@FmDate AS DATE)  AND CAST(@ToDate AS DATE) order by logs.[UpdatedOn] desc";
            //string query = "select logs.[Id],logs.[AspNetUsersId],logs.[UserId],logs.[IP],logs.[Updatedby],logs.[UpdatedOn],logs.[RoleId],roles.Name RoleName"+
            //               " from [AFSAC2].[dbo].TrnLogin_Log logs "+
            //               " inner join AspNetRoles roles on roles.Id=logs.[RoleId] " +
            //               " where logs.AspNetUsersId=@AspnetUserId and CAST(logs.[UpdatedOn] as Date) BETWEEN CAST(@FmDate AS DATE)  AND CAST(@ToDate AS DATE) order by logs.[UpdatedOn] desc";
            using (var connection = _context.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { AspnetUserId, FmDate, ToDate });



                return Ret.ToList();
            }
        }

        public async Task<bool> AddDataExport(DTODataExported Data)
        {
            string query = "";

            using (var connection = _contextDP2.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                //var Ret = await connection.QueryAsync<DTOLoginLogResponse>(query, new { UnitId });
                connection.Execute("INSERT INTO [dbo].[TrnExported]([AspNetUsersId],[UserId],[IP],[CreatedBy],[CreatedOn],[RequestId]) VALUES (@AspNetUsersId,@UserId,@IP,@CreatedBy,@CreatedOn,@RequestId)", new { Data.AspNetUsersId, Data.UserId, Data.IP, Data.CreatedBy, Data.CreatedOn,Data.RequestId });


                return true;
            }
        }
    }
}
