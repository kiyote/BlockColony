using System;
using Surface;

namespace MapGenerator {
	public interface IMapGenerator {
		event EventHandler MapGenerationCompleted;

		Map Map { get; }

		void Build( TerrainManager terrainManager, MapGeneratorOptions options );
	}
}