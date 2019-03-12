using System;

namespace Surface {
	public class Map {
		private const int Unwalkable = int.MaxValue / 2;

		private MapCell[] _cells;
		private int _rowSize;

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public Map( int columns, int rows, IMapMethod initializer ) {
			if( initializer == default( IMapMethod ) ) {
				throw new ArgumentException( nameof( initializer ) );
			}

			if( columns <= 0 ) {
				throw new ArgumentException( nameof( columns ) );
			}

			if( rows <= 0 ) {
				throw new ArgumentException( nameof( rows ) );
			}

			_rowSize = columns;

			Columns = columns;
			HalfColumns = columns / 2;
			Rows = rows;
			HalfRows = rows / 2;
			_cells = new MapCell[ columns * rows ];

			for( int row = 0; row < rows; row++ ) {
				for( int column = 0; column < columns; column++ ) {
					var index = ( row * _rowSize ) + column;
					ref var cell = ref _cells[ index ];
					cell.Column = (short)column;
					cell.Row = (short)row;
					cell.Index = index;

					initializer.Do( ref cell );
				}
			}
		}

		public int Columns { get; }

		public int HalfColumns { get; }

		public int Rows { get; }

		public int HalfRows { get; }

		public ref MapCell GetCell( int index ) {
			return ref _cells[ index ];
		}

		public ref MapCell GetCell( int column, int row ) {
			return ref GetWrappedCell( column, row );
		}

		public void ForEachNeighbour( int cellIndex, IMapNeighbourMethod method ) {
			ref var cell = ref _cells[ cellIndex ];
			method.Do( ref cell, ref GetWrappedCell( cell.Column - 1, cell.Row - 1 ), Direction.NorthWest );
			method.Do( ref cell, ref GetWrappedCell( cell.Column, cell.Row - 1 ), Direction.North );
			method.Do( ref cell, ref GetWrappedCell( cell.Column + 1, cell.Row - 1 ), Direction.NorthEast );

			method.Do( ref cell, ref GetWrappedCell( cell.Column - 1, cell.Row ), Direction.West );
			method.Do( ref cell, ref GetWrappedCell( cell.Column + 1, cell.Row ), Direction.East );

			method.Do( ref cell, ref GetWrappedCell( cell.Column - 1, cell.Row + 1 ), Direction.SouthWest );
			method.Do( ref cell, ref GetWrappedCell( cell.Column, cell.Row + 1 ), Direction.South );
			method.Do( ref cell, ref GetWrappedCell( cell.Column + 1, cell.Row + 1 ), Direction.SouthEast );
		}

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public int Cost( Locomotion locomotion, int sourceIndex, int targetIndex ) {
			ref var source = ref _cells[ sourceIndex ];
			ref var target = ref _cells[ targetIndex ];
			int result;

			if( target.TerrainLevel != 0 ) {
				return Unwalkable;
			}

			if( ( source.Column == target.Column ) ||
				( source.Row == target.Row ) ) {
				result = source.TerrainCost + target.TerrainCost;
			} else {
				result = ( source.TerrainCost + Sqrt( source.TerrainCost + target.TerrainCost ) );
			}

			return result;
		}

		public ref MapCell GetNeighbour( ref MapCell cell, Direction direction ) {
			switch( direction ) {
				case Direction.North:
					return ref GetWrappedCell( cell.Column, cell.Row - 1 );
				case Direction.NorthEast:
					return ref GetWrappedCell( cell.Column + 1, cell.Row - 1 );
				case Direction.East:
					return ref GetWrappedCell( cell.Column + 1, cell.Row );
				case Direction.SouthEast:
					return ref GetWrappedCell( cell.Column + 1, cell.Row + 1 );
				case Direction.South:
					return ref GetWrappedCell( cell.Column, cell.Row + 1 );
				case Direction.SouthWest:
					return ref GetWrappedCell( cell.Column - 1, cell.Row + 1 );
				case Direction.West:
					return ref GetWrappedCell( cell.Column - 1, cell.Row );
				case Direction.NorthWest:
					return ref GetWrappedCell( cell.Column - 1, cell.Row - 1 );
				default:
					throw new ArgumentException( "Unknown direction" );
			}
		}

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		internal ref MapCell GetWrappedCell( int column, int row ) {
			if( column < 0 ) {
				column += Columns;
			} else if( column >= Columns ) {
				column -= Columns;
			}

			if( row < 0 ) {
				row += Rows;
			} else if( row >= Rows ) {
				row -= Rows;
			}

			int index = ( row * _rowSize ) + column;
			return ref _cells[ index ];

		}

		private static int Sqrt( int num ) {
			if( 0 == num ) { return 0; }  // Avoid zero divide  
			int n = ( num / 2 ) + 1;       // Initial estimate, never low  
			int n1 = ( n + ( num / n ) ) / 2;
			while( n1 < n ) {
				n = n1;
				n1 = ( n + ( num / n ) ) / 2;
			} // end while  
			return n;
		} // end Isqrt()  
	}
}