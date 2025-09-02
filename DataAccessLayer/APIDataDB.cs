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
    /// <summary>
    /// Data Access Layer for API Data entity, providing database operations.
    /// and implements the IAPIDataDB interface.
    /// For more information, refer to the IAPIDataDB interface documentation.
    /// </summary>
    public class APIDataDB : GenericRepositoryDL<MApiData>, IAPIDataDB
    {
        protected readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly DapperContext _contextDP;// For Dapper operations

        /// <summary>
        /// Constructor to initialize the APIDataDB with necessary contexts.
        /// and logger.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dapperContext"></param>
        public APIDataDB(ApplicationDbContext context, DapperContext dapperContext) : base(context)
        {
            _context = context;
            _contextDP = dapperContext;
        }

        /// <summary>
        /// Asynchronously checks if the provided access key exists in the MApiLogin table.
        /// </summary>
        /// <param name="accessKey">The access key to be checked in the MApiLogin table.</param>
        /// <returns>
        /// Returns true if the access key exists in the table, otherwise false.
        /// </returns>
        public async Task<bool> apiLogin(string accessKey)
        {
            // SQL query to select the Id and ClientName from the MApiLogin table where the accessKey matches the provided value.
            string query = "select [Id], [ClientName] from MApiLogin where accessKey = @accessKey";

            // Using a database connection to execute the SQL query asynchronously.
            using (var connection = _contextDP.CreateConnection())
            {
                // Executing the query and passing the accessKey as a parameter to avoid SQL injection.
                var ret = await connection.QueryAsync<MApiLogin>(query, new { accessKey });

                // Check if the query returned any records. If so, return true.
                if (ret != null && ret.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;  // If no records were found, return false.
                }

                // The following line is commented out, and it would return a single record if necessary:
                // return ret.SingleOrDefault();
            }
        }


        /// <summary>
        /// Asynchronously retrieves personal data for the given Army number (Pers_Army_No) from the MApiData table, 
        /// decrypts the sensitive fields, and returns a DTOApiPersDataResponse with the data.
        /// </summary>
        /// <param name="Data">The DTOAPIDataRequest object containing the Army number for querying.</param>
        /// <returns>
        /// A DTOApiPersDataResponse object containing the personal data if the Army number exists, 
        /// otherwise returns a response with a message indicating the Army number was not found.
        /// </returns>
        public async Task<DTOApiPersDataResponse> GetByIC(DTOAPIDataRequest Data)
        {
            // SQL query to select personal data from the MApiData table for the given Army number.
            // Sensitive fields such as name, father name, birth date, etc., are decrypted using the DEcryptByPassPhrase function.
            string query = "SELECT [ApplyForId], [Pers_Army_No], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_name])) [Pers_name], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Father_Name])) [Pers_Father_Name], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_birth_dt])) [Pers_birth_dt], " +
                           "[Pers_enrol_dt], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_District])) [Pers_District], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_State])) [Pers_State], " +
                           "[Pers_Regt], [Pers_Height], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_UID])) [Pers_UID], " +
                           "[Pers_Blood_Gp], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_House_no])) [Pers_House_no], " +
                           "[Pers_Moh_st], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Village])) [Pers_Village], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Tehsil])) [Pers_Tehsil], " +
                           "CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Post_office])) [Pers_Post_office], " +
                           "[Pers_Police_stn], [Pers_Pin_code], [Pers_Iden_mark_1], [Pers_Iden_mark_2], [Pers_Gender] " +
                           "FROM [dbo].[MApiData] WHERE [Pers_Army_No] = @ArmyNo";

            // Using the database connection to execute the query and retrieve the personal data for the given Army number.
            using (var connection = _contextDP.CreateConnection())
            {
                // Execute the query asynchronously and retrieve the first matching record (if any).
                MApiData? ret = (await connection.QueryAsync<MApiData>(query, new { Data.ArmyNo })).FirstOrDefault();

                // Create a response object to store the result.
                DTOApiPersDataResponse response = new DTOApiPersDataResponse();

                // If a record was found, populate the response object with the decrypted data.
                if (ret != null)
                {
                    response.Pers_Army_No = ret.Pers_Army_No ?? string.Empty;
                    response.Pers_name = ret.Pers_name ?? string.Empty;
                    response.Pers_birth_dt = ret.Pers_birth_dt ?? string.Empty;
                    response.Pers_enrol_dt = ret.Pers_enrol_dt ?? string.Empty;

                    // Mapping address-related data
                    response.Pers_Address.Pers_House_no = ret.Pers_House_no ?? string.Empty;
                    response.Pers_Address.Pers_Moh_st = ret.Pers_Moh_st;
                    response.Pers_Address.Pers_Village = ret.Pers_Village;
                    response.Pers_Address.Pers_Tehsil = ret.Pers_Tehsil;
                    response.Pers_Address.Pers_Post_office = ret.Pers_Post_office;
                    response.Pers_Address.Pers_Police_stn = ret.Pers_Police_stn;
                    response.Pers_Address.Pers_Pin_code = ret.Pers_Pin_code;
                    response.Pers_Address.Pers_District = ret.Pers_District;
                    response.Pers_Address.Pers_State = ret.Pers_State;

                    // Status and message indicating success
                    response.Status = true;
                    response.Message = "Ok";
                }
                else
                {
                    // If no matching record is found, set status and message accordingly
                    response.Status = false;
                    response.Message = "Army No not found!";
                }

                // Return the response object.
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





