using BusinessLogicsLayer.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Helpers
{
    public static class ApiCall
    {
        public const string ApiUrl = "http://192.168.10.203/api/";

        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiUrl)
        };

        public static async Task<HttpResponseMessage> PostAPI<T>(string url, T data)
        {
            try
            {
                // ApiUrl already set as BaseAddress
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, data);
                return response;
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
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                return response;
            }
            catch (Exception ex)
            {
                _ = ex;
                return null;
            }
        }
    }
}
