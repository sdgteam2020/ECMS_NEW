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
            DTOApiPersDataResponse res=new DTOApiPersDataResponse();  
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            DTOAPILoginRequest data = new DTOAPILoginRequest();
            data.ClientName = "miso";
            data.ClientKey  ="";
            data.ClientIP   = remoteIpAddress.ToString();
            data.ClientURL  ="";
            data.ClientPW   = "";

            var ret = await _aPIBL.Getauthentication(data);
          
           
            if (ret != null)
            {
                DTOPersDataRequest retdat = new DTOPersDataRequest();
                retdat.Pers_Army_No = ICNumber;
                retdat.jwt=ret.jwt;
                

                // ret.timestamp = DateTime.Today.ToString("dd-MMM-yy", CultureInfo.InvariantCulture);
                res =await _aPIBL.GetData(retdat);
                if (res != null)
                {
                    res.Status = true;
                    res.Message = "OK";
                }
                else
                {
                    
                    res.Message = "Not Fetach Data From Api";
                }
            }

            return Json(res);
        }
    }
}
