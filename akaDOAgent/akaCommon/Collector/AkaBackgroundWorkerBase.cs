using System;
using System.Threading;

namespace akaCommon.Collector
{
	public abstract class AkaBackgroundWorkerBase
	{
		readonly ManualResetEvent _manualReset;
		Thread _thread;
		protected int _nManagedThreadId;
		private readonly AkaFocusLogger _logger = new akaCommon.AkaFocusLogger();


		protected AkaBackgroundWorkerBase(ManualResetEvent resetEvent)
		{
			_manualReset = resetEvent;
		}

		protected abstract void ThreadProc();

		protected ManualResetEvent ResetEvent => _manualReset;

		public virtual void Start()
		{
			if (_thread == null)
			{
				_thread = new Thread(ThreadProc);
				_thread.Start();
			}
		}

		public virtual void Wait()
		{
			_thread.Join();
		}

		public AkaFocusLogger Logger => _logger;
		public Thread Thread => _thread;
		public int ManagedThreadId => _nManagedThreadId;
	}
}
