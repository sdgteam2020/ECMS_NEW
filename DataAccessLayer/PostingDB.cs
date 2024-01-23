using Azure.Core;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class PostingDB : IPostingDB
    {
        protected readonly DapperContext _contextDP;
        public PostingDB(DapperContext contextDP) 
        {

            _contextDP = contextDP;
        }

        public async Task<List<DTOPostingOutDetilsResponse>> GetAllPostingHistory(int AspNetUsersId)
        {
            try
            {

                string query = "select res.Reason,Authority,CONVERT (varchar(10),Cast(SOSDate as date), 103) SOSDate,CONVERT (varchar(10),Cast(pout.UpdatedOn as date), 103) UpdatedOn,user1.DomainId FromDomainId,user2.DomainId TODomainId," +
                               " unit1.UnitName FromUnitName,unit2.UnitName ToUnitName,prof1.ArmyNo FromArmyNO,prof2.ArmyNo TOArmyNO,ranks.RankAbbreviation FromRankName,prof1.Name FromName from TrnPostingOut pout" +
                               " inner join MPostingReason res on pout.ReasonId=res.Id"+
                               " inner join AspNetUsers user1 on user1.Id=pout.FromAspNetUsersId"+
                               " inner join AspNetUsers user2 on user2.Id=pout.ToAspNetUsersId"+
                               " inner join MapUnit mapunit1 on mapunit1.UnitMapId=pout.FromUnitID"+
                               " inner join MUnit unit1 on unit1.UnitId=mapunit1.UnitId"+
                               " inner join MapUnit mapunit2 on mapunit2.UnitMapId=pout.ToUnitID"+
                               " inner join MUnit unit2 on unit2.UnitId=mapunit2.UnitId"+
                               " inner join UserProfile prof1 on prof1.UserId=pout.FromUserID"+
                               " inner join MRank ranks on ranks.RankId=prof1.RankId" +
                               " inner join UserProfile prof2 on prof2.UserId=pout.ToUserID"+
                               " where pout.FromAspNetUsersId= @AspNetUsersId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOPostingOutDetilsResponse>(query, new { AspNetUsersId });

                    return ret.ToList();

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DTOPostingInResponse> GetArmyDataForPostingOut(string ArmyNo)
        { 
            try
            {

                string query = "  SELECT trnicardr.RequestId,basi.Name,basi.ServiceNo,ranks.RankAbbreviation RankName,appl.Name ApplyFor,trnicardr.TrackingId,"+
                                " trnicardr.Status,uplod.PhotoImagePath"+
                                " ,users.DomainId Users_DomainId,pro.ArmyNo Users_ArmyNo,pro.Name Users_Name,ranks1.RankAbbreviation Users_RankName,app.AppointmentName Users_AppointmentName"+
                                " ,muni.UnitName,muni.Suffix,muni.Sus_no,mapunit.UnitMapId FromUnitID,users.Id FromAspNetUsersId,pro.userId FromUserID from BasicDetails basi" +
                                " inner join TrnICardRequest trnicardr on trnicardr.BasicDetailId=basi.BasicDetailId"+
                                " inner join TrnDomainMapping trndom on trndom.id=trnicardr.TrnDomainMappingId" +
                                " inner join MRank ranks on ranks.RankId=basi.RankId"+
                                " inner join MApplyFor appl on appl.ApplyForId=basi.ApplyForId"+
                                " inner join TrnUpload uplod on uplod.BasicDetailId=basi.BasicDetailId"+
                                " inner join AspNetUsers users on users.Id=trndom.AspNetUsersId"+
                                " inner join UserProfile pro on pro.UserId=trndom.UserId"+
                                " inner join MRank ranks1 on ranks1.RankId=pro.RankId"+
                                " inner join MAppointment app on app.ApptId=trndom.ApptId"+
                                " inner join MapUnit mapunit on mapunit.UnitMapId=basi.UnitId"+
                                " inner join MUnit muni on muni.UnitId=mapunit.UnitId"+
                                " where basi.ServiceNo=@ArmyNo and trnicardr.Status=0";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOPostingInResponse>(query, new { ArmyNo });

                    return ret.SingleOrDefault();

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateForPosting(TrnPostingOut Data)
        {
            string query = "update TrnICardRequest set TrnDomainMappingId=(select Id from TrnDomainMapping where AspNetUsersId=@ToAspNetUsersId) where RequestId=@RequestId " +
                // " update BasicDetails set UnitId=@ToUnitID where BasicDetailId =(select BasicDetailId from TrnICardRequest where RequestId=@RequestId)";
                //" update TrnStepCounter set StepId=1 where RequestId=@RequestId" +
                //" update TrnFwds set Status=0 ,IsComplete=1,Remark='Posting Out' ,ToAspNetUsersId=@ToAspNetUsersId where RequestId=@RequestId and IsComplete=0";
                " update TrnFwds set PostingOutId= @Id where RequestId=@RequestId and IsComplete=0";
            int ToAspNetUsersId = Data.ToAspNetUsersId;
            int RequestId = Data.RequestId;
            int ToUnitID = Data.ToUnitID;
            int Id=Data.Id;

            using (var connection = _contextDP.CreateConnection())
            {
                 connection.Execute(query, new { ToAspNetUsersId, RequestId, Id });//,ToUnitID

                return true;

            }
            

        }
    }
}
