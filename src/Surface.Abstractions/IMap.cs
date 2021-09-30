namespace BlockColony.Core.Surface {
	public interface IMap {
		int Columns { get; }
		int HalfColumns { get; }
		int HalfRows { get; }
		int Rows { get; }

		int Cost( Locomotion locomotion, int sourceIndex, int targetIndex );
		void ForEachNeighbour( int cellIndex, IMapNeighbourMethod method );
		ref MapCell GetCell( int index );
		ref MapCell GetCell( int column, int row );
		ref MapCell GetNeighbour( ref MapCell cell, Directions direction );
	}
}
