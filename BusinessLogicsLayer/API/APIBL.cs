using DataTransferObject.Requests;
using DataTransferObject.Response;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace BusinessLogicsLayer.API
{
    public class Apibl : IaPiBl
    {
       
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
