using System;
using System.Diagnostics;
using BlockColony.Core.Surface;
using BlockColony.Core.Pathfinding.AStar;

namespace BlockColony.Core.Pathfinding.Profiler.Tests {
	internal class Program {
		private static void Main( string[] _ ) {
			ProfileAStar();

			Console.ReadKey();
		}

		private static void ProfileAStar() {
			const int iterations = 10;
			const int size = 400;

			IMap map = new Map( size, size, new DefaultInitializer() );

			ref MapCell start = ref map.GetCell( (int)( (float)map.Columns * 0.25f ), (int)( (float)map.Rows * 0.25f ) );
			ref MapCell goal = ref map.GetCell( map.Columns - (int)( (float)map.Columns * 0.25f ), map.Rows - (int)( (float)map.Rows * 0.25f ) );
			IPathfinder pathfinder = new AStarPathfinder();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for( int i = 0; i < iterations; i++ ) {
				_ = pathfinder.GetPath( map, ref start, ref goal, Locomotion.Walk );
			}
			stopwatch.Stop();
			Console.WriteLine( $"A* {map.Columns}x{map.Rows}: {(float)stopwatch.ElapsedMilliseconds / (float)iterations}ms" );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Invoke( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Directions.All;
			}
		}

	}
}
