using Microsoft.Extensions.Caching.Memory;

namespace Web.Services
{
    public interface ICdnHealthService
    {
        Task<string> GetStaticBaseUrlAsync();
    }

    public class CdnHealthService : ICdnHealthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        public CdnHealthService(
           IConfiguration configuration,
           IHttpClientFactory httpClientFactory,
           IMemoryCache cache)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }
        public async Task<string> GetStaticBaseUrlAsync()
        {
            return await _cache.GetOrCreateAsync("CDN_BASE_URL", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                string cdnUrl =
                    Environment.GetEnvironmentVariable("AFSAC__CDN")?.TrimEnd('/')
                    ?? _configuration["AFSAC:CDN"]?.TrimEnd('/')
                    ?? "";

                string localUrl =
                    _configuration["AFSAC__Local"]?.TrimEnd('/')
                    ?? _configuration["AFSAC:Local"]?.TrimEnd('/')
                    ?? "";

                bool isCdnRunning = await IsCdnRunningAsync(cdnUrl);

                return isCdnRunning ? cdnUrl : localUrl;
            }) ?? "";
        }

        private async Task<bool> IsCdnRunningAsync(string cdnUrl)
        {
            if (string.IsNullOrWhiteSpace(cdnUrl))
                return false;

            try
            {
                var client = _httpClientFactory.CreateClient("CdnHealthClient");

                string healthUrl = cdnUrl.TrimEnd('/') + "/health.txt";

                using var response = await client.GetAsync(
                    healthUrl,
                    HttpCompletionOption.ResponseHeadersRead
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
