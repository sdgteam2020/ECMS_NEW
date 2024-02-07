using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Azure.Core;
using DataTransferObject.Response.User;


namespace DataAccessLayer
{
    public class BasicDetailTempDB : GenericRepositoryDL<BasicDetailTemp>, IBasicDetailTempDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        public BasicDetailTempDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            this.protector = protector;
        }
        public async Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId)
        {
            //var BasicDetailTempList = _context.BasicDetailTemps.Where(x => x.Updatedby == UserId).ToList();
            var query = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.Name,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                        " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2"+
                        " ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps  Temps" +
                        " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                        " WHERE Temps.Updatedby=@UserId AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(query, new { UserId });
                int sno = 1;
                var allrecord = (from e in BasicDetailTempList
                                 select new DTOBasicDetailTempRequest()
                                 {
                                     EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                     Sno = sno++,
                                     Name = e.Name,
                                     ServiceNo = e.ServiceNo,
                                     DOB = e.DOB,
                                     DateOfCommissioning = e.DateOfCommissioning,
                                     District = e.District,
                                     PO = e.PO,
                                     PS = e.PS,
                                     PinCode = e.PinCode,
                                     State = e.State,
                                     Tehsil = e.Tehsil,
                                     Village = e.Village,
                                     Observations = e.Observations,
                                     Remarks2=e.Remarks2,
                                     RankName = e.RankName, 
                                     UpdatedOn = e.UpdatedOn,
                                     RegistrationId=e.RegistrationId,
                                     TypeId=e.TypeId,
                                     ApplyForId= e.ApplyForId


                                 }).ToList();
                return await Task.FromResult(allrecord);

            }

        }

        public async Task<DTOBasicDetailTempRequest> GetALLBasicDetailTempByBasicDetailId(int UserId, int BasicDetailId)
        {
            //var BasicDetailTempList = _context.BasicDetailTemps.Where(x => x.Updatedby == UserId).ToList();
            var query = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.Name,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                        " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2" +
                        " ,mappy.Name ApplyType,reg.Name RegistrationName,micard.Name CardType" +
                        " ,users.DomainId,unit.UnitName,unit.Suffix,unit.Sus_no,pro.Name OffName,ranks.RankAbbreviation,pro.ArmyNo,Temps.UpdatedOn " +
                        " FROM BasicDetailTemps  Temps" +
                        " inner join MApplyFor mappy on mappy.ApplyForId=Temps.ApplyForId" +
                        " inner join MRegistration reg on Temps.ApplyForId=reg.RegistrationId" +
                        " inner join MICardType micard on Temps.TypeId=micard.TypeId" +
                        " inner join AspNetUsers users on users.Id = Temps.Updatedby" +
                        " inner join TrnDomainMapping trn on trn.AspNetUsersId = users.Id" +
                        " inner join MapUnit mapuni on mapuni.UnitMapId = trn.UnitId" +
                        " inner join MUnit unit on unit.UnitId = mapuni.UnitId" +
                        " left join UserProfile pro on pro.UserId = trn.UserId" +
                        " inner join MRank ranks on ranks.RankId = pro.RankId"+
                        " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                        " WHERE Temps.Updatedby=@UserId and Temps.BasicDetailTempId=@BasicDetailId ORDER BY Temps.UpdatedOn DESC";
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(query, new { UserId, BasicDetailId });
                int sno = 1;
                var allrecord = (from e in BasicDetailTempList
                                 select new DTOBasicDetailTempRequest()
                                 {
                                     EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                     Sno = sno++,
                                     Name = e.Name,
                                     ServiceNo = e.ServiceNo,
                                     DOB = e.DOB,
                                     DateOfCommissioning = e.DateOfCommissioning,
                                     District = e.District,
                                     PO = e.PO,
                                     PS = e.PS,
                                     PinCode = e.PinCode,
                                     State = e.State,
                                     Tehsil = e.Tehsil,
                                     Village = e.Village,
                                     Observations = e.Observations,
                                     Remarks2 = e.Remarks2,
                                     ApplyType=e.ApplyType,
                                     RegistrationName=e.RegistrationName,
                                     CardType=e.CardType,
                                     DomainId=e.DomainId,
                                     UnitName= e.UnitName,  
                                     Suffix=e.Suffix,
                                     Sus_no = e.Sus_no,
                                     OffName = e.OffName,
                                     RankAbbreviation = e.RankAbbreviation,
                                     ArmyNo = e.ArmyNo,
                                     RankName=e.RankName,
                                     UpdatedOn=e.UpdatedOn

                                 }).SingleOrDefault();
                return await Task.FromResult(allrecord);

            }
        }

        public async Task<BasicDetailTemp> GetByArmyNo(string ArmyNo)
        {
            var query = "SELECT * FROM BasicDetailTemps where ServiceNo=@ArmyNo";
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailTempList = await connection.QueryAsync<BasicDetailTemp>(query, new { ArmyNo });
               
                return BasicDetailTempList.SingleOrDefault();

            }

        }

        public async Task<bool> UpdateByArmyNo(string ArmyNo)
        {
            
            using (var connection = _contextDP.CreateConnection())
            {
                connection.Execute("UPDATE BasicDetailTemps SET IsActive=0 WHERE ServiceNo=@ArmyNo", new { ArmyNo });

                return true;

            }
        }
    }
}
