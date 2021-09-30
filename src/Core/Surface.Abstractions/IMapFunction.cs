namespace BlockColony.Core.Surface {
	public interface IMapFunction<T> {
		T Invoke( ref MapCell cell );
	}
}
