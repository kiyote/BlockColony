using System;

namespace BlockColony.Core.Surface {
	internal sealed class MapFactory : IMapFactory {
		IMap IMapFactory.Create( int columns, int rows, MapCellInitializer initializer ) {
			return new Map( columns, rows, initializer );
		}
	}
}
