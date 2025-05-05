using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response.User;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Logger;
using Dapper;
using DataTransferObject.Requests;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer
{
    public class APIDataDB : GenericRepositoryDL<MApiData>, IAPIDataDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        public APIDataDB(ApplicationDbContext context, DapperContext dapperContext) : base(context)
        {
            _context = context;
            _contextDP = dapperContext;
        }

        public async Task<bool> apiLogin(string accessKey)
        {
            string query = "select [Id],[ClientName] from MApiLogin where accessKey=@accessKey";
            using (var connection = _contextDP.CreateConnection())
            {

                var ret = await connection.QueryAsync<MApiLogin>(query, new { accessKey });

                if (ret != null && ret.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

                // return ret.SingleOrDefault();


            }
        }

        public async Task<DTOApiPersDataResponse> GetByIC(DTOAPIDataRequest Data)
        {

            //string query = "SELECT [ApplyForId],[Pers_Army_No],[Pers_Blood_Gp],[Pers_District],[Pers_Father_Name],[Pers_Gender],[Pers_Height],[Pers_House_no],[Pers_Iden_mark_1],[Pers_Iden_mark_2],[Pers_Moh_st],[Pers_Pin_code],[Pers_Police_stn],[Pers_Post_office],[Pers_Rank],[Pers_Regt],[Pers_State],[Pers_Tehsil],[Pers_UID],[Pers_Village],[Pers_birth_dt],[Pers_enrol_dt],[Pers_name] FROM [dbo].[MApiData] where [Pers_Army_No]=@ArmyNo";
            string query = "SELECT     [ApplyForId],[Pers_Army_No],CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_name])) [Pers_name],[Pers_Rank],CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_Father_Name])) [Pers_Father_Name],CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_birth_dt])) [Pers_birth_dt] " +
           " ,[Pers_enrol_dt] " +
           " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_District])) [Pers_District]" +
           " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_State])) [Pers_State]" +
           " ,[Pers_Regt]" +
           " ,[Pers_Height]" +
           " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_UID])) [Pers_UID]" +
           " ,[Pers_Blood_Gp]" +
           " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_House_no])) [Pers_House_no]" +
           " ,[Pers_Moh_st]" +
           " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_Village])) [Pers_Village]" +
           " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_Tehsil])) [Pers_Tehsil]" +
           " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_Post_office])) [Pers_Post_office]" +
           " ,[Pers_Police_stn]" +
           " ,[Pers_Pin_code]" +
           " ,[Pers_Iden_mark_1]" +
           " ,[Pers_Iden_mark_2]" +
           " ,[Pers_Gender]" +
           " FROM [dbo].MApiData where [Pers_Army_No]=@ArmyNo";


            using (var connection = _contextDP.CreateConnection())
            {
                MApiData? ret = (await connection.QueryAsync<MApiData>(query, new { Data.ArmyNo })).FirstOrDefault();

                DTOApiPersDataResponse response = new DTOApiPersDataResponse();

                if(ret != null )
                {
                    response.Pers_Army_No = ret.Pers_Army_No ?? string.Empty;
                    response.Pers_name = ret.Pers_name ?? string.Empty;
                    response.Pers_birth_dt = ret.Pers_birth_dt ?? string.Empty;
                    response.Pers_enrol_dt = ret.Pers_enrol_dt ?? string.Empty;
                    response.Pers_Address.Pers_House_no = ret.Pers_House_no ?? string.Empty; ;
                    response.Pers_Address.Pers_Moh_st = ret.Pers_Moh_st;
                    response.Pers_Address.Pers_Village = ret.Pers_Village;
                    response.Pers_Address.Pers_Tehsil = ret.Pers_Tehsil;
                    response.Pers_Address.Pers_Post_office = ret.Pers_Post_office;
                    response.Pers_Address.Pers_Police_stn = ret.Pers_Police_stn;
                    response.Pers_Address.Pers_Pin_code = ret.Pers_Pin_code;
                    response.Pers_Address.Pers_District = ret.Pers_District;
                    response.Pers_Address.Pers_State = ret.Pers_State;
                    response.Status = true;
                    response.Message = "Ok";
                }
                else
                {
                    response.Status = false;
                    response.Message = "Army No not found!";
                }
                return response;
            }
        }

        public async Task<DTOApiPersDataResponse> GetByoffrsIC(DTOAPIDataRequest Data)
        {
            //string query = "SELECT [ApplyForId],[Pers_Army_No],[Pers_Blood_Gp],[Pers_District],[Pers_Father_Name],[Pers_Gender],[Pers_Height],[Pers_House_no],[Pers_Iden_mark_1],[Pers_Iden_mark_2],[Pers_Moh_st],[Pers_Pin_code],[Pers_Police_stn],[Pers_Post_office],[Pers_Rank],[Pers_Regt],[Pers_State],[Pers_Tehsil],[Pers_UID],[Pers_Village],[Pers_birth_dt],[Pers_enrol_dt],[Pers_name] FROM [dbo].[MApiDataOffrs] where [Pers_Army_No]=@ArmyNo";
            //    string query = "SELECT     [ApplyForId],[Pers_Army_No] "+
            //" FROM [dbo].MApiDataOffrs where [Pers_Army_No]=@ArmyNo";
            string query = "SELECT     [ApplyForId],[Pers_Army_No],CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_name])) [Pers_name],[Pers_Rank],CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_Father_Name])) [Pers_Father_Name],CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_birth_dt])) [Pers_birth_dt] " +
                            " ,[Pers_enrol_dt] " +
                            " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_District])) [Pers_District]" +
                            " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_State])) [Pers_State]" +
                            " ,[Pers_Regt]" +
                            " ,[Pers_Height]" +
                            " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_UID])) [Pers_UID]" +
                            " ,[Pers_Blood_Gp]" +
                            " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_House_no])) [Pers_House_no]" +
                            " ,[Pers_Moh_st]" +
                            " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_Village])) [Pers_Village]" +
                            " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_Tehsil])) [Pers_Tehsil]" +
                            " ,CONVERT(nvarchar(MAX),DEcryptByPassPhrase('ASDC@123',[Pers_Post_office])) [Pers_Post_office]" +
                            " ,[Pers_Police_stn]" +
                            " ,[Pers_Pin_code]" +
                            " ,[Pers_Iden_mark_1]" +
                            " ,[Pers_Iden_mark_2]" +
                            " ,[Pers_Gender]" +
                            " FROM [dbo].[MApiDataOffrs] where [Pers_Army_No]=@ArmyNo";

            using (var connection = _contextDP.CreateConnection())
            {
                MApiDataOffrs? ret = (await connection.QueryAsync<MApiDataOffrs>(query, new { Data.ArmyNo })).FirstOrDefault();
                DTOApiPersDataResponse response = new DTOApiPersDataResponse();

                if (ret != null)
                {
                    response.Pers_Army_No = ret.Pers_Army_No ?? string.Empty;
                    response.Pers_name = ret.Pers_name ?? string.Empty;
                    response.Pers_birth_dt = ret.Pers_birth_dt ?? string.Empty;
                    response.Pers_enrol_dt = ret.Pers_enrol_dt ?? string.Empty;
                    response.Pers_Address.Pers_House_no = ret.Pers_House_no ?? string.Empty; ;
                    response.Pers_Address.Pers_Moh_st = ret.Pers_Moh_st;
                    response.Pers_Address.Pers_Village = ret.Pers_Village;
                    response.Pers_Address.Pers_Tehsil = ret.Pers_Tehsil;
                    response.Pers_Address.Pers_Post_office = ret.Pers_Post_office;
                    response.Pers_Address.Pers_Police_stn = ret.Pers_Police_stn;
                    response.Pers_Address.Pers_Pin_code = ret.Pers_Pin_code;
                    response.Pers_Address.Pers_District = ret.Pers_District;
                    response.Pers_Address.Pers_State = ret.Pers_State;
                    response.Status = true;
                    response.Message = "Ok";
                }
                else 
                {
                    response.Status = false;
                    response.Message = "Army No not found!";
                }
                return response;
            }
        }
    }
}





