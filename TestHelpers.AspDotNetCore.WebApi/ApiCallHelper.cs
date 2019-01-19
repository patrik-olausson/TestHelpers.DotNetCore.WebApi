using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestHelpers.DotNetCore.WebApi
{
    public class ApiCallHelper : IDisposable
    {
        public readonly HttpClient HttpClient;
        private readonly Action<string> _writeToTestOutput;

        public ApiCallHelper(
            HttpClient httpClient,
            Action<string> writeToTestOutput,
            IReadOnlyCollection<Tuple<string, string>> defaultHeaders = null)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _writeToTestOutput = writeToTestOutput;

            if (defaultHeaders != null)
            {
                foreach (var tuple in defaultHeaders)
                {
                    HttpClient.DefaultRequestHeaders.Add(tuple.Item1, tuple.Item2);
                }
            }
        }

        public virtual async Task<AssertableHttpResponse> GetAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            preRequestConfigureHttpClientAction?.Invoke(HttpClient);

            var response = await HttpClient.GetAsync(requestUri);

            return await CreateAssertableResponseAsync(response, $"GET {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<HttpResponseMessage> GetRawAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            preRequestConfigureHttpClientAction?.Invoke(HttpClient);

            var response = await HttpClient.GetAsync(requestUri);

            OutputToTestLog($"GET {requestUri}{Environment.NewLine}Response.StatusCode:{Environment.NewLine}{response.StatusCode}");

            if (ensureSuccessStatusCode)
                response.EnsureSuccessStatusCode();

            return response;
        }

        public virtual async Task<AssertableHttpResponse> PostAsJsonAsync<T>(
            string requestUri,
            T value,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            preRequestConfigureHttpClientAction?.Invoke(HttpClient);

            var response = await HttpClient.PostAsJsonAsync(requestUri, value);

            return await CreateAssertableResponseAsync(response, $"POST {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> PostFileAsync(
            string requestUri,
            string pathToFile,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            preRequestConfigureHttpClientAction?.Invoke(HttpClient);

            HttpResponseMessage response;
            using (var form = new MultipartFormDataContent())
            using (var streamContent = new ByteArrayContent(File.ReadAllBytes(pathToFile)))
            {
                form.Add(streamContent, "files", Path.GetFileName(pathToFile));
                response = await HttpClient.PostAsync(requestUri, form);
            }

            return await CreateAssertableResponseAsync(response, $"POST file {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> PutAsJsonAsync<T>(
            string requestUri,
            T value,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            preRequestConfigureHttpClientAction?.Invoke(HttpClient);

            var response = await HttpClient.PutAsJsonAsync(requestUri, value);

            return await CreateAssertableResponseAsync(response, $"PUT {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> DeleteAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            preRequestConfigureHttpClientAction?.Invoke(HttpClient);

            var response = await HttpClient.DeleteAsync(requestUri);

            return await CreateAssertableResponseAsync(response, $"DELETE {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> OptionsAsync<T>(
            string requestUri,
            T value,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            preRequestConfigureHttpClientAction?.Invoke(HttpClient);
            var request = new HttpRequestMessage(HttpMethod.Options, requestUri);

            var response = await HttpClient.SendAsync(request);
            return await CreateAssertableResponseAsync(response, $"OPTIONS {requestUri}", ensureSuccessStatusCode);
        }

        protected virtual Task<AssertableHttpResponse> CreateAssertableResponseAsync(
            HttpResponseMessage response,
            string testOutput,
            bool ensureSuccessStatusCode)
        {
            return CreateAssertableResponseAsync(
                response,
                testOutput,
                ensureSuccessStatusCode,
                OutputToTestLog);
        }
        
        public static async Task<AssertableHttpResponse> CreateAssertableResponseAsync(
            HttpResponseMessage response,
            string testOutput,
            bool ensureSuccessStatusCode,
            Action<string> testOutputWriter)
        {
            var assertableResponse = new AssertableHttpResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync(),
                response.Headers);

             testOutputWriter?.Invoke($"{testOutput}{Environment.NewLine}Response:{Environment.NewLine}{assertableResponse}");

            if (ensureSuccessStatusCode)
                assertableResponse.EnsureSuccessStatusCode();

            return assertableResponse;
        }

        public virtual void OutputToTestLog(string message)
        {
            _writeToTestOutput?.Invoke($"{Environment.NewLine}{message}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HttpClient.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}