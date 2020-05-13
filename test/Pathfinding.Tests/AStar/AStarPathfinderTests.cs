using System;
using Surface;
using NUnit.Framework;

namespace Pathfinding.AStar.Tests {
	[TestFixture]
	public class AStarPathfinderTests {

		private IPathfinder _pathfinder;
		private DefaultInitializer _defaultInitializer;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			_pathfinder = new AStarPathfinder();
			_defaultInitializer = new DefaultInitializer();
		}

		[TestCase( 1, 1, 8, 8, Locomotion.Fly )]
		[TestCase( 8, 8, 1, 1, Locomotion.Fly )]
		[TestCase( 1, 8, 8, 1, Locomotion.Fly )]
		[TestCase( 8, 1, 1, 8, Locomotion.Fly )]
		[TestCase( 1, 1, 8, 8, Locomotion.Walk )]
		[TestCase( 8, 8, 1, 1, Locomotion.Walk )]
		[TestCase( 1, 8, 8, 1, Locomotion.Walk )]
		[TestCase( 8, 1, 1, 8, Locomotion.Walk )]
		public void GetPath_WrappingSpace_ShortestPath(
			int startColumn,
			int startRow,
			int goalColumn,
			int goalRow,
			Locomotion locomotion
		) {
			Map map = new Map( 10, 10, _defaultInitializer );
			ref MapCell start = ref map.GetCell( startColumn, startRow );
			ref MapCell goal = ref map.GetCell( goalColumn, goalRow );
			Route path = _pathfinder.GetPath( map, ref start, ref goal, locomotion );

			CollectionAssert.IsNotEmpty( path );
			Assert.AreEqual( 3, path.Count );
		}

		[TestCase( 1, 1, 8, 8, Locomotion.Fly )]
		[TestCase( 8, 8, 1, 1, Locomotion.Fly )]
		[TestCase( 1, 8, 8, 1, Locomotion.Fly )]
		[TestCase( 8, 1, 1, 8, Locomotion.Fly )]
		[TestCase( 1, 1, 8, 8, Locomotion.Walk )]
		[TestCase( 8, 8, 1, 1, Locomotion.Walk )]
		[TestCase( 1, 8, 8, 1, Locomotion.Walk )]
		[TestCase( 8, 1, 1, 8, Locomotion.Walk )]
		public void GetPath_BoxedSpace_ShortedPath(
			int startColumn,
			int startRow,
			int goalColumn,
			int goalRow,
			Locomotion locomotion
		) {
			Map map = new Map( 10, 10, _defaultInitializer );

			for( int column = 0; column < 10; column++ ) {
				map.GetCell( column, 0 ).TerrainCost = 32000;
				map.GetCell( column, 9 ).TerrainCost = 32000;
			}
			for( int row = 0; row < 10; row++ ) {
				map.GetCell( 0, row ).TerrainCost = 32000;
				map.GetCell( 9, row ).TerrainCost = 32000;
			}

			ref MapCell start = ref map.GetCell( startColumn, startRow );
			ref MapCell goal = ref map.GetCell( goalColumn, goalRow );
			Route path = _pathfinder.GetPath( map, ref start, ref goal, locomotion );

			CollectionAssert.IsNotEmpty( path );
			Assert.AreEqual( 7, path.Count );
		}

		[TestCase( 1, 1, 1, 5, Locomotion.Walk )]
		[TestCase( 1, 1, 5, 1, Locomotion.Walk )]
		[TestCase( 1, 5, 1, 1, Locomotion.Walk )]
		[TestCase( 5, 1, 1, 1, Locomotion.Walk )]
		[TestCase( 1, 1, 1, 5, Locomotion.Fly )]
		[TestCase( 1, 1, 5, 1, Locomotion.Fly )]
		[TestCase( 1, 5, 1, 1, Locomotion.Fly )]
		[TestCase( 5, 1, 1, 1, Locomotion.Fly )]
		public void GetPath_StraightLine_ShortestPath(
			int startColumn,
			int startRow,
			int goalColumn,
			int goalRow,
			Locomotion locomotion
		) {
			Map map = new Map( 10, 10, _defaultInitializer );
			ref MapCell start = ref map.GetCell( startColumn, startRow );
			ref MapCell goal = ref map.GetCell( goalColumn, goalRow );
			Route path = _pathfinder.GetPath( map, ref start, ref goal, locomotion );

			CollectionAssert.IsNotEmpty( path );
			Assert.AreEqual( 4, path.Count );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Invoke( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Directions.All;
			}
		}
	}
}
