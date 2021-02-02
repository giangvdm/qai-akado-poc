using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using akaCommon;
using akaCommon.Collector;
using akaSensor.Windows;
using akaSensor.Windows.Collector;

namespace akaSensorAgent
{
	static class Program
	{
		static AkaFocusLogger _akaFocusLogger;
		static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

		static void Main(string[] args)
		{
			try
			{
				List<AkaBackgroundWorkerBase> collectors = new List<AkaBackgroundWorkerBase>(new AkaBackgroundWorkerBase[]
				{
					new FileTransferCollector(_manualResetEvent),
				});

				// Start all collector threads
				collectors.ForEach(x => x.Start());
				Thread thread = new Thread(ThreadWait);
				thread.Start(_manualResetEvent);


				// Wait for stop
				collectors.ForEach(x => x.Wait());
				thread.Join();
			}
			catch(Exception ex)
			{
				_akaFocusLogger.LogError(ex.Message, ex);
			}
		}

		/// <summary>
		/// Thread uses for communicating between Service monitor and Agent.
		/// If service send a command exit, this program will save all data and then automatic terminate.
		/// </summary>
		/// <param name="o"></param>
		static void ThreadWait(object o)
		{
			ManualResetEvent resetEvent = o as ManualResetEvent;
			var pipeServer = new NamedPipeServerStream("1FFBC9036670418B944083AE2CD53A78", PipeDirection.InOut, 10);

			StreamReader sr = new StreamReader(pipeServer);
			StreamWriter sw = new StreamWriter(pipeServer);

			do
			{
				try
				{
					pipeServer.WaitForConnection();
					string test;
					sw.WriteLine("Waiting");
					sw.Flush();
					pipeServer.WaitForPipeDrain();
					test = sr.ReadLine();
#if DEBUG
					Console.WriteLine(test);
#endif

					if (string.Compare(test, "exit", true) == 0)
					{
						resetEvent.Set();
						Thread.Sleep(2000);
						Process.GetCurrentProcess().Kill();
						break;
					}
				}
				catch (Exception ex)
				{
					AkaFocusLogger logger = new AkaFocusLogger();
					logger.LogError(ex.Message, ex);
				}
				finally
				{
					pipeServer.WaitForPipeDrain();

					if (pipeServer.IsConnected)
					{
						pipeServer.Disconnect();
					}
				}
			} while (true);
		}
	}
}
