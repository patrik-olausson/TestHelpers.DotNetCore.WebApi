﻿using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestHelpers.DotNetCore.WebApi
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T value)
        {
            return httpClient.PostAsync(requestUri, CreateJsonContent(value));
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T value)
        {
            return httpClient.PutAsync(requestUri, CreateJsonContent(value));
        }

        private static HttpContent CreateJsonContent<T>(T value, bool indented = true, Encoding encoding = null)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = indented,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

        var jsonString = JsonSerializer.Serialize(value, options);

            return CreateHttpStringContent(content:jsonString, encoding: encoding);
        }

        /// <summary>
        /// Factory method for creating a StringContent that defaults to using
        /// application/json and UTF8.
        /// </summary>
        /// <param name="content">The string value (payload) to include</param>
        /// <param name="mediaType">The media type, defaults to application/json</param>
        /// <param name="encoding">The encoding, defaults to UTF8</param>
        /// <returns></returns>
        public static StringContent CreateHttpStringContent(
            string content,
            string mediaType = "application/json",
            Encoding encoding = null)
        {
            return new StringContent(
                content,
                encoding ?? Encoding.UTF8,
                mediaType);
        }
    }
}