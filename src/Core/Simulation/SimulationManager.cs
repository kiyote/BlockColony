using System;
using System.Threading;
using BlockColony.Core.Mob;
using BlockColony.Core.Surface;

namespace BlockColony.Core.Simulation {
	public sealed class SimulationManager: IDisposable {
		private Thread _thread;
		private readonly AutoResetEvent _gate;
		private bool _terminated;
		private bool _isRunning;
		private long _uiElapsedMilliseconds;
		private readonly ActorManager _actors;
		private readonly IMapProvider _mapProvider;

		public SimulationManager(
			ActorManager actorManager,
			IMapProvider mapProvider
		) {
			_actors = actorManager;
			_mapProvider = mapProvider;
			_thread = new Thread( Run );
			_gate = new AutoResetEvent( false );
			_terminated = false;
		}

#if DEBUG
		// These events run on the simulation thread, so use these at
		// your own peril!
		public event EventHandler? Started;
		public event EventHandler? Stopped;
#endif

		public void Start() {
			if( !_isRunning ) {
				_thread = new Thread( Run ) {
					Name = "Simulation Thread"
				};
				_thread.Start();
			}
		}

		public void Stop() {
			if( _isRunning ) {
				_terminated = true;
				_gate.Set();
				_thread.Join();
			}
		}

		// Call this from the UI thread
		public void UiUpdate( long elapsedMilliseconds ) {
			Interlocked.Add( ref _uiElapsedMilliseconds, elapsedMilliseconds );
			_gate.Set();
		}

		private void Run() {
			_isRunning = true;
#if DEBUG
			Started?.Invoke( this, EventArgs.Empty );
#endif
			while( !_terminated ) {

				long elapsedMilliseconds = Interlocked.Exchange( ref _uiElapsedMilliseconds, 0 );

				// Perform a simulation tick
				if( elapsedMilliseconds > 0 ) {
					IMap map = _mapProvider.Current();
					_actors.SimulationUpdate( map );
				}

				_gate.WaitOne();
			}
			_isRunning = false;
#if DEBUG
			Stopped?.Invoke( this, EventArgs.Empty );
#endif
		}

		public void Dispose() {
			_gate?.Dispose();
		}
	}
}
