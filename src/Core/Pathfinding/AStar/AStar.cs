using System;
using System.Collections.Generic;
using BlockColony.Core.Surface;
using BlockColony.Core.Pathfinding.BlueRajah;
using Microsoft.Extensions.ObjectPool;

namespace BlockColony.Core.Pathfinding.AStar {
	internal class AStar : IMapNeighbourMethod {

		private readonly Queue<AStarPriorityQueueNode> _pool;
		private static readonly Route Empty = new Route();
		private readonly Dictionary<int, int> _costSoFar;
		private readonly Dictionary<int, int> _cameFrom;
		private readonly FastPriorityQueue<AStarPriorityQueueNode> _frontier;
		private int _currentIndex;
		private int _goalIndex;
		private Locomotion _locomotion;

		public AStar(
			int maximumNodes
		) {
			MaximumNodes = maximumNodes;
			// frontier is a List of key-value pairs:
			// Location, (float) priority
			_frontier = new FastPriorityQueue<AStarPriorityQueueNode>( maximumNodes );

			// Someone suggested making this a 2d field.
			// That will be worth looking at if you run into performance issues.
			_cameFrom = new Dictionary<int, int>();
			_costSoFar = new Dictionary<int, int>();
			_pool = new Queue<AStarPriorityQueueNode>();
		}

		public int MaximumNodes { get; }

		static public int Heuristic( IMap map, ref MapCell a, ref MapCell b ) {
			int dc = Math.Abs( a.Column - b.Column );
			int dr = Math.Abs( a.Row - b.Row );

			if( dc > map.HalfColumns ) {
				dc -= map.HalfColumns;
			}

			if( dr > map.HalfRows ) {
				dr -= map.HalfRows;
			}

			return dc + dr;
		}

		public Route GetPath( IMap map, int startColumn, int startRow, int goalColumn, int goalRow, Locomotion locomotion ) {
			return GetPath( map, ref map.GetCell( startColumn, startRow ), ref map.GetCell( goalColumn, goalRow ), locomotion );
		}

			// Conduct the A* search
		public Route GetPath( IMap map, ref MapCell start, ref MapCell goal, Locomotion locomotion ) {
			_locomotion = locomotion;
			if (!_pool.TryDequeue( out AStarPriorityQueueNode? startNode ) ) {
				startNode = new AStarPriorityQueueNode {
					CellIndex = start.Index
				};
			} else {
				startNode!.CellIndex = start.Index;
			}

			// Add the starting location to the frontier with a priority of 0
			_frontier.Enqueue( startNode, 0 );

			_cameFrom.Add( start.Index, start.Index ); // is set to start, None in example
			_costSoFar.Add( start.Index, 0 );

			while( _frontier.Count > 0 ) {
				// Get the Location from the frontier that has the lowest
				// priority, then remove that Location from the frontier
				AStarPriorityQueueNode current = _frontier.Dequeue();

				// If we're at the goal Location, stop looking.
				if( current.CellIndex == goal.Index ) {
					break;
				}

				// Neighbors will return a List of valid tile Locations
				// that are next to, diagonal to, above or below current
				_currentIndex = current.CellIndex;
				_goalIndex = goal.Index;
				map.ForEachNeighbour( current.CellIndex, this );

				_pool.Enqueue( current );
			}


			var path = new Route();
			int step = goal.Index;

			while( step != start.Index ) {
				if( !_cameFrom.ContainsKey( step ) ) {
					return Empty;
				}
				path.Add( step );
				step = _cameFrom[step];
			}
			// path.Add(start);
			path.Reverse();

			_frontier.Clear();
			_costSoFar.Clear();
			_cameFrom.Clear();

			return path;
		}

		void IMapNeighbourMethod.Invoke( IMap map, ref MapCell source, ref MapCell neighbour, Directions direction ) {
			if( ( source.Walkability & (byte)direction ) != (byte)direction ) {
				return;
			}
			// If neighbor is diagonal to current, graph.Cost(current,neighbor)
			// will return Sqrt(2). Otherwise it will return only the cost of
			// the neighbor, which depends on its type, as set in the TileType enum.
			// So if this is a normal floor tile (1) and it's neighbor is an
			// adjacent (not diagonal) floor tile (1), newCost will be 2,
			// or if the neighbor is diagonal, 1+Sqrt(2). And that will be the
			// value assigned to costSoFar[neighbor] below.
			int newCost = _costSoFar[_currentIndex] + map.Cost( _locomotion, _currentIndex, neighbour.Index );

			// If there's no cost assigned to the neighbor yet, or if the new
			// cost is lower than the assigned one, add newCost for this neighbor
			if( !_costSoFar.ContainsKey( neighbour.Index ) || newCost < _costSoFar[neighbour.Index] ) {

				// If we're replacing the previous cost, remove it
				if( _costSoFar.ContainsKey( neighbour.Index ) ) {
					_costSoFar.Remove( neighbour.Index );
					_cameFrom.Remove( neighbour.Index );
				}

				_costSoFar.Add( neighbour.Index, newCost );
				_cameFrom.Add( neighbour.Index, _currentIndex );
				ref MapCell goal = ref map.GetCell( _goalIndex );
				int priority = newCost + Heuristic( map, ref neighbour, ref goal );
				if( !_pool.TryDequeue( out AStarPriorityQueueNode? neighbourNode ) ) {
					neighbourNode = new AStarPriorityQueueNode {
						CellIndex = neighbour.Index
					};
				} else {
					neighbourNode!.CellIndex = neighbour.Index;
				}
				_frontier.Enqueue( neighbourNode, priority );
			}
		}
	}
}
