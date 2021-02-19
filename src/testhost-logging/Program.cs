using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace testhost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.File("test.log")
                    .CreateLogger();

            var hostBuilder = new HostBuilder();

            hostBuilder.ConfigureServices((context, services) => {
                services.AddSingleton<ITest, Test>();
                services.AddSingleton<IHostedService, MyService>();
            });

            hostBuilder.ConfigureAppConfiguration((context, configuration) => {
               configuration.AddCommandLine(args);
               configuration.AddEnvironmentVariables();
            });

            hostBuilder.ConfigureLogging((context, logging) => {
                logging.AddConsole();
                logging.AddSerilog();
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
        private readonly ILogger<MyService> logger;

        public MyService(ITest test, IConfiguration configuration, ILogger<MyService> logger)
        {
            this.test = test;
            this.configuration = configuration;
            this.logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var message = configuration.GetValue<string>("PROCESSOR_IDENTIFIER");
            logger?.LogInformation($"Sending {message}");
            test.Run(message);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
