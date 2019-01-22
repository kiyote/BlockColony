using NUnit.Framework;

namespace Surface.Tests {
	[TestFixture]
	public class MapTests {

		private Map _map;

		[OneTimeSetUp]
		public void TestInitialize() {
			const int Rows = 3;
			const int Columns = 3;
			_map = new Map( Columns, Rows, new DefaultInitializer() );
		}

		[TestCase( 1, 0, Direction.North, 1, 2 )]
		[TestCase( 0, 1, Direction.West, 2, 1 )]
		[TestCase( 2, 1, Direction.East, 0, 1 )]
		[TestCase( 1, 2, Direction.South, 1, 0 )]
		[TestCase( 0, 0, Direction.NorthWest, 2, 2 )]
		[TestCase( 1, 0, Direction.NorthWest, 0, 2 )]
		[TestCase( 2, 0, Direction.NorthEast, 0, 2 )]
		[TestCase( 1, 0, Direction.NorthEast, 2, 2 )]
		[TestCase( 0, 2, Direction.SouthWest, 2, 0 )]
		[TestCase( 1, 2, Direction.SouthWest, 0, 0 )]
		[TestCase( 2, 2, Direction.SouthEast, 0, 0 )]
		[TestCase( 1, 2, Direction.SouthEast, 2, 0 )]
		public void GetNeighbour_TestWrapping_CoordinatesCorrect(
			short column,
			short row,
			Direction direction,
			short expectedColumn,
			short expectedRow
		) {
			var source = _map.GetCell( column, row );
			var actual = _map.GetNeighbour( ref source, direction );
			Assert.AreEqual( expectedColumn, actual.Column );
			Assert.AreEqual( expectedRow, actual.Row );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
		}
	}
}