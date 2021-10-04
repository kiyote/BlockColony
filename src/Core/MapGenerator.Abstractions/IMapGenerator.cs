using System;
using BlockColony.Core.Surface;

namespace BlockColony.Core.MapGenerator {
	public interface IMapGenerator {

		void Build(
			MapGeneratorOptions options,
			IMapTerrainGenerator terrainGenerator,
			Action<string> generationUpdated,
			Action<IMap> mapCompleted
		);
	}
}
