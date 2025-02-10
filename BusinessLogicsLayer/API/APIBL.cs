using BusinessLogicsLayer.Helpers;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.API
{
    public class APIBL : IAPIBL
    {
        //public const string ApiUrl = "https://localhost:7002/api/";
        //public const string ApiUrloffrs = "https://localhost:7002/api/";

        //public const string ApiUrl = "https://192.168.10.203:8443/api/";
        //public const string ApiUrloffrs = "https://192.168.10.203:8443/api/";

        //public const string ApiUrl = "https://131.3.47.13:8443/api/";
        //public const string ApiUrloffrs = "https://131.3.47.13:8443/api/";



        //public async Task<DTOLoginAPIResponse> Getauthentication(DTOAPILoginRequest Data)
        //{
        //    try
        //    {

        //        DTOLoginAPIResponse dynamicResponseDTO = new DTOLoginAPIResponse();
        //        HttpClient httpClient = new HttpClient(new NativeMessageHandler() { UseDefaultCredentials = true });

        //        var data1 = new[]
        //        {
        //            new KeyValuePair<string, string>("ClientKey", ""),
        //            new KeyValuePair<string, string>("ClientIP", "123"),
        //            new KeyValuePair<string, string>("ClientURL", ""),
        //            new KeyValuePair<string, string>("ClientPW", ""),
        //            new KeyValuePair<string, string>("ClientName", "miso"),
        //        };
        //        //HttpResponseMessage result = null;
        //        HttpResponseMessage result = await httpClient.PostAsync(ApiUrl + "validate", new FormUrlEncodedContent(data1));



        //        if (result != null)
        //        {

        //            // dynamicResponseDTO = result.Content.ReadAsAsync<DTOLoginResponse>().Result;

        //            string responseBody = await result.Content.ReadAsStringAsync();
        //            dynamicResponseDTO = JsonSerializer.Deserialize<DTOLoginAPIResponseData>(responseBody).ValidateRequest;

        //        }

        //        return dynamicResponseDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public async Task<DTOLoginAPIResponse> Getauthentication(DTOAPILoginRequest Data)
        {
            DTOLoginAPIResponse dynamicResponseDTO = new DTOLoginAPIResponse();
            using (var client = new HttpClient())
            {
                var requestBody = new StringContent($"accesskey={Data.accessKey}");
                requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Accept.Clear();
                var response = await client.PostAsync(Data.LoginUrl, requestBody);
                if (response.IsSuccessStatusCode)
                {
                    dynamicResponseDTO.token = await response.Content.ReadAsStringAsync();
                    dynamicResponseDTO.Status = true;
                    dynamicResponseDTO.Message = "ok";
                }
                else 
                {
                    dynamicResponseDTO.Status = false;
                    dynamicResponseDTO.Message = "IAAP Portal is not reachable! Please contact AHCC.";
                }
            }
            return dynamicResponseDTO;
        }
        public async Task<DTOApiPersDataResponse> GetData(DTOPersDataRequest Data)
        {
            DTOApiPersDataResponse dynamicResponseDTO = new DTOApiPersDataResponse();
            try
            {
                DTOAPIDataRequest dataRequest = new DTOAPIDataRequest();
                dataRequest.ArmyNo = Data.Pers_Army_No;
                //dataRequest.ApplyForId = Data.ApplyForId;

                using (var client = new HttpClient())
                {
                    // Query parameters
                    var queryParams = new
                    {
                        token = Data.jwt

                    };
                    var query = new FormUrlEncodedContent(queryParams.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.GetValue(queryParams)?.ToString()));


                    // JSON body
                    var body = new
                    {
                        pers_Army_No = Data.Pers_Army_No,
                        pubKey = Data.PubKey,

                    };
                    var uri = $"{Data.ApiUrl}?{await query.ReadAsStringAsync()}";


                    // Construct the query parameters
                    var jsonBody = System.Text.Json.JsonSerializer.Serialize(body);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    // Make the POST request
                    var response = await client.PostAsync(uri, content);

                    // Ensure the response is successful
                    //response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResult = await response.Content.ReadAsStringAsync();
                        DTOApiPersDataResponse? people = JsonConvert.DeserializeObject<DTOApiPersDataResponse>(jsonResult);
                        if (people != null) 
                        {
                            dynamicResponseDTO = people;
                            dynamicResponseDTO.Status = true;
                            dynamicResponseDTO.Message = "Ok";
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        dynamicResponseDTO.Status =false;
                        dynamicResponseDTO.Message = "Army No not found! Try with correct No (Error Code - 400).";
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        dynamicResponseDTO.Status = false;
                        dynamicResponseDTO.Message = "API Authentication failed! Please contact Admin.";
                    }
                    else
                    {
                        dynamicResponseDTO.Status = false;
                        dynamicResponseDTO.Message = "OASIS/INDRA Server not reachable! Please contact MP8/MP6.";
                    }
                }
                return dynamicResponseDTO;
            }
            catch (Exception ex)
            {
                dynamicResponseDTO.Status = false;
                dynamicResponseDTO.Message = ex.Message;
                return dynamicResponseDTO;
            }
        }

    }
}
