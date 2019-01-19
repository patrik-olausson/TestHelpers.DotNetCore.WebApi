using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers.DotNetCore.WebApi
{
    public class WebApiTestHelper<TStartup> : IDisposable where TStartup : class
    {
        private readonly Action<IServiceCollection> _configureServiceCollection;
        public ApiCallHelper ApiCall { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logToTestOutput">Action that logs information messages.</param>
        /// <param name="configureServiceCollection">Action that makes it possible to configure the service collection
        /// that will be used during the test run. Normally you would only use this when there is a specific change
        /// that needs to be done in a specific test case. When there are setup that is common for all (or most)
        /// tests you should override the ConfigureServicesCollectionWithDefaultTestBehaviour instead.</param>
        /// <param name="defaultHeaders">Headers that should be used for all API calls.</param>
        public WebApiTestHelper(
            Action<string> logToTestOutput = null,
            Action<IServiceCollection> configureServiceCollection = null,
            IReadOnlyCollection<Tuple<string, string>> defaultHeaders = null)
        {
            _configureServiceCollection = configureServiceCollection;
            var applicationFactory = new WebApiApplicationFactory<TStartup>(ConfigureServiceCollection);
            var httpClient = applicationFactory.CreateClient();
            
            ApiCall = new ApiCallHelper(
                httpClient, 
                logToTestOutput, 
                defaultHeaders);
        }
        
        /// <summary>
        /// Default initialization of the services collection.
        /// First allow an inheriting class to provide default replacements and registrations.
        /// Then invoke the test case specific override (if there is any).
        /// </summary>
        private void ConfigureServiceCollection(IServiceCollection services)
        {
            ConfigureServicesCollectionWithDefaultTestBehaviour(services);
            _configureServiceCollection?.Invoke(services);
        }

        /// <summary>
        /// Method to be used by inheriting class to provide relevant registrations to the
        /// service collection that will be used during the test run. 
        /// </summary>
        protected virtual void ConfigureServicesCollectionWithDefaultTestBehaviour(IServiceCollection services)
        {
            
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