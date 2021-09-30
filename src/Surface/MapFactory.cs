namespace BlockColony.Core.Surface {
	internal sealed class MapFactory : IMapFactory {
		IMap IMapFactory.Create( int columns, int rows, IMapMethod initializer ) {
			return new Map( columns, rows, initializer );
		}
	}
}
