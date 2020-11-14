using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestHelpers.DotNetCore.WebApi
{
    public interface IApiCallHelper
    {
        Task<AssertableHttpResponse> GetAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null);

        Task<HttpResponseMessage> GetRawAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null);

        Task<AssertableHttpResponse> PostAsJsonAsync<T>(
            string requestUri,
            T value,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null);

        Task<AssertableHttpResponse> PostFileAsync(
            string requestUri,
            string pathToFile,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null);

        Task<AssertableHttpResponse> PutAsJsonAsync<T>(
            string requestUri,
            T value,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null);

        Task<AssertableHttpResponse> DeleteAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null);

        Task<AssertableHttpResponse> OptionsAsync<T>(
            string requestUri,
            T value,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null);

        void OutputToTestLog(string message);
        void Dispose();
    }
}