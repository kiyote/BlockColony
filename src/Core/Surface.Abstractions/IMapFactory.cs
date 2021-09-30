namespace BlockColony.Core.Surface {
	public interface IMapFactory {
		IMap Create( int columns, int rows, IMapMethod initializer );
	}
}
