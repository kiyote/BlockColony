using System;
using System.Threading;
using BlockColony.Core.Surface;

namespace BlockColony.Core.MapGenerator {
	internal sealed class MapGenerator: IMapGenerator {

		private readonly IMapFactory _mapFactory;

		public MapGenerator(
			IMapFactory mapFactory
		) {
			_mapFactory = mapFactory;
		}

		public void Build(
			MapGeneratorOptions options,
			IMapTerrainGenerator terrainGenerator,
			Action<string> generationUpdated,
			Action<IMap> mapCompleted
		) {

			Thread thread = new Thread( () => {
				Run(
					options,
					terrainGenerator,
					generationUpdated,
					mapCompleted
				);
			} ) {
				Name = "Map Generator Thread"
			};
			thread.Start();
			// Thread will terminate when the Run completes which is the
			// completion of the map generation.
		}

		private void Run(
			MapGeneratorOptions options,
			IMapTerrainGenerator mapTerrainGenerator,
			Action<string> generationUpdated,
			Action<IMap> mapCompleted
		) {
			generationUpdated( "Map generation started." );

			generationUpdated( "Initializing map." );
			IMap map = _mapFactory.Create(
				options.Columns,
				options.Rows,
				mapTerrainGenerator.Initialize
			);
			generationUpdated( "Map initialized." );

			generationUpdated( "Building terrain." );
			mapTerrainGenerator.Build( map );
			generationUpdated( "Terrain completed." );

			generationUpdated( "Map generation completed." );
			mapCompleted( map );
		}
	}
}
