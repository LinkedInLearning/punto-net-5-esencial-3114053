using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Serilog;


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File("test.log")
                .CreateLogger();

        HostBuilder hostBuilder = new();

        hostBuilder.ConfigureServices((context, services) => {
            services.AddSingleton<ITest, Test>();
            services.AddSingleton<IHostedService, MyService>();
            services.AddHttpClient<ITest, Test>(client => {
                client.BaseAddress = new Uri("https://swapi.dev");
            }).AddPolicyHandler(GetRetryPolicy());
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

interface ITest {
    void Run(string message);
    Task InvokeEndpointAsync(string endpoint);
}

class Test : ITest
{
    private readonly HttpClient client;

    public Test(HttpClient client)
    {
        this.client = client;
    }
    public async Task InvokeEndpointAsync(string endpoint)
    {
        var result = await client.GetAsync(endpoint);
        System.Console.WriteLine(await result.Content.ReadAsStringAsync());
    }

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

    public string ConfigurationKey {get; init;} = "PROCESSOR_IDENTIFIER";

    public MyService(ITest test, 
        IConfiguration configuration,
        ILogger<MyService> logger)
    {
        this.test = test;
        this.configuration = configuration;
        this.logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var message = configuration.GetValue<string>(ConfigurationKey);
        logger?.LogInformation($"Sending {message}");
        await test.InvokeEndpointAsync("api/people/1/");
        test.Run(message);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

