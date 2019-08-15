using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestHelpers.DotNetCore.WebApi
{
    public class GraphQLCallHelper
    {
        private readonly string _url;
        private readonly ApiCallHelper _apiCall;
        private readonly IReadOnlyCollection<Tuple<string, string>> _defaultHeaders;

        public GraphQLCallHelper(
            string url, 
            ApiCallHelper apiCall,
            IReadOnlyCollection<Tuple<string, string>> defaultHeaders = null)
        {
            _url = url;
            _apiCall = apiCall ?? throw new ArgumentNullException(nameof(apiCall));
            _defaultHeaders = defaultHeaders;
        }

        /// <summary>
        /// Helper method that makes a call to the specified GraphQL endpoint.
        /// </summary>
        /// <param name="query">The actual GraphQL query (without any JSON notation!). The JSON
        /// structure is added automatically before making the call to the server.</param>
        /// <returns>A response object that contains the returned JSON (and the assertable
        /// http response)</returns>
        public async Task<GraphQLResponse> QueryAsync(string query)
        {
            var response = await PostAsync(_url, $"{{\"query\":\"{query}\"}}");
            
            return new GraphQLResponse(response);
        }
        
        private async Task<AssertableHttpResponse> PostAsync(
            string requestUri,
            string value)
        {
            var stringContent = HttpClientExtensions.CreateHttpStringContent(value);
            if (_defaultHeaders != null)
            {
                foreach (var defaultHeader in _defaultHeaders)
                {
                    stringContent.Headers.Add(defaultHeader.Item1, defaultHeader.Item2);
                }
            }

            var response = await _apiCall.HttpClient.PostAsync(requestUri, stringContent);

            return await ApiCallHelper.CreateAssertableResponseAsync(
                response, 
                $"POST {requestUri}", 
                true,
                _apiCall.OutputToTestLog);
        }
    }
}