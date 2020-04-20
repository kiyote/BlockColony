using System;
using System.Collections.Concurrent;
using System.Threading;
using Surface;
using Pathfinding.AStar;
using System.Diagnostics;

namespace Pathfinding {
	public class PathfindingManager {
		private Thread _thread;
		private readonly AutoResetEvent _gate;
		private bool _terminated;
		private readonly ConcurrentQueue<PathRequest> _requests;
		private readonly IPathfinder _pathfinder;

		private class PathRequest {
			public Map Map;
			public int StartColumn;
			public int StartRow;
			public int GoalColumn;
			public int GoalRow;
			public Locomotion Locomotion;
			public IPathfindingCallback Callback;
			public int CallbackContext;
		}

		public PathfindingManager()
			: this( new AStarPathfinder() ) {
		}

		public PathfindingManager(
			IPathfinder pathfinder
		) {
			_gate = new AutoResetEvent( false );
			_pathfinder = pathfinder;
			IsRunning = false;
			_requests = new ConcurrentQueue<PathRequest>();
		}

#if DEBUG
		// These run on the Pathfinding thread, so use them at your own risk
		public event EventHandler Started;
		public event EventHandler Stopped;
#endif

		public bool IsRunning { get; private set; }

		public void GetPath( Map map, ref MapCell start, ref MapCell goal, Locomotion locomotion, IPathfindingCallback callback, int callbackContext ) {
			if( !IsRunning ) {
				throw new InvalidOperationException( "Attempt to path with stopped pathfinding manager." );
			}

			var request = new PathRequest {
				Map = map,
				StartColumn = start.Column,
				StartRow = start.Row,
				GoalColumn = goal.Column,
				GoalRow = goal.Row,
				Locomotion = locomotion,
				Callback = callback,
				CallbackContext = callbackContext
			};
			_requests.Enqueue( request );
			_gate.Set();
		}

		public void Start() {
			if( !IsRunning ) {
				_thread = new Thread( Run ) {
					Name = "Pathfinding Thread"
				};
				_thread.Start();
			}
		}

		public void Stop() {
			if( IsRunning ) {
				_terminated = true;
				_gate.Set();
				_thread.Join();
			}
		}

		private void Run() {
			IsRunning = true;
#if DEBUG
			Started?.Invoke( this, EventArgs.Empty );
#endif
			while( !_terminated ) {

				while( !_requests.IsEmpty ) {
					if( _terminated ) {
						break;
					}
					if( _requests.TryDequeue( out PathRequest request ) ) {
#if DEBUG
						Debug.WriteLine( "PathfindingManager::Run: Request Found" );
#endif
						Route path = _pathfinder.GetPath(
							request.Map,
							ref request.Map.GetCell( request.StartColumn, request.StartRow ),
							ref request.Map.GetCell( request.GoalColumn, request.GoalRow ),
							request.Locomotion );
						request.Callback.PathFound( path, request.CallbackContext );
					}

				}
				_gate.WaitOne( 1000 ); // Automatically unlock after 1s just in case
			}
			IsRunning = false;
#if DEBUG
			Stopped?.Invoke( this, EventArgs.Empty );
#endif
		}
	}
}
