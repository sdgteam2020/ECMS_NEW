using BusinessLogicsLayer.API;
using ModernHttpClient;
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
    }
}
