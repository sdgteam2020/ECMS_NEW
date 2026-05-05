using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer
{
    /// <summary>
    /// Data Access Layer for API Data entity, providing database operations.
    /// and implements the IAPIDataDB interface.
    /// For more information, refer to the IAPIDataDB interface documentation.
    /// </summary>
    public class APIDataDB : GenericRepositoryDL<MApiData>, IAPIDataDB
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
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
            string query = @"SELECT [ApplyForId], [Pers_Army_No], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_name])) [Pers_name], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_birth_dt])) [Pers_birth_dt], 
                            [Pers_enrol_dt], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_District])) [Pers_District], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_State])) [Pers_State], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_House_no])) [Pers_House_no], 
                            [Pers_Moh_st], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Village])) [Pers_Village], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Tehsil])) [Pers_Tehsil], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Post_office])) [Pers_Post_office], 
                            [Pers_Police_stn], [Pers_Pin_code]
                            FROM [dbo].[MApiData] WHERE [Pers_Army_No] = @ArmyNo";

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


        /// <summary>
        /// Asynchronously retrieves personal data for the given Army number (Pers_Army_No) from the MApiDataOffrs table, 
        /// decrypts the sensitive fields, and returns a DTOApiPersDataResponse with the data.
        /// </summary>
        /// <param name="Data">The DTOAPIDataRequest object containing the Army number for querying.</param>
        /// <returns>
        /// A DTOApiPersDataResponse object containing the personal data if the Army number exists, 
        /// otherwise returns a response with a message indicating the Army number was not found.
        /// </returns>
        public async Task<DTOApiPersDataResponse> GetByoffrsIC(DTOAPIDataRequest Data)
        {
            // SQL query to select personal data from the MApiDataOffrs table for the given Army number.
            string query = @"SELECT [ApplyForId], [Pers_Army_No], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_name])) [Pers_name], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_birth_dt])) [Pers_birth_dt], 
                            [Pers_enrol_dt], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_District])) [Pers_District], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_State])) [Pers_State], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_House_no])) [Pers_House_no], 
                            [Pers_Moh_st], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Village])) [Pers_Village], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Tehsil])) [Pers_Tehsil], 
                            CONVERT(nvarchar(MAX), DEcryptByPassPhrase('ASDC@123', [Pers_Post_office])) [Pers_Post_office], 
                            [Pers_Police_stn], [Pers_Pin_code]
                            FROM [dbo].[MApiDataOffrs] WHERE [Pers_Army_No] = @ArmyNo";

            // Using the database connection to execute the query and retrieve the personal data for the given Army number.
            using (var connection = _contextDP.CreateConnection())
            {
                // Execute the query asynchronously and retrieve the first matching record (if any).
                MApiDataOffrs? ret = (await connection.QueryAsync<MApiDataOffrs>(query, new { Data.ArmyNo })).FirstOrDefault();

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

    }
}





