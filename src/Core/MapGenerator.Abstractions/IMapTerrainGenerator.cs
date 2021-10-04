using BlockColony.Core.Surface;

namespace BlockColony.Core.MapGenerator {
	public interface IMapTerrainGenerator {

		void Initialize(
			ref MapCell cell
		);

		void Build(
			IMap map
		);
	}
}
