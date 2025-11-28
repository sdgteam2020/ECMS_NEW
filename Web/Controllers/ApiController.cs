using BusinessLogicsLayer.API;
using BusinessLogicsLayer.APIData;
using BusinessLogicsLayer.EncryptionSetting;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Healpers;

namespace Web.Controllers
{
    /// <summary>
    /// This controller handles API-related operations, including user authentication and data retrieval for officers and JCOs.
    /// </summary>
    [Authorize]
    public class ApiController : Controller
    {
        private readonly IaPiBl _aPIBL;
        private readonly IConfiguration _configuration;
        private readonly IapiDataBl _aPIDataBL;
        private readonly IEncryptionSettingBL encryptionSettingBL;// For Encryption Setting
        public ApiController(IaPiBl aPIBL, IConfiguration configuration, IapiDataBl aPIDataBL, IEncryptionSettingBL encryptionSettingBL)
        {
            _aPIBL = aPIBL;
            _configuration = configuration;
            _aPIDataBL = aPIDataBL;
            this.encryptionSettingBL = encryptionSettingBL;
        }

        /// <summary>
        /// Handles login authentication for different types (officers and JCOs) by making API calls to external services.
        /// Depending on the type, it processes the API response, decrypts sensitive data, and returns the user details.
        /// </summary>
        /// <param name="ICNumber">The IC number for the individual.</param>
        /// <param name="Type">The type of user: 1 for officers, 2 for JCOs.</param>
        /// <returns>A JSON response containing the user details or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> LoginApi(string ICNumber, int Type)
        {
            DTOApiPersDataResponse res1 = new DTOApiPersDataResponse();
            string Pk = string.Empty;
            try
            {
                // Retrieve the encryption key from the database
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    Pk = keyRecord.PrivateKeyForApi; // Assign the private key from the database record
                }
                else
                {
                    throw new InvalidOperationException("Encryption key record not found."); // Throw error if key record is missing
                }
                // Retrieve configuration values from appsettings.json
                bool FromApiJCO = Convert.ToBoolean(_configuration["ApiCall:FromApiJCO"]);
                bool FromApiOffr = Convert.ToBoolean(_configuration["ApiCall:FromApiOffr"]);

                // Get the remote IP address from the HTTP context
                var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;

                // Initialize login request data object
                DTOAPILoginRequest data = new DTOAPILoginRequest();

                // Check if the user is an officer (Type 1) and process accordingly
                if (FromApiOffr == true && Type == 1)
                {
                    // Configure API URLs and access key for officers
                    data.LoginUrl = _configuration["ApiCall:ApioffsLoginUrl"] ?? string.Empty;
                    data.ApiUrl = _configuration["ApiCall:OffrsApiUrl"] ?? string.Empty;
                    data.accessKey = _configuration["ApiCall:ApiaccessKeyOffrApiUrl"] ?? string.Empty;

                    // Make authentication API call for officers
                    DTOLoginAPIResponse ret = await _aPIBL.Getauthentication(data);

                    // If authentication is successful, retrieve user data
                    if (ret.Status == true)
                    {
                        // Create an API helper for decryption operations
                        ApiHelpers apiHelpers = new ApiHelpers();
                        DTOPersDataRequest retdat = new DTOPersDataRequest();
                        retdat.Pers_Army_No = ICNumber;
                        retdat.jwt = ret.token;
                        retdat.ApplyForId = Type;
                        retdat.PubKey = apiHelpers.GetHashValue("EISAC_OFFR");

                        // Get the last 4 digits of the IC number for decryption purposes
                        string PubKeyForDesc = retdat.Pers_Army_No.Substring(retdat.Pers_Army_No.Length - 4, 4);
                        retdat.ApiUrl = _configuration["ApiCall:OffrsApiUrl"] ?? string.Empty;

                        // Call to retrieve personnel data
                        DTOApiPersDataResponse res = await _aPIBL.GetData(retdat);

                        // If data retrieval is successful, decrypt and format the information
                        if (res.Status == true)
                        {
                            // Decrypt sensitive fields using the public key
                            res.Pers_Army_No = apiHelpers.EncDec(res.Pers_Army_No, PubKeyForDesc, Pk, false);
                            res.Pers_name = apiHelpers.EncDec(res.Pers_name, PubKeyForDesc, Pk, false);
                            res.Pers_birth_dt = apiHelpers.EncDec(res.Pers_birth_dt, PubKeyForDesc, Pk, false);
                            res.Pers_enrol_dt = apiHelpers.EncDec(res.Pers_enrol_dt, PubKeyForDesc, Pk, false);

                            // Try parsing dates and format them if successful
                            DateTime DOB, DOC;
                            bool result = DateTime.TryParse(res.Pers_birth_dt, out DOB);
                            if (result)
                            {
                                res.Pers_birth_dt = DOB.ToString("yyyy-MM-dd");
                            }
                            result = DateTime.TryParse(res.Pers_enrol_dt, out DOC);
                            if (result)
                            {
                                res.Pers_enrol_dt = DOC.ToString("yyyy-MM-dd");
                            }

                            // Decrypt address fields
                            res.Pers_Address.Pers_House_no = apiHelpers.EncDec(res.Pers_Address.Pers_House_no, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Moh_st = apiHelpers.EncDec(res.Pers_Address.Pers_Moh_st, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Village = apiHelpers.EncDec(res.Pers_Address.Pers_Village, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Tehsil = apiHelpers.EncDec(res.Pers_Address.Pers_Tehsil, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Post_office = apiHelpers.EncDec(res.Pers_Address.Pers_Post_office, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Police_stn = apiHelpers.EncDec(res.Pers_Address.Pers_Police_stn, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Pin_code = apiHelpers.EncDec(res.Pers_Address.Pers_Pin_code, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_District = apiHelpers.EncDec(res.Pers_Address.Pers_District, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_State = apiHelpers.EncDec(res.Pers_Address.Pers_State, PubKeyForDesc, Pk, false);

                            res.Message = "OK"; // Set the success message
                            res1 = res; // Return the decrypted response data
                        }
                        else
                        {
                            // If data retrieval failed, set the failure message
                            res1.Message = res.Message;
                        }
                    }
                    else
                    {
                        // If authentication failed, return the failure message
                        res1.Message = ret.Message;
                    }
                    return Json(res1);
                }
                // Check if the user is a JCO (Type 2) and process accordingly
                else if (FromApiJCO == true && Type == 2)
                {
                    // Configure API URLs and access key for JCOs
                    data.LoginUrl = _configuration["ApiCall:ApiJcoLoginUrl"] ?? string.Empty;
                    data.ApiUrl = _configuration["ApiCall:JCOApiUrl"] ?? string.Empty;
                    data.accessKey = _configuration["ApiCall:ApiaccessKeyJCOApiUrl"] ?? string.Empty;

                    // Make authentication API call for JCOs
                    DTOLoginAPIResponse ret = await _aPIBL.Getauthentication(data);

                    // If authentication is successful, retrieve user data
                    if (ret.Status == true)
                    {
                        ApiHelpers apiHelpers = new ApiHelpers();
                        DTOPersDataRequest retdat = new DTOPersDataRequest();
                        retdat.Pers_Army_No = ICNumber;
                        retdat.jwt = ret.token;
                        retdat.ApplyForId = Type;
                        retdat.PubKey = apiHelpers.GetHashValue("EISAC");

                        // Get the last 4 digits of the IC number for decryption purposes
                        string PubKeyForDesc = retdat.Pers_Army_No.Substring(retdat.Pers_Army_No.Length - 4, 4);
                        retdat.ApiUrl = _configuration["ApiCall:JCOApiUrl"] ?? string.Empty;

                        // Call to retrieve personnel data
                        DTOApiPersDataResponse? res = await _aPIBL.GetData(retdat);

                        // If data retrieval is successful, decrypt and format the information
                        if (res.Status == true)
                        {
                            // Decrypt sensitive fields using the public key
                            res.Pers_Army_No = apiHelpers.EncDec(res.Pers_Army_No, PubKeyForDesc, Pk, false);
                            res.Pers_name = apiHelpers.EncDec(res.Pers_name, PubKeyForDesc, Pk, false);
                            res.Pers_birth_dt = apiHelpers.EncDec(res.Pers_birth_dt, PubKeyForDesc, Pk, false);
                            res.Pers_enrol_dt = apiHelpers.EncDec(res.Pers_enrol_dt, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_House_no = apiHelpers.EncDec(res.Pers_Address.Pers_House_no, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Moh_st = apiHelpers.EncDec(res.Pers_Address.Pers_Moh_st, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Village = apiHelpers.EncDec(res.Pers_Address.Pers_Village, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Tehsil = apiHelpers.EncDec(res.Pers_Address.Pers_Tehsil, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Post_office = apiHelpers.EncDec(res.Pers_Address.Pers_Post_office, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Police_stn = apiHelpers.EncDec(res.Pers_Address.Pers_Police_stn, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_Pin_code = apiHelpers.EncDec(res.Pers_Address.Pers_Pin_code, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_District = apiHelpers.EncDec(res.Pers_Address.Pers_District, PubKeyForDesc, Pk, false);
                            res.Pers_Address.Pers_State = apiHelpers.EncDec(res.Pers_Address.Pers_State, PubKeyForDesc, Pk, false);

                            res.Message = "OK"; // Set the success message
                            res1 = res; // Return the decrypted response data
                        }
                        else
                        {
                            // If data retrieval failed, set the failure message
                            res1.Message = res.Message;
                        }
                    }
                    else
                    {
                        // If authentication failed, return the failure message
                        res1.Message = ret.Message;
                    }
                    return Json(res1);
                }
                else
                {
                    // Handle the case for non-officer and non-JCO types
                    DTOAPIDataRequest retdat = new DTOAPIDataRequest();
                    retdat.ArmyNo = ICNumber;

                    // Process request for officers (Type 2)
                    if (Type == 2)
                    {
                        DTOApiPersDataResponse res = await _aPIDataBL.GetByIC(retdat);
                        if (res.Status == true)
                        {
                            return Json(res);
                        }
                        else
                        {
                            res1.Status = res.Status;
                            res1.Message = res.Message;
                            return Json(res1);
                        }
                    }
                    else
                    {
                        // Process request for JCOs (Type 1)
                        DTOApiPersDataResponse res = await _aPIDataBL.GetByoffrsIC(retdat);
                        if (res.Status == true)
                        {
                            return Json(res);
                        }
                        else
                        {
                            res1.Status = res.Status;
                            res1.Message = res.Message;
                            return Json(res1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                res1.Status = false;
                res1.Message = ex.Message;
                return Json(res1);
            }
        }

    }
}
