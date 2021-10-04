using System;

namespace BlockColony.Core.Surface {

	public delegate void MapCellInitializer( ref MapCell cell );

	public interface IMapFactory {
		IMap Create( int columns, int rows, MapCellInitializer initializer );
	}
}
