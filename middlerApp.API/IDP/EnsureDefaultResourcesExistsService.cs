using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace middlerApp.API.IDP
{
    public class EnsureDefaultResourcesExistsService : IHostedService
    {

        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IServiceProvider _provider;

        public EnsureDefaultResourcesExistsService(
            ILogger<EnsureDefaultResourcesExistsService> logger,
            IHostApplicationLifetime appLifetime,
            IServiceProvider provider)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _provider = provider;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        private void OnStarted()
        {

            using var scope = _provider.CreateScope();
            var rManager = scope.ServiceProvider.GetRequiredService<DefaultResourcesManager>();

            rManager.EnsureAllResourcesExists();

        }

    }
}
