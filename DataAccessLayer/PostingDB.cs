using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
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
        public async Task<DTOPostingInResponse> GetArmyDataForPostingIn(string ArmyNo)
        {
            try
            {

                string query = "  SELECT trnicardr.RequestId,basi.Name,basi.ServiceNo,ranks.RankAbbreviation RankName,appl.Name ApplyFor,trnicardr.TrackingId,"+
                                " trnicardr.Status,uplod.PhotoImagePath"+
                                " ,users.DomainId Users_DomainId,pro.ArmyNo Users_ArmyNo,pro.Name Users_Name,ranks1.RankAbbreviation Users_RankName,app.AppointmentName Users_AppointmentName"+
                                " ,muni.UnitName,muni.Suffix,muni.Sus_no from BasicDetails basi" +
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
    }
}
