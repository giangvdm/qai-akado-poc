using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using akaSensor.Windows;
using System.Reflection;
using System.Text;

namespace akaFocus.Service
{
	public class Worker : BackgroundService
	{
		private const string _agentFileName = "akaSensorAgent.exe";
		private readonly ILogger<Worker> _logger;

		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			string sDirName = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string sAgentFile = System.IO.Path.Combine(sDirName, _agentFileName);

			while (!stoppingToken.IsCancellationRequested)
			{
				int nRun = Win32Invoker.IsProcessRunning(new StringBuilder(_agentFileName));
				_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
				await Task.Delay(1000, stoppingToken);
			}
		}
	}
}
