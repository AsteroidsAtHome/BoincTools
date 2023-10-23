using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using serverStatusNotifier.Extensions;
using serverStatusNotifier.Models;

namespace serverStatusNotifier.Services
{
	public class NotificationService : IHostedService, INotificationService
	{
		private readonly IConfiguration _configuration;
		private readonly IRunnerService _runner;
		private readonly ILogger _logger = Log.ForContext<NotificationService>();
		private int _wakeupTimerPeriod;
		private int _httpClientTimeout = 100000;
		private string _serverURL = string.Empty;
		private Timer notificationTimer;

		public NotificationService(
			IConfiguration config,
			IRunnerService runner)
		{
			_configuration = config;
			_runner = runner;
			notificationTimer = new Timer(InvokeRunnerAsync, null, Timeout.Infinite, Timeout.Infinite);
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_wakeupTimerPeriod = _configuration.GetValue<int>("WakeupTimerIntervalInSeconds");
			_httpClientTimeout = _configuration.GetValue<int>("HttpClientTimeoutInMilliseconds");
			_serverURL = _configuration.GetValue<string>("ServerURL") ?? "localhost/boinc/server_status.php?xml=1";
			_logger.Information($"Server URL: {_serverURL}]");
			await Task.Run(() => notificationTimer.Change(0, _wakeupTimerPeriod * 1000));
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await Task.Run(() => notificationTimer.Change(Timeout.Infinite, Timeout.Infinite));
		}

		private void InvokeRunnerAsync(object? stateInfo)
		{
			_logger.Information($"Checking server status");
			string xmlStr;

			xmlStr = GetServerStatus().Result;
			if (!string.IsNullOrWhiteSpace(xmlStr))
			{
				var _xmlSerializer = new xmlSerializer();
				var serverStatus = _xmlSerializer.Deserialize<ServerStatus>(xmlStr);
				if (serverStatus == null)
				{
					_logger.Warning($"Cannot read server status ({_serverURL})!");
				}
				else
				{
					_runner.CheckServerStatus(serverStatus);
				}
			}

			_logger.Information($"Done. Sleeping for {_wakeupTimerPeriod} seconds");
		}

		private async Task<string> GetServerStatus()
		{
			string xmlStr = string.Empty;
			using (var client = new HttpClient())
			{
				try
				{
					client.Timeout = TimeSpan.FromMilliseconds(_httpClientTimeout);
					xmlStr = await client.GetStringAsync(_serverURL);
				}
				catch (Exception ex)
				{
					_logger.Error($"Exception was thrown while getting server status: {ex.Message}");
				}
			}

			return xmlStr;
		}
	}
}
