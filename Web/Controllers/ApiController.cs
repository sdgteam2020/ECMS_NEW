using BusinessLogicsLayer.API;
using BusinessLogicsLayer.Bde;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Web.Controllers
{
    public class ApiController : Controller
    {
        private readonly IAPIBL _aPIBL;
        public ApiController(IAPIBL aPIBL)
        {
            _aPIBL=aPIBL;
        }
       
        public async Task<IActionResult> LoginApi(string ICNumber)
        {
            DTOApiPersDataResponse res1=new DTOApiPersDataResponse();  
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            DTOAPILoginRequest data = new DTOAPILoginRequest();
            data.ClientName = "admin";
            data.ClientKey = "";
            data.ClientIP = remoteIpAddress.ToString();
            data.ClientURL = "";
            data.ClientPW = "123";
            //data.email = "devopstasking@gmail.com";
            //data.password = "Admin@123";
            var ret = await _aPIBL.Getauthentication(data);
          
           
            if (ret != null)
            {
                DTOPersDataRequest retdat = new DTOPersDataRequest();
                retdat.Pers_Army_No = ICNumber;
                retdat.jwt=ret.token;
                

                // ret.timestamp = DateTime.Today.ToString("dd-MMM-yy", CultureInfo.InvariantCulture);
               var res =await _aPIBL.GetData(retdat);
                if (res != null)
                {
                    res.Status = true;
                    res.Message = "OK";

                    res1=res;
                }
                else
                {
                    res1.Status = false;
                    res1.Message = "Not Fetach Data From Api";
                }
            }

            return Json(res1);
        }
    }
}
