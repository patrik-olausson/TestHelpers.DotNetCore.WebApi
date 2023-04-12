using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestHelpers.DotNetCore.WebApi
{
    public class NewApiCallHelper : IApiCallHelper
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        private readonly HttpClient _httpClient;
        private readonly Action<string> _writeToTestOutput;

        public NewApiCallHelper(
            HttpClient httpClient,
            Action<string> writeToTestOutput,
            IReadOnlyCollection<Header> defaultHeaders = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _writeToTestOutput = writeToTestOutput;

            if (defaultHeaders != null)
            {
                foreach (var header in defaultHeaders)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Name, header.Value);
                }
            }
        }

        public Task<AssertableHttpResponse> GetAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            return SendAsync(CreateRequestMessage(HttpMethod.Get, requestUri), preRequestConfigureHttpClientAction, ensureSuccessStatusCode);
        }

        public Task<HttpResponseMessage> GetRawAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            preRequestConfigureHttpClientAction?.Invoke(_httpClient);

            return SendRawAsync(CreateRequestMessage(HttpMethod.Get, requestUri), ensureSuccessStatusCode);
        }

        public Task<AssertableHttpResponse> PostAsJsonAsync<T>(
            string requestUri,
            T value,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            return SendAsync(
                CreateRequestMessage(
                    HttpMethod.Post,
                    requestUri,
                    CreateJsonContent(value)),
                preRequestConfigureHttpClientAction,
                ensureSuccessStatusCode);
        }

        public Task<AssertableHttpResponse> PostFileAsync(
            string requestUri,
            string pathToFile,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            using (var form = new MultipartFormDataContent())
            using (var streamContent = new ByteArrayContent(File.ReadAllBytes(pathToFile)))
            {
                form.Add(streamContent, "files", Path.GetFileName(pathToFile));
                return SendAsync(
                    CreateRequestMessage(
                        HttpMethod.Post,
                        requestUri,
                        form),
                    preRequestConfigureHttpClientAction,
                    ensureSuccessStatusCode);
            }
        }

        public Task<AssertableHttpResponse> PutAsJsonAsync<T>(
            string requestUri,
            T value,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            return SendAsync(
                CreateRequestMessage(
                    HttpMethod.Put,
                    requestUri,
                    CreateJsonContent(value)),
                preRequestConfigureHttpClientAction,
                ensureSuccessStatusCode);
        }

        public Task<AssertableHttpResponse> DeleteAsync(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            return SendAsync(CreateRequestMessage(HttpMethod.Delete, requestUri), preRequestConfigureHttpClientAction, ensureSuccessStatusCode);
        }

        public Task<AssertableHttpResponse> OptionsAsync<T>(
            string requestUri,
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            return SendAsync(CreateRequestMessage(HttpMethod.Options, requestUri), preRequestConfigureHttpClientAction, ensureSuccessStatusCode);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<AssertableHttpResponse> SendAsync(
            HttpRequestMessage request,
            Action<HttpClient> preRequestConfigureHttpClientAction,
            bool ensureSuccessStatusCode = true)
        {
            preRequestConfigureHttpClientAction?.Invoke(_httpClient);

            var response = await _httpClient.SendAsync(request);

            var assertableResponse = new AssertableHttpResponse(
                response.StatusCode,
                await TryGetContentAsString(response.Content),
                response.Headers);

            if(_writeToTestOutput != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Request");
                sb.AppendLine($"{request.Method} {request.RequestUri}");
                sb.AppendLine($"Headers: {SerializeToJson(request.Headers)}");
                sb.AppendLine(assertableResponse.ToString());
                _writeToTestOutput(sb.ToString());
            }
            
            if (ensureSuccessStatusCode)
                assertableResponse.EnsureSuccessStatusCode();

            return assertableResponse;
        }

        private async Task<HttpResponseMessage> SendRawAsync(
            HttpRequestMessage request,
            bool ensureSuccessStatusCode)
        {
            var response = await _httpClient.SendAsync(request);

            if(_writeToTestOutput != null)
            {
                var body = await TryGetContentAsString(response.Content);
                var sb = new StringBuilder();
                sb.AppendLine("Request");
                sb.AppendLine($"{request.Method} {request.RequestUri}");
                sb.AppendLine($"Headers: {SerializeToJson(request.Headers)}");
                sb.AppendLine("Response");
                sb.AppendLine($"StatusCode: {response.StatusCode}");
                sb.AppendLine($"Headers: {SerializeToJson(response.Headers)}");
                sb.AppendLine($"Body: {body}");

                _writeToTestOutput(sb.ToString());
            }
            
            if (ensureSuccessStatusCode)
                response.EnsureSuccessStatusCode();

            return response;
        }

        private string SerializeToJson(object value)
        {
            return JsonSerializer.Serialize(value, jsonSerializerOptions);
        }

        private HttpRequestMessage CreateRequestMessage(
            HttpMethod method,
            string url,
            HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (content != null)
            {
                request.Content = content;
            }

            return request;
        }

        private HttpContent CreateJsonContent(object body)
        {
            return new JsonContent(SerializeToJson(body));
        }

        private async Task<string> TryGetContentAsString(HttpContent content)
        {
            if (content == null)
                return "No content (null)";

            var contentString = await content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(contentString))
                return "No content (empty)";

            return contentString;
        }
    }

    public class JsonContent : StringContent
    {
        public JsonContent(string content) : base(
            content,
            Encoding.UTF8,
            "application/json")
        {
        }
    }
}