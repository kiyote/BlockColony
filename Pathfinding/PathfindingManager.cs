using System;
using System.Collections.Concurrent;
using System.Threading;
using Surface;
using Pathfinding.AStar;

namespace Pathfinding {
	public class PathfindingManager {
		private Thread _thread;
		private AutoResetEvent _gate;
		private bool _terminated;
		private ConcurrentQueue<PathRequest> _requests;
		private IPathfinder _pathfinder;
		private bool _isRunning;

		private class PathRequest {
			public Map Map;
			public int StartColumn;
			public int StartRow;
			public int GoalColumn;
			public int GoalRow;
			public Locomotion Locomotion;
			public IPathfindingCallback Callback;
		}

		public PathfindingManager() {
			_gate = new AutoResetEvent( false );
			_pathfinder = new AStarPathfinder();
			_thread = new Thread( Run ) {
				Name = "Pathfinding"
			};
			_isRunning = false;
			_requests = new ConcurrentQueue<PathRequest>();
		}

		public event EventHandler Started;

		public event EventHandler Stopped;

		public bool IsRunning {
			get {
				return _isRunning;
			}
		}

		public void GetPath( Map map, ref MapCell start, ref MapCell goal, Locomotion locomotion, IPathfindingCallback callback ) {
			if( !_isRunning ) {
				throw new InvalidOperationException( "Attempt to path with stopped pathfinding manager." );
			}

			var request = new PathRequest {
				Map = map,
				StartColumn = start.Column,
				StartRow = start.Row,
				GoalColumn = goal.Column,
				GoalRow = goal.Row,
				Locomotion = locomotion,
				Callback = callback
			};
			_requests.Enqueue( request );
			_gate.Set();
		}

		public void Start() {
			if( !_isRunning ) {
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

		private void Run() {
			_isRunning = true;
			Started?.Invoke( this, EventArgs.Empty );
			while( !_terminated ) {

				while( !_requests.IsEmpty ) {
					if( _terminated ) {
						break;
					}
					if( _requests.TryDequeue( out PathRequest request ) ) {
						var path = _pathfinder.GetPath(
							request.Map,
							ref request.Map.GetCell( request.StartColumn, request.StartRow ),
							ref request.Map.GetCell( request.GoalColumn, request.GoalRow ),
							request.Locomotion );
						request.Callback.PathFound( path );
					}

				}
				_gate.WaitOne( 5000 ); // Automatically unlock after 5s just in case
			}
			_isRunning = false;
			Stopped?.Invoke( this, EventArgs.Empty );
		}
	}
}