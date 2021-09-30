
namespace BlockColony.Core.Shared {
	public interface IJson {
		public string Serialize<T>( T data );
		public T Deserialize<T>( string json );
	}
}
