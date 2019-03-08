using System;
using System.Threading;
using Mob;
using Work;

namespace Simulation
{
    public class SimulationManager
    {
		private Thread _thread;
		private AutoResetEvent _gate;
		private bool _terminated;
		private bool _isRunning;
		private long _uiElapsedMilliseconds;
		private readonly ActorManager _actors;

		public SimulationManager(
			ActorManager actorManager
		) {
			_actors = actorManager;

			_thread = new Thread( Run );
			_gate = new AutoResetEvent( false );
			_terminated = false;
		}

		// These events run on the simulation thread, so use these at
		// your own peril!
		public event EventHandler Started;
		public event EventHandler Stopped;

		public void Start() {
			if (!_isRunning) {
				_thread.Start();
			}
		}

		public void Stop() {
			if (_isRunning) {
				_terminated = true;
				_gate.Set();
				_thread.Join();
			}
		}

		public void UiUpdate(long elapsedMilliseconds) {
			Interlocked.Add( ref _uiElapsedMilliseconds, elapsedMilliseconds );
			_gate.Set();
		}

		private void Run() {
			_isRunning = true;
			Started?.Invoke(this, EventArgs.Empty);
			while (!_terminated) {

				long elapsedMilliseconds = Interlocked.Exchange( ref _uiElapsedMilliseconds, 0 );

				if (elapsedMilliseconds > 0) {
					// Perform a simulation tick
					_actors.SimulationUpdate();
				}

				_gate.WaitOne();
			}
			_isRunning = false;
			Stopped?.Invoke(this, EventArgs.Empty);
		}
    }
}
