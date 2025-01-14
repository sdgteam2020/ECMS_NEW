using BusinessLogicsLayer.API;
using BusinessLogicsLayer.APIData;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class ApiController : Controller
    {
        private readonly IAPIBL _aPIBL;
        private readonly IConfiguration _configuration;
        private readonly IAPIDataBL _aPIDataBL;
        public ApiController(IAPIBL aPIBL, IConfiguration configuration, IAPIDataBL aPIDataBL)
        {
            _aPIBL = aPIBL;
            _configuration = configuration;
            _aPIDataBL = aPIDataBL;
        }

        public async Task<IActionResult> LoginApi(string ICNumber, int Type)
        {
            bool FromApiJCO = Convert.ToBoolean(_configuration["ApiCall:FromApiJCO"]);
            bool FromApiOffr = Convert.ToBoolean(_configuration["ApiCall:FromApiOffr"]);
            DTOApiPersDataResponse res1 = new DTOApiPersDataResponse();
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            DTOAPILoginRequest data = new DTOAPILoginRequest();
            if (FromApiOffr == true && Type == 1)
            {
                data.LoginUrl = _configuration["ApiCall:ApioffsLoginUrl"] ?? string.Empty;
                data.ApiUrl = _configuration["ApiCall:OffrsApiUrl"] ?? string.Empty;
                data.accessKey = _configuration["ApiCall:ApiaccessKeyOffrApiUrl"] ?? string.Empty;

                var ret = await _aPIBL.Getauthentication(data);


                if (ret != null)
                {
                    DTOPersDataRequest retdat = new DTOPersDataRequest();
                    retdat.Pers_Army_No = ICNumber;
                    retdat.jwt = ret.token;
                    retdat.ApplyForId = Type;

                    retdat.ApiUrl = _configuration["ApiCall:OffrsApiUrl"] ?? string.Empty;
                    var res = await _aPIBL.GetData(retdat);
                    if (res != null)
                    {
                        if (res.pers_Army_No != null)
                        {
                            res.Status = true;
                            res.Message = "OK";

                            res1 = res;
                        }
                        else
                        {
                            res.Status = false;
                            res.Message = "Not Fetch Data From Api";

                            res1 = res;
                        }
                    }
                    else
                    {
                        res1.Status = false;
                        res1.Message = "Not Fetch Data From Api";
                    }

                }
                return Json(res1);
            }
            else if (FromApiJCO == true && Type == 2)
            {

                data.LoginUrl = _configuration["ApiCall:ApiJcoLoginUrl"] ?? string.Empty;
                data.ApiUrl = _configuration["ApiCall:JCOApiUrl"] ?? string.Empty;
                data.accessKey = _configuration["ApiCall:ApiaccessKeyJCOApiUrl"] ?? string.Empty;

                var ret = await _aPIBL.Getauthentication(data);


                if (ret != null)
                {
                    DTOPersDataRequest retdat = new DTOPersDataRequest();
                    retdat.Pers_Army_No = ICNumber;
                    retdat.jwt = ret.token;
                    retdat.ApplyForId = Type;
                    retdat.ApiUrl = _configuration["ApiCall:JCOApiUrl"] ?? string.Empty;
                    var res = await _aPIBL.GetData(retdat);
                    if (res != null)
                    {
                        if (res.pers_Army_No != null)
                        {
                            res.Status = true;
                            res.Message = "OK";

                            res1 = res;
                        }
                        else
                        {
                            res.Status = false;
                            res.Message = "Data Could not be Fetched From Data Records";

                            res1 = res;
                        }
                    }
                    else
                    {
                        res1.Status = false;
                        res1.Message = "Data Could not be Fetched From Data Records ";
                    }

                }
                return Json(res1);
            }
            else
            {

                DTOAPIDataRequest retdat = new DTOAPIDataRequest();
                retdat.ArmyNo = ICNumber;

                if (Type == 2)
                {
                    var res = await _aPIDataBL.GetByIC(retdat);
                    if (res != null)
                    {
                        if (res.pers_Army_No != null)
                        {
                            res.Status = true;
                            res.Message = "OK";

                            return Json(res);
                        }
                        else
                        {
                            res.Status = false;
                            res.Message = "Data Could not be Fetched From Data Records";

                            return Json(res);
                        }
                    }
                    else
                    {
                        res1.Status = false;
                        res1.Message = "Data Could not be Fetched From Data Records ";
                        return Json(res1);
                    }

                }
                else
                {
                    var res = await _aPIDataBL.GetByoffrsIC(retdat);
                    if (res != null)
                    {
                        if (res.pers_Army_No != null)
                        {
                            res.Status = true;
                            res.Message = "OK";

                            return Json(res);
                        }
                        else
                        {
                            res.Status = false;
                            res.Message = "Not Fetch Data From Api";

                            return Json(res);
                        }
                    }
                    else
                    {
                        res1.Status = false;
                        res1.Message = "Not Fetch Data From Api";
                        return Json(res1);
                    }
                }

            }

        }
    }
}
