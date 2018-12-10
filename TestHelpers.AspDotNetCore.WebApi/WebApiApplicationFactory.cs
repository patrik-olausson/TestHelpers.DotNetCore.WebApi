using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers.DotNetCore.WebApi
{
    public class WebApiApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _configureServiceCollectionAction;

        public WebApiApplicationFactory(Action<IServiceCollection> configureServiceCollectionAction = null)
        {
            _configureServiceCollectionAction = configureServiceCollectionAction;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                _configureServiceCollectionAction?.Invoke(services);
            });
        }
    }
}