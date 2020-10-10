using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers.DotNetCore.WebApi
{
    public class WebApiApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _configureServiceCollectionAction;
        private readonly Action<WebHostBuilderContext, IConfigurationBuilder> _configureAppConfigurationAction;

        public WebApiApplicationFactory(
            Action<IServiceCollection> configureServiceCollectionAction = null,
            Action<WebHostBuilderContext, IConfigurationBuilder> configureAppConfigurationAction = null)
        {
            _configureServiceCollectionAction = configureServiceCollectionAction;
            _configureAppConfigurationAction = configureAppConfigurationAction;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                _configureServiceCollectionAction?.Invoke(services);
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                _configureAppConfigurationAction?.Invoke(context, config);
            });
        }
    }
}