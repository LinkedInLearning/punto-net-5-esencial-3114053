using System;
using System.Threading;
using System.Threading.Tasks;
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

            await hostBuilder.RunConsoleAsync();
        }
    }

    interface ITest {
        void Run();
    }

    class Test : ITest
    {
        public void Run()
        {
            System.Console.WriteLine("Hola LinkedIn Learning");
        }
    }

    class MyService : IHostedService
    {
        private readonly ITest test;

        public MyService(ITest test)
        {
            this.test = test;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            test.Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
