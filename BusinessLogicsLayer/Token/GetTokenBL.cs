using DataTransferObject.Requests;
using DataTransferObject.Response;
using ModernHttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Token
{
    public class GetTokenBL : iGetTokenBL
    {
        public static HttpResponseMessage result;
     
        public const string ApiUrl = "http://localhost/Temporary_Listen_Addresses/";
        public async Task<List<DTOTokenResponse>> GetTokenDetails(string ApiName)
        {
            try
            {

                List<DTOTokenResponse> dynamicResponseDTO = new List<DTOTokenResponse>();



                HttpResponseMessage result = null;
                await GetAPI(ApiName, "").ContinueWith(task =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        result = task.Result;
                    }
                });



                if (result != null)
                {

                    // dynamicResponseDTO = result.Content.ReadAsAsync<DTOLoginResponse>().Result;

                    string responseBody = await result.Content.ReadAsStringAsync();
                    dynamicResponseDTO = JsonSerializer.Deserialize<List<DTOTokenResponse>>(responseBody);

                }
                List<DTOTokenResponse> retdata = new List<DTOTokenResponse>();
                foreach (var data in dynamicResponseDTO)
                {
                    DTOTokenResponse db=new DTOTokenResponse();
                    if (data.subject !=null)
                    {
                        var subdata = data.subject.Split(",");
                   
                        db.Name = subdata[0].Replace("CN=", "");
                        db.ArmyNo = subdata[1].Replace("SERIALNUMBER=", "");
                    }
                    db.Status = data.Status;
                    db.Remarks = data.Remarks;
                    retdata.Add(db);
                }

                return retdata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static async Task<HttpResponseMessage> PostAPI<T>(string url, T data)
        {
            try
            {

                HttpClient httpClient = new HttpClient(new NativeMessageHandler());
                HttpResponseMessage s = await httpClient.PostAsJsonAsync(ApiUrl + url, data);
                return s;
            }
            catch (Exception ex)
            {
                _ = ex;
                return null;
            }
        }
        public static async Task<HttpResponseMessage> GetAPI<T>(string url, T Data)
        {
            try
            {

                HttpClient httpClient = new HttpClient(new NativeMessageHandler());
                HttpResponseMessage s = await httpClient.GetAsync(ApiUrl + url);
                return s;
            }
            catch (Exception ex)
            {
                _ = ex;
                return null;
            }
        }
        //public async Task<DTOTokenResponse> GetIsToken(DTOTokenRequest token)
        //{
        //    try
        //    {
        //        token.ICNo = token.ICNo.Replace("-", "");
        //        DTOTokenResponse dynamicResponseDTO = new DTOTokenResponse();



        //        HttpResponseMessage result = null;
        //        await ApiHelpers.PostAPI(url, token).ContinueWith(task =>
        //        {
        //            if (task.Status == TaskStatus.RanToCompletion)
        //            {
        //                result = task.Result;
        //            }
        //        });



        //        if (result != null)
        //        {

        //            // dynamicResponseDTO = result.Content.ReadAsAsync<DTOLoginResponse>().Result;

        //            string responseBody = await result.Content.ReadAsStringAsync();
        //            dynamicResponseDTO = JsonSerializer.Deserialize<DTOTokenResponse>(responseBody);

        //        }
        //        return dynamicResponseDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}
    }
}
