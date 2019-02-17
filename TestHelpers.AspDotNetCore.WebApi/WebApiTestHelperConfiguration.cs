using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers.DotNetCore.WebApi
{
    public abstract class WebApiTestHelperConfiguration
    {
        private readonly Action<string> _logToTestOutput;
        public IReadOnlyCollection<Tuple<string, string>> DefaultHeaders { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logToTestOutput">Action that logs information messages to the test output window.</param>
        /// <param name="defaultHeaders">Headers that should be used for all API calls.</param>
        protected WebApiTestHelperConfiguration(
            Action<string> logToTestOutput,
            IReadOnlyCollection<Tuple<string, string>> defaultHeaders = null)
        {
            _logToTestOutput = logToTestOutput;
            DefaultHeaders = defaultHeaders;
        }
        
        public virtual void LogToTestOutput(string message)
        {
            _logToTestOutput?.Invoke(message);
        }
        
        /// <summary>
        /// Method that is invoked after the service collection has been initialized by the Startup class
        /// and makes it possible to override services that will be used during the test run.
        /// </summary>
        /// <param name="services"></param>
        public abstract void ConfigureServiceCollection(IServiceCollection services);
    }
}