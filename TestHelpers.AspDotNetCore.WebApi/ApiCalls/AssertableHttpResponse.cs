using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace TestHelpers.DotNetCore.WebApi
{
    public class AssertableHttpResponse
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, };

        public string Body { get; }

        public string BodyAsJsonFormattedString
        {
            get
            {
                if (LooksLikeItContainsJson(Body))
                {
                    var element = JsonSerializer.Deserialize<JsonElement>(Body);
                    return JsonSerializer.Serialize(element, jsonSerializerOptions);
                }

                return "{}";
            }
        }
        public HttpStatusCode StatusCode { get; }
        public HttpResponseHeaders Headers { get; }

        public AssertableHttpResponse(
            HttpStatusCode statusCode,
            string body,
            HttpResponseHeaders responseHeaders)
        {
            StatusCode = statusCode;
            Headers = responseHeaders;
            Body = body;
        }

        /// <summary>
        /// Throws an exception if the status code isn't within the 2xx range
        /// </summary>
        public void EnsureSuccessStatusCode()
        {
            var numericStatusCode = (int)StatusCode;
            if (numericStatusCode >= 200 && numericStatusCode < 300)
                return;

            throw new Exception($"Status code {StatusCode} is not considered as success.{Environment.NewLine}Body: {Body}");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"StatusCode: {StatusCode}");
            sb.AppendLine($"Headers: {JsonSerializer.Serialize(Headers, jsonSerializerOptions)}");

            var body = LooksLikeItContainsJson(Body) ? BodyAsJsonFormattedString : Body;
            sb.AppendLine($"Body: {body}");

            return sb.ToString();
        }

        private static bool LooksLikeItContainsJson(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            if (value.StartsWith("{")) return true;
            if (value.StartsWith("[")) return true;

            return false;
        }
    }
}