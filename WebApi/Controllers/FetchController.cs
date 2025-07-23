using BusinessLogicsLayer;
using BusinessLogicsLayer.APIData;
using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
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
        private readonly IapiDataBl _aPIDataBL;
       
       
        public FetchController(IapiDataBl aPIDataBL)
        {
         
            _aPIDataBL = aPIDataBL;
          
        }
        
        [HttpPost]
        public async Task<ActionResult> GetData(DTOAPIDataRequest Data)
        {
            try
            {
               // MApiData data = new MApiData();
               // data = await _aPIDataBL.GetByIC(Data.ArmyNo);
                if (Data.ArmyNo != null)
                {
                    DTOApiPersDataResponse apiData = await _aPIDataBL.GetByIC(Data);

                    if (apiData.Status == true)
                    {
                        return Ok(apiData);
                    }
                    else
                    {
                        return Ok(apiData);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex) {
                return NotFound(ex.Message);
            }

        }
        [HttpPost]
        public async Task<ActionResult> GetDataoffrs(DTOAPIDataRequest Data)
        {
            try
            {
                // MApiData data = new MApiData();
                // data = await _aPIDataBL.GetByIC(Data.ArmyNo);
                if (Data.ArmyNo != null)
                {
                    DTOApiPersDataResponse apiData = await _aPIDataBL.GetByoffrsIC(Data);

                    if (apiData.Status == true)
                    {
                        return Ok(apiData);
                    }
                    else
                    {
                        return Ok(apiData);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

    }
}
