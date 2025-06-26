using BusinessLogicsLayer.API;
using BusinessLogicsLayer.APIData;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Healpers;

namespace Web.Controllers
{
    [Authorize]
    public class ApiController : Controller
    {
        private readonly IaPiBl _aPIBL;
        private readonly IConfiguration _configuration;
        private readonly IapiDataBl _aPIDataBL;
        public ApiController(IaPiBl aPIBL, IConfiguration configuration, IapiDataBl aPIDataBL)
        {
            _aPIBL = aPIBL;
            _configuration = configuration;
            _aPIDataBL = aPIDataBL;
        }

        public async Task<IActionResult> LoginApi(string ICNumber, int Type)
        {
            DTOApiPersDataResponse res1 = new DTOApiPersDataResponse();
            try
            {
                bool FromApiJCO = Convert.ToBoolean(_configuration["ApiCall:FromApiJCO"]);
                bool FromApiOffr = Convert.ToBoolean(_configuration["ApiCall:FromApiOffr"]);

                var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
                DTOAPILoginRequest data = new DTOAPILoginRequest();
                if (FromApiOffr == true && Type == 1)
                {
                    data.LoginUrl = _configuration["ApiCall:ApioffsLoginUrl"] ?? string.Empty;
                    data.ApiUrl = _configuration["ApiCall:OffrsApiUrl"] ?? string.Empty;
                    data.accessKey = _configuration["ApiCall:ApiaccessKeyOffrApiUrl"] ?? string.Empty;

                    DTOLoginAPIResponse ret = await _aPIBL.Getauthentication(data);


                    if (ret.Status == true)
                    {
                        ApiHelpers apiHelpers = new ApiHelpers();
                        DTOPersDataRequest retdat = new DTOPersDataRequest();
                        retdat.Pers_Army_No = ICNumber;
                        retdat.jwt = ret.token;
                        retdat.ApplyForId = Type;
                        retdat.PubKey = apiHelpers.GetHashValue("EISAC_OFFR");
                        string PubKeyForDesc = retdat.Pers_Army_No.Substring(retdat.Pers_Army_No.Length - 4, 4);
                        retdat.ApiUrl = _configuration["ApiCall:OffrsApiUrl"] ?? string.Empty;
                        DTOApiPersDataResponse res = await _aPIBL.GetData(retdat);

                        if (res.Status == true)
                        {
                            res.Pers_Army_No = apiHelpers.EncDec(res.Pers_Army_No, PubKeyForDesc, false);
                            res.Pers_name = apiHelpers.EncDec(res.Pers_name, PubKeyForDesc, false);
                            res.Pers_birth_dt = apiHelpers.EncDec(res.Pers_birth_dt, PubKeyForDesc, false);
                            res.Pers_enrol_dt = apiHelpers.EncDec(res.Pers_enrol_dt, PubKeyForDesc, false);
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

                            res.Pers_Address.Pers_House_no = apiHelpers.EncDec(res.Pers_Address.Pers_House_no, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Moh_st = apiHelpers.EncDec(res.Pers_Address.Pers_Moh_st, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Village = apiHelpers.EncDec(res.Pers_Address.Pers_Village, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Tehsil = apiHelpers.EncDec(res.Pers_Address.Pers_Tehsil, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Post_office = apiHelpers.EncDec(res.Pers_Address.Pers_Post_office, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Police_stn = apiHelpers.EncDec(res.Pers_Address.Pers_Police_stn, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Pin_code = apiHelpers.EncDec(res.Pers_Address.Pers_Pin_code, PubKeyForDesc, false);
                            res.Pers_Address.Pers_District = apiHelpers.EncDec(res.Pers_Address.Pers_District, PubKeyForDesc, false);
                            res.Pers_Address.Pers_State = apiHelpers.EncDec(res.Pers_Address.Pers_State, PubKeyForDesc, false);

                            //res.pers_Rank = apiHelpers.EncDec(res.pers_Rank, PubKeyForDesc, false);
                            //res.pers_Father_Name = apiHelpers.EncDec(res.pers_Father_Name, PubKeyForDesc, false);
                            //res.pers_Address.pers_Regt = apiHelpers.EncDec(res.pers_Regt, PubKeyForDesc, false);
                            //res.pers_Address.pers_Height = apiHelpers.EncDec(res.pers_Height, PubKeyForDesc, false);
                            //res.pers_Address.pers_UID = apiHelpers.EncDec(res.pers_UID, PubKeyForDesc, false);
                            //res.pers_Address.pers_Blood_Gp = apiHelpers.EncDec(res.pers_Blood_Gp, PubKeyForDesc, false);
                            //res.pers_Address.pers_Iden_mark_1 = apiHelpers.EncDec(res.pers_Iden_mark_1, PubKeyForDesc, false);
                            //res.pers_Address.pers_Iden_mark_2 = apiHelpers.EncDec(res.pers_Iden_mark_2, PubKeyForDesc, false);
                            //res.pers_Address.pers_Gender = apiHelpers.EncDec(res.pers_Gender, PubKeyForDesc, false);
                            
                            res.Message = "OK";
                            res1 = res;
                        }
                        else
                        {
                            res1.Message = res.Message;
                        }

                    }
                    else
                    {
                        res1.Message = ret.Message;
                    }
                    return Json(res1);
                }
                else if (FromApiJCO == true && Type == 2)
                {

                    data.LoginUrl = _configuration["ApiCall:ApiJcoLoginUrl"] ?? string.Empty;
                    data.ApiUrl = _configuration["ApiCall:JCOApiUrl"] ?? string.Empty;
                    data.accessKey = _configuration["ApiCall:ApiaccessKeyJCOApiUrl"] ?? string.Empty;

                    DTOLoginAPIResponse ret = await _aPIBL.Getauthentication(data);


                    if (ret.Status == true)
                    {
                        ApiHelpers apiHelpers = new ApiHelpers();
                        DTOPersDataRequest retdat = new DTOPersDataRequest();
                        retdat.Pers_Army_No = ICNumber;
                        retdat.jwt = ret.token;
                        retdat.ApplyForId = Type;
                        retdat.PubKey = apiHelpers.GetHashValue("EISAC");
                        string PubKeyForDesc = retdat.Pers_Army_No.Substring(retdat.Pers_Army_No.Length - 4, 4);
                        retdat.ApiUrl = _configuration["ApiCall:JCOApiUrl"] ?? string.Empty;
                        DTOApiPersDataResponse? res = await _aPIBL.GetData(retdat);

                        if (res.Status == true)
                        {
                            res.Pers_Army_No = apiHelpers.EncDec(res.Pers_Army_No, PubKeyForDesc, false);
                            res.Pers_name = apiHelpers.EncDec(res.Pers_name, PubKeyForDesc, false);
                            res.Pers_birth_dt = apiHelpers.EncDec(res.Pers_birth_dt, PubKeyForDesc, false);
                            res.Pers_enrol_dt = apiHelpers.EncDec(res.Pers_enrol_dt, PubKeyForDesc, false);
                            res.Pers_Address.Pers_House_no = apiHelpers.EncDec(res.Pers_Address.Pers_House_no, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Moh_st = apiHelpers.EncDec(res.Pers_Address.Pers_Moh_st, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Village = apiHelpers.EncDec(res.Pers_Address.Pers_Village, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Tehsil = apiHelpers.EncDec(res.Pers_Address.Pers_Tehsil, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Post_office = apiHelpers.EncDec(res.Pers_Address.Pers_Post_office, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Police_stn = apiHelpers.EncDec(res.Pers_Address.Pers_Police_stn, PubKeyForDesc, false);
                            res.Pers_Address.Pers_Pin_code = apiHelpers.EncDec(res.Pers_Address.Pers_Pin_code, PubKeyForDesc, false);
                            res.Pers_Address.Pers_District = apiHelpers.EncDec(res.Pers_Address.Pers_District, PubKeyForDesc, false);
                            res.Pers_Address.Pers_State = apiHelpers.EncDec(res.Pers_Address.Pers_State, PubKeyForDesc, false);

                            //res.pers_Rank = apiHelpers.EncDec(res.pers_Rank, PubKeyForDesc, false);
                            //res.pers_Father_Name = apiHelpers.EncDec(res.pers_Father_Name, PubKeyForDesc, false);
                            //res.pers_Address.pers_Regt = apiHelpers.EncDec(res.pers_Regt, PubKeyForDesc, false);
                            //res.pers_Address.pers_Height = apiHelpers.EncDec(res.pers_Height, PubKeyForDesc, false);
                            //res.pers_Address.pers_UID = apiHelpers.EncDec(res.pers_UID, PubKeyForDesc, false);
                            //res.pers_Address.pers_Blood_Gp = apiHelpers.EncDec(res.pers_Blood_Gp, PubKeyForDesc, false);
                            //res.pers_Address.pers_Iden_mark_1 = apiHelpers.EncDec(res.pers_Iden_mark_1, PubKeyForDesc, false);
                            //res.pers_Address.pers_Iden_mark_2 = apiHelpers.EncDec(res.pers_Iden_mark_2, PubKeyForDesc, false);
                            //res.pers_Address.pers_Gender = apiHelpers.EncDec(res.pers_Gender, PubKeyForDesc, false);
                            res.Message = "OK";
                            res1 = res;
                        }
                        else
                        {
                            res1.Message = res.Message;
                        }

                    }
                    else
                    {
                        res1.Message = ret.Message;
                    }
                    return Json(res1);
                }
                else
                {

                    DTOAPIDataRequest retdat = new DTOAPIDataRequest();
                    retdat.ArmyNo = ICNumber;

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
                res1.Status = false;
                res1.Message = ex.Message;
                return Json(res1);
            }

        }
    }
}
