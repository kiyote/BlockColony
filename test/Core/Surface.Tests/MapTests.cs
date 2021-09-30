using NUnit.Framework;

namespace BlockColony.Core.Surface.Tests {
	[TestFixture]
	public class MapTests {

		private IMap _map;

		[OneTimeSetUp]
		public void TestInitialize() {
			const int Rows = 3;
			const int Columns = 3;
			_map = new Map( Columns, Rows, new DefaultInitializer() );
		}

		[TestCase( 1, 0, Directions.North, 1, 2 )]
		[TestCase( 0, 1, Directions.West, 2, 1 )]
		[TestCase( 2, 1, Directions.East, 0, 1 )]
		[TestCase( 1, 2, Directions.South, 1, 0 )]
		[TestCase( 0, 0, Directions.NorthWest, 2, 2 )]
		[TestCase( 1, 0, Directions.NorthWest, 0, 2 )]
		[TestCase( 2, 0, Directions.NorthEast, 0, 2 )]
		[TestCase( 1, 0, Directions.NorthEast, 2, 2 )]
		[TestCase( 0, 2, Directions.SouthWest, 2, 0 )]
		[TestCase( 1, 2, Directions.SouthWest, 0, 0 )]
		[TestCase( 2, 2, Directions.SouthEast, 0, 0 )]
		[TestCase( 1, 2, Directions.SouthEast, 2, 0 )]
		public void GetNeighbour_TestWrapping_CoordinatesCorrect(
			short column,
			short row,
			Directions direction,
			short expectedColumn,
			short expectedRow
		) {
			MapCell source = _map.GetCell( column, row );
			MapCell actual = _map.GetNeighbour( ref source, direction );
			Assert.AreEqual( expectedColumn, actual.Column );
			Assert.AreEqual( expectedRow, actual.Row );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Invoke( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Directions.All;
			}
		}
	}
}
