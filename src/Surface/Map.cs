using System;

namespace Surface {
	public class Map {
		private const int Unwalkable = int.MaxValue / 2;

		private readonly MapCell[] _cells;
		private readonly int _rowSize;

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public Map( int columns, int rows, IMapMethod initializer ) {
			if( initializer == default( IMapMethod ) ) {
				throw new ArgumentNullException( nameof( initializer ) );
			}

			if( columns <= 0 ) {
				throw new ArgumentException( "Columns cannot be <= 0", nameof( columns ) );
			}

			if( rows <= 0 ) {
				throw new ArgumentException( "Rows cannot be <= 0", nameof( rows ) );
			}

			_rowSize = columns;

			Columns = columns;
			HalfColumns = columns / 2;
			Rows = rows;
			HalfRows = rows / 2;
			_cells = new MapCell[ columns * rows ];

			for( int row = 0; row < rows; row++ ) {
				for( int column = 0; column < columns; column++ ) {
					int index = ( row * _rowSize ) + column;
					ref MapCell cell = ref _cells[ index ];
					cell.Column = (short)column;
					cell.Row = (short)row;
					cell.Index = index;

					initializer.Invoke( ref cell );
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
			if (method == default) {
				throw new ArgumentNullException( nameof( method ) );
			}

			ref MapCell cell = ref _cells[ cellIndex ];
			method.Invoke( ref cell, ref GetWrappedCell( cell.Column - 1, cell.Row - 1 ), Directions.NorthWest );
			method.Invoke( ref cell, ref GetWrappedCell( cell.Column, cell.Row - 1 ), Directions.North );
			method.Invoke( ref cell, ref GetWrappedCell( cell.Column + 1, cell.Row - 1 ), Directions.NorthEast );

			method.Invoke( ref cell, ref GetWrappedCell( cell.Column - 1, cell.Row ), Directions.West );
			method.Invoke( ref cell, ref GetWrappedCell( cell.Column + 1, cell.Row ), Directions.East );

			method.Invoke( ref cell, ref GetWrappedCell( cell.Column - 1, cell.Row + 1 ), Directions.SouthWest );
			method.Invoke( ref cell, ref GetWrappedCell( cell.Column, cell.Row + 1 ), Directions.South );
			method.Invoke( ref cell, ref GetWrappedCell( cell.Column + 1, cell.Row + 1 ), Directions.SouthEast );
		}

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public int Cost( Locomotion _, int sourceIndex, int targetIndex ) {
			ref MapCell source = ref _cells[ sourceIndex ];
			ref MapCell target = ref _cells[ targetIndex ];
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

		public ref MapCell GetNeighbour( ref MapCell cell, Directions direction ) {
			switch( direction ) {
				case Directions.North:
					return ref GetWrappedCell( cell.Column, cell.Row - 1 );
				case Directions.NorthEast:
					return ref GetWrappedCell( cell.Column + 1, cell.Row - 1 );
				case Directions.East:
					return ref GetWrappedCell( cell.Column + 1, cell.Row );
				case Directions.SouthEast:
					return ref GetWrappedCell( cell.Column + 1, cell.Row + 1 );
				case Directions.South:
					return ref GetWrappedCell( cell.Column, cell.Row + 1 );
				case Directions.SouthWest:
					return ref GetWrappedCell( cell.Column - 1, cell.Row + 1 );
				case Directions.West:
					return ref GetWrappedCell( cell.Column - 1, cell.Row );
				case Directions.NorthWest:
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
