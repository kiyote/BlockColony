using System;
using BlockColony.Core.Surface;

namespace BlockColony.Core.MapGenerator {
	public interface IMapGenerator {
		event EventHandler MapGenerationCompleted;

		IMap Map { get; }

		void Build( ITerrainManager terrainManager, MapGeneratorOptions options );
	}
}
