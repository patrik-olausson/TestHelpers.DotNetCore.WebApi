using System;

namespace TestHelpers.DotNetCore.WebApi
{
    public class WebApiTestHelper<TStartup> : IDisposable where TStartup : class
    {
        public IApiCallHelper ApiCall { get; }

        public WebApiTestHelper(WebApiTestHelperConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
            var applicationFactory = new WebApiApplicationFactory<TStartup>(
                configuration.ConfigureServiceCollection,
                configuration.ConfigureAppConfiguration);
            var httpClient = applicationFactory.CreateClient();

            ApiCall = configuration.CreateApiCallHelper(httpClient);
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