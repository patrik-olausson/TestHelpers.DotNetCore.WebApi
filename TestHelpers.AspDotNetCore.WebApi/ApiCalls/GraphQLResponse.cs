using System;

namespace TestHelpers.DotNetCore.WebApi
{
    public class GraphQLResponse 
    {
        private readonly AssertableHttpResponse _assertableHttpResponse;
        
        public string JsonString => _assertableHttpResponse.BodyAsJsonFormattedString;
        public AssertableHttpResponse Response => _assertableHttpResponse;
        
        public GraphQLResponse(AssertableHttpResponse assertableHttpResponse)
        {
            _assertableHttpResponse = assertableHttpResponse ?? throw new ArgumentNullException(nameof(assertableHttpResponse));
        }
    }
}