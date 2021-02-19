using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace testhost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder();

            hostBuilder.ConfigureServices((context, services) => {
                services.AddSingleton<ITest, Test>();
                services.AddSingleton<IHostedService, MyService>();
            });

            hostBuilder.ConfigureAppConfiguration((context, configuration) => {
               configuration.AddCommandLine(args);
               configuration.AddEnvironmentVariables();
            });

            await hostBuilder.RunConsoleAsync();
        }
    }

    interface ITest {
        void Run(string message);
    }

    class Test : ITest
    {
        public void Run(string message)
        {
            System.Console.WriteLine(message);
        }
    }

    class MyService : IHostedService
    {
        private readonly ITest test;
        private readonly IConfiguration configuration;

        public MyService(ITest test, IConfiguration configuration)
        {
            this.test = test;
            this.configuration = configuration;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var message = configuration.GetValue<string>("PROCESSOR_IDENTIFIER");
            test.Run(message);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
