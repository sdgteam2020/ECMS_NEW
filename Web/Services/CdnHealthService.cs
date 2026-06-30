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
            string cdnUrl =
                    Environment.GetEnvironmentVariable("AFSAC__CDN")?.TrimEnd('/')
                    ?? _configuration["AFSAC:CDN"]?.TrimEnd('/')
                    ?? "";

            string localUrl =
                _configuration["AFSAC__Local"]?.TrimEnd('/')
                ?? _configuration["AFSAC:Local"]?.TrimEnd('/')
                ?? "";
            return await _cache.GetOrCreateAsync("CDN_BASE_URL", async entry =>
            {
                bool isCdnRunning = await IsCdnRunningAsync(cdnUrl);

                // If CDN is running, cache only for few seconds
                // so failover happens quickly
                entry.AbsoluteExpirationRelativeToNow = isCdnRunning
                    ? TimeSpan.FromSeconds(5)
                    : TimeSpan.FromSeconds(10);

                return isCdnRunning ? cdnUrl : localUrl;
            }) ?? localUrl;
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
