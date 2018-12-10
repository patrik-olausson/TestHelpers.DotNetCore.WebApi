using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers.DotNetCore.WebApi
{
    public class WebApiTestHelper<TStartup> : IDisposable where TStartup : class
    {
        public ApiCallHelper ApiCall { get; }

        public WebApiTestHelper(
            Action<string> logToTestOutput = null,
            Action<IServiceCollection> configureServiceCollection = null,
            IReadOnlyCollection<Tuple<string, string>> defaultHeaders = null)
        {
            var applicationFactory = new WebApiApplicationFactory<TStartup>(configureServiceCollection);
            var httpClient = applicationFactory.CreateClient();
            ApiCall = new ApiCallHelper(
                httpClient, 
                logToTestOutput, 
                defaultHeaders);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ApiCall.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}