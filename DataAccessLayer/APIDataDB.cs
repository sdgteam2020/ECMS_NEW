using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response.User;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Logger;
using Dapper;
using DataTransferObject.Requests;

namespace DataAccessLayer
{
    public class APIDataDB : GenericRepositoryDL<MApiData>, IAPIDataDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        public APIDataDB(ApplicationDbContext context, DapperContext dapperContext) : base(context)
        {
            _context = context;
            _contextDP= dapperContext;
        }

        public async Task<MApiData> GetByIC(DTOAPIDataRequest Data)
        {

            string query = "SELECT [ApplyForId],[Pers_Army_No],[Pers_Blood_Gp],[Pers_District],[Pers_Father_Name],[Pers_Gender],[Pers_Height],[Pers_House_no],[Pers_Iden_mark_1],[Pers_Iden_mark_2],[Pers_Moh_st],[Pers_Pin_code],[Pers_Police_stn],[Pers_Post_office],[Pers_Rank],[Pers_Regt],[Pers_State],[Pers_Tehsil],[Pers_UID],[Pers_Village],[Pers_birth_dt],[Pers_enrol_dt],[Pers_name] FROM [dbo].[MApiData] where [Pers_Army_No]=@ArmyNo";


            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<MApiData>(query, new { Data.ArmyNo });



                return ret.SingleOrDefault();
            }
        }

        public async Task<MApiDataOffrs> GetByoffrsIC(DTOAPIDataRequest Data)
        {
            string query = "SELECT [ApplyForId],[Pers_Army_No],[Pers_Blood_Gp],[Pers_District],[Pers_Father_Name],[Pers_Gender],[Pers_Height],[Pers_House_no],[Pers_Iden_mark_1],[Pers_Iden_mark_2],[Pers_Moh_st],[Pers_Pin_code],[Pers_Police_stn],[Pers_Post_office],[Pers_Rank],[Pers_Regt],[Pers_State],[Pers_Tehsil],[Pers_UID],[Pers_Village],[Pers_birth_dt],[Pers_enrol_dt],[Pers_name] FROM [dbo].[MApiDataOffrs] where [Pers_Army_No]=@ArmyNo";


            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<MApiDataOffrs>(query, new { Data.ArmyNo });



                return ret.SingleOrDefault();
            }
        }
    }
}
