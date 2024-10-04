using BusinessLogicsLayer.API;
using BusinessLogicsLayer.APIData;
using BusinessLogicsLayer.Bde;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using System.Globalization;

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
            _aPIBL=aPIBL;
            _configuration=configuration;
            _aPIDataBL=aPIDataBL;
        }
       
        public async Task<IActionResult> LoginApi(string ICNumber,int Type)
        {
           bool FromApi= Convert.ToBoolean(_configuration["ApiCall:FromApi"]);
            DTOApiPersDataResponse res1=new DTOApiPersDataResponse();  
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            DTOAPILoginRequest data = new DTOAPILoginRequest();
          
            //data.email = "devopstasking@gmail.com";
            //data.password = "Admin@123";
            if (FromApi == true)
            {
                if(Type == 2)
                {
                    data.ClientName = _configuration["ApiCall:JCOClientName"];
                    data.ClientKey = _configuration["ApiCall:JCOClientKey"];
                    data.ClientIP = _configuration["ApiCall:JCOClientIP"];
                    data.ClientURL = _configuration["ApiCall:JCOClientURL"];
                    data.ClientPW = _configuration["ApiCall:JCOClientPW"];
                    data.ApiUrl= _configuration["ApiCall:JCOApiUrl"];
                }
                else if (Type == 1)
                {
                    data.ClientName = _configuration["ApiCall:OffrsClientName"];
                    data.ClientKey = _configuration["ApiCall:OffrsClientKey"];
                    data.ClientIP = _configuration["ApiCall:OffrsClientIP"];
                    data.ClientURL = _configuration["ApiCall:OffrsClientURL"];
                    data.ClientPW = _configuration["ApiCall:OffrsClientPW"];
                    data.ApiUrl = _configuration["ApiCall:OffrsApiUrl"];
                }
                var ret = await _aPIBL.Getauthentication(data);


                if (ret != null)
                {
                    DTOPersDataRequest retdat = new DTOPersDataRequest();
                    retdat.Pers_Army_No = ICNumber;
                    retdat.jwt = ret.token;
                    retdat.ApplyForId = Type;



                    if (retdat.ApplyForId == 2)
                    {
                        retdat.ApiUrl = _configuration["ApiCall:JCOApiUrl"];
                        var res = await _aPIBL.GetData(retdat);
                        if (res != null)
                        {
                            if (res.Pers_Army_No != null)
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
                    else
                    {
                        retdat.ApiUrl = _configuration["ApiCall:OffrsApiUrl"];
                        var res = await _aPIBL.GetDataOffrs(retdat);
                        if (res != null)
                        {
                            if (res.Pers_Army_No != null)
                            {
                                res.Status = true;
                                res.Message = "OK";

                                res1 = res;
                            }
                            else
                            {
                                res.Status = false;
                                res.Message = "Not Fetach Data From Api";

                                res1 = res;
                            }
                        }
                        else
                        {
                            res1.Status = false;
                            res1.Message = "Not Fetach Data From Api";
                        }
                    }
                }
                return Json(res1);
            }
            else
            {

                DTOAPIDataRequest retdat = new DTOAPIDataRequest();
                    retdat.ArmyNo = ICNumber;
                    retdat.ApplyForId = Type;



                    if (retdat.ApplyForId == 2)
                    {
                        var res = await _aPIDataBL.GetByIC(retdat);
                        if (res != null)
                        {
                            if (res.Pers_Army_No != null)
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
                            if (res.Pers_Army_No != null)
                            {
                                res.Status = true;
                                res.Message = "OK";

                            return Json(res);
                        }
                            else
                            {
                                res.Status = false;
                                res.Message = "Not Fetach Data From Api";

                            return Json(res);
                        }
                        }
                        else
                        {
                            res1.Status = false;
                            res1.Message = "Not Fetach Data From Api";
                        return Json(res1);
                    }
                    }
                
            }
           
        }
    }
}
