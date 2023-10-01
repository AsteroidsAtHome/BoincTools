using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using serverStatusNotifier.Models;
using serverStatusNotifier.Services;

namespace serverStatusNotifier
{
	internal class Program
	{
		private static string LogPath = "Log/";
		private readonly ILogger _logger = Log.ForContext<Program>();
		public static IConfiguration Configuration { get; set; }

		static void Main(string[] args)
		{
			Configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				//.AddCommandLine(args)
				.Build();

			IHost builder = (IHost)Host.CreateDefaultBuilder(args)
				.ConfigureServices(services =>
				{
					services.AddSingleton<IConfiguration>(provider => Configuration);
					services.AddSingleton<IConfiguration>(Configuration);
					services.Configure<NotifyEmails>(options => Configuration.GetSection("NotifyEmails").Bind(options));
					services.AddSingleton<ILogger>(Log.Logger);
					services.AddHostedService<NotificationService>();
					services.AddScoped<IEmailService, EmailService>();
					services.AddScoped<IRunnerService, RunnerService>();
				})
				.Build();

			Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Information()
					.Enrich.FromLogContext()
					.WriteTo.File(LogPath + "LogServerStatus_.log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1048576)
					.WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:l}{NewLine}{Exception}")
					.CreateLogger();


			//var services = ConfigureServices();
			//builder.Configuration
			//		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			//		.AddEnvironmentVariables();
			//builder.Services.AddSingleton<IConfiguration>(Configuration);
			//builder.Services.Configure<NotifyEmails>(options => Configuration.GetSection("NotifyEmails").Bind(options));
			//builder.Services.AddSingleton<ILogger>(Log.Logger);
			//builder.Services.AddSingleton<IConfiguration>(provider => Configuration);
			//builder.Services.AddHostedService<NotificationService>();
			//builder.Services.BuildServiceProvider();

			//var runner = new Runner(config);

			Log.Logger.Information("Starting...");
			//runner.CheckServerStatus(serverStatus);

			//var app = builder.Build();
			builder.Run();

			Log.Logger.Information("Exiting...");
		}

		//private static IServiceCollection ConfigureServices()
		//{
		//	IServiceCollection services = new ServiceCollection();






		//	services.Configure<NotifyEmails>(options => Configuration.GetSection("NotifyEmails").Bind(options))
		//		.BuildServiceProvider();

		//	services.AddSingleton<ILogger>(Log.Logger);
		//	services.AddSingleton<IConfiguration>(provider => Configuration);
		//	services.AddHostedService<NotificationService>();

		//	return services;
		//}


	}
}