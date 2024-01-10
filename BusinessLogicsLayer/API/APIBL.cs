using BusinessLogicsLayer.Helpers;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using ModernHttpClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
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
        //public const string ApiUrl = "http://192.168.10.200/api/";
        public const string ApiUrl = "http://192.168.10.203:7002/api/";

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
            try
            {
                DTOLoginAPIResponse dynamicResponseDTO = new DTOLoginAPIResponse();

                HttpResponseMessage result = null;
                await HRMSPostAPI("Login", Data, "").ContinueWith(task =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)

                    {
                        result = task.Result;
                    }
                });



                if (result != null)
                {
                    using (var contentStream = await result.Content.ReadAsStreamAsync())
                    {
                        dynamicResponseDTO = await JsonSerializer.DeserializeAsync<DTOLoginAPIResponse>(contentStream);
                        //dynamicResponseDTO = await JsonSerializer.DeserializeAsync<IEnumerable<DTOLoginResponse>>(contentStream);
                    }
                    // dynamicResponseDTO = result.Content.ReadAsAsync<DTOLoginResponse>().Result;
                    // dynamicResponseDTO = result.Content.ReadFromJsonAsync<DTOLoginResponse>().Result;
                }


                // dTOLoginResponse.armyNo = jwtSecurityToken.Actor;
                // dTOLoginResponse

                return dynamicResponseDTO;



            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //public async Task<DTOApiPersDataResponse> GetData(DTOPersDataRequest Data)
        //{
        //    try
        //    {

        //        DTOApiPersDataResponse resdata = new DTOApiPersDataResponse();



        //        HttpResponseMessage result = null;
        //        //await ApiCall.PostAPI("persdata", Data).ContinueWith(task =>
        //        //{
        //        //    if (task.Status == TaskStatus.RanToCompletion)
        //        //    {
        //        //        result = task.Result;
        //        //    }
        //        //});
        //        HttpClient httpClient = new HttpClient(new NativeMessageHandler());
        //        var dt = DateTime.Parse(DateTime.Now.ToString());
        //        var dtStr = dt.ToString("dd-MMM-yyyy HH:mm:ss");
        //        var data1 = new[]
        //        {
        //            new KeyValuePair<string, string>("Pers_Army_No", Data.Pers_Army_No),
        //            new KeyValuePair<string, string>("jwt", Data.jwt),
        //            new KeyValuePair<string, string>("timestamp", dtStr),
        //            new KeyValuePair<string, string>("ClientName", "miso"),
        //        };
        //        //HttpResponseMessage result = null;
        //        result = await httpClient.PostAsync(ApiUrl + "persdata", new FormUrlEncodedContent(data1));



        //        if (result != null)
        //        {

        //            // dynamicResponseDTO = result.Content.ReadAsAsync<DTOLoginResponse>().Result;

        //            string responseBody = await result.Content.ReadAsStringAsync();
        //            resdata = JsonSerializer.Deserialize<ApiPersDataResponseData>(responseBody).AFSAC;

        //        }

        //        return resdata;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public async Task<DTOApiPersDataResponse> GetData(DTOPersDataRequest Data)
        {
            try
            {
                DTOApiPersDataResponse dynamicResponseDTO = new DTOApiPersDataResponse();
                DTOAPIDataRequest dataRequest = new DTOAPIDataRequest();
                dataRequest.ArmyNo = Data.Pers_Army_No;
                HttpResponseMessage result = null;
                await HRMSPostAPI("Fetch/GetData", dataRequest, Data.jwt).ContinueWith(task =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)

                    {
                        result = task.Result;
                    }
                });



                if (result != null)
                {
                    using (var contentStream = await result.Content.ReadAsStreamAsync())
                    {
                        dynamicResponseDTO = await JsonSerializer.DeserializeAsync<DTOApiPersDataResponse>(contentStream);
                        //dynamicResponseDTO = await JsonSerializer.DeserializeAsync<IEnumerable<DTOLoginResponse>>(contentStream);
                    }
                    // dynamicResponseDTO = result.Content.ReadAsAsync<DTOLoginResponse>().Result;
                    // dynamicResponseDTO = result.Content.ReadFromJsonAsync<DTOLoginResponse>().Result;
                }


                // dTOLoginResponse.armyNo = jwtSecurityToken.Actor;
                // dTOLoginResponse

                return dynamicResponseDTO;



            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<HttpResponseMessage> HRMSPostAPI<T>(string url, T data, string jwtToken)
        {
            try
            {

                // HttpClient httpClient = new HttpClient(new NativeMessageHandler());
                // HttpResponseMessage s = await httpClient.PostAsJsonAsync(HRMSApiUrl + url, data);



                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    using (var httpClient = new HttpClient(httpClientHandler))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        if (data is null || data.ToString() == "")
                        {
                            return await httpClient.GetAsync(ApiUrl + url);
                        }
                        else
                        {
                            return await httpClient.PostAsJsonAsync(ApiUrl + url, data);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException.Message;
                _ = ex;
                return null;

            }
        }
    }
}
