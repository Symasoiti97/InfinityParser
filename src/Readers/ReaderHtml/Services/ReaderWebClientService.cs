using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ReaderHtml.Services
{
    public class ReaderWebClientService : IReaderHtmlService
    {
        private readonly ILogger<ReaderWebClientService> _logger;
        private readonly HttpClient _httpClient;

        public ReaderWebClientService(
            ILogger<ReaderWebClientService> logger,
            HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GetAsync(string address)
        {
            var httpResponseMessage = await _httpClient.GetAsync(address);
            var source = string.Empty;

            if (httpResponseMessage != null && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                source = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            else
            {
                _logger.LogWarning("httpResponseMessage == null/page loaded not successfully\tAddress: {0}", address);
            }

            return source;
        }
    }
}