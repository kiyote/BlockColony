using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Surface;

namespace Pathfinding.AStar {
	// This provides an implementation pool so that if we're running the 
	// pathfinding async then there is always an implementation available
	// to be used exclusively by that task run.  If we're running synchronously
	// then the pool implementation is pointless.
	public class AStarPathfinder : IPathfinder {

		private readonly ConcurrentBag<AStar> _pool;

		public AStarPathfinder() {
			_pool = new ConcurrentBag<AStar>();
		}

		Route IPathfinder.GetPath( Map map, ref MapCell start, ref MapCell goal, Locomotion locomotion ) {
			if (!_pool.TryTake(out AStar impl)) {
				impl = new AStar( map.Columns * map.Rows );
			}

			var path = impl.GetPath( map, ref start, ref goal, locomotion );

			_pool.Add( impl );

			return path;
		}

		Task<Route> IPathfinder.GetPathAsync( Map map, int startColumn, int startRow, int goalColumn, int goalRow, Locomotion locomotion ) {

			if( !_pool.TryTake( out AStar impl ) ) {
				impl = new AStar( map.Columns * map.Rows );
			}

			return Task.Run( () => {
				var path = impl.GetPath( map, startColumn, startRow, goalColumn, goalRow, locomotion );

				_pool.Add( impl );

				return path;
			} );
		}
	}
}