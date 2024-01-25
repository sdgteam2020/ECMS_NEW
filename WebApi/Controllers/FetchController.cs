using BusinessLogicsLayer;
using BusinessLogicsLayer.APIData;
using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class FetchController : ControllerBase
    {
        private readonly IAPIDataBL _aPIDataBL;
       
       
        public FetchController(IAPIDataBL aPIDataBL)
        {
         
            _aPIDataBL = aPIDataBL;
          
        }
        //[EnableCors("CorsPolicy")]
        //[HttpGet("{ICNumber}")]
        //public async Task<ActionResult> Get(string ICNumber)
        //{

        //    if (ICNumber != null)
        //    {
        //        UserData? userData = _userDataList.FirstOrDefault();
        //        if (userData != null)
        //        {
        //            return Ok(userData);
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }

        //}
        //[EnableCors("CorsPolicy")]
        [HttpPost]
        public async Task<ActionResult> GetData(DTOAPIDataRequest Data)
        {
            try
            {
               // MApiData data = new MApiData();
               // data = await _aPIDataBL.GetByIC(Data.ArmyNo);
                if (Data.ArmyNo != null)
                {
                    MApiData? apiData = (MApiData?)await _aPIDataBL.GetByIC(Data.ArmyNo);

                    if (apiData != null)
                    {
                        return Ok(apiData);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex) {
                return NotFound();
            }

        }
       
    }
}
