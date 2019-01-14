using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surface;

namespace Pathfinding.AStar {
	public class AStarPathfinder : IPathfinder {

		private readonly ConcurrentBag<AStar> _pool;

		public AStarPathfinder() {
			_pool = new ConcurrentBag<AStar>();
		}

		List<int> IPathfinder.GetPath( Map map, ref MapCell start, ref MapCell goal, Locomotion locomotion ) {
			if (!_pool.TryTake(out AStar impl)) {
				impl = new AStar( map.Columns * map.Rows );
			}

			var path = impl.GetPath( map, ref start, ref goal, locomotion );

			_pool.Add( impl );

			return path;
		}

		Task<List<int>> IPathfinder.GetPathAsync( Map map, int startColumn, int startRow, int goalColumn, int goalRow, Locomotion locomotion ) {

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