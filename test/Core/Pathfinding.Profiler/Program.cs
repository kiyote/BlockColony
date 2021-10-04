using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BlockColony.Core.Pathfinding.AStar;
using BlockColony.Core.Surface;

namespace BlockColony.Core.Pathfinding.Profiler.Tests {
	public class Program {
		public static void Main( string[] _ ) {
			BenchmarkDotNet.Reports.Summary summary = BenchmarkRunner.Run<Pathfinding>();
		}

		[MemoryDiagnoser]
		public class Pathfinding {

			private readonly IMap _map;
			private readonly IPathfinder _astarPathfinder;

			public Pathfinding() {
				const int size = 400;

				_map = new Map( size, size, new DefaultInitializer() );
				_astarPathfinder = new AStarPathfinder();
			}

			[Benchmark]
			public void ProfileAStar() {

				ref MapCell start = ref _map.GetCell( (int)( (float)_map.Columns * 0.25f ), (int)( (float)_map.Rows * 0.25f ) );
				ref MapCell goal = ref _map.GetCell( _map.Columns - (int)( (float)_map.Columns * 0.25f ), _map.Rows - (int)( (float)_map.Rows * 0.25f ) );
				_astarPathfinder.GetPath( _map, ref start, ref goal, Locomotion.Walk );
			}

			private class DefaultInitializer : IMapMethod {
				void IMapMethod.Invoke( ref MapCell cell ) {
					cell.TerrainCost = 100;
					cell.Walkability = (byte)Directions.All;
				}
			}
		}

	}
}
