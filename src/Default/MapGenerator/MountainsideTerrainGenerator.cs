using BlockColony.Core.Shared;
using BlockColony.Core.Surface;
using BlockColony.Default.MapGenerator;

namespace BlockColony.Core.MapGenerator {
	internal sealed class MountainsideTerrainGenerator : IMapTerrainGenerator {
		private readonly ITerrainManager _terrainManager;
		private readonly IRandom _random;
		private readonly MapGeneratorOptions _mapOptions;
		private readonly MountainsideTerrainGeneratorOptions _terrainOptions;

		public MountainsideTerrainGenerator(
			IRandom random,
			ITerrainManager terrainManager,
			MapGeneratorOptions options,
			MountainsideTerrainGeneratorOptions terrainOptions
		) {
			_random = random;
			_terrainManager = terrainManager;
			_mapOptions = options;
			_terrainOptions = terrainOptions;
		}

		void IMapTerrainGenerator.Initialize(
			ref MapCell cell
		) {
			cell.NewTemperature = (byte)_terrainOptions.AmbientTemperature;

			ITerrain terrain = _terrainManager[3];  // Magically know this is soil!
			cell.NewTerrainId = 0;
			cell.NewMoisture = 0;
			cell.TerrainCost = (short)terrain[_terrainOptions.AmbientTemperature].PathingCost;
		}

		void IMapTerrainGenerator.Build(
			IMap map
		) {
			int layerWidth = map.Columns / 4;
			// Applying layers from far-right to far-left order, otherwise
			// the function wouldn't know when to stop applying its terrain
			ApplyTerrainLayer( _terrainManager, map, _terrainOptions.Terrain.Far, layerWidth * 3, map.Columns );
			ApplyTerrainLayer( _terrainManager, map, _terrainOptions.Terrain.Middle, layerWidth * 2, map.Columns );
			ApplyTerrainLayer( _terrainManager, map, _terrainOptions.Terrain.Entry, layerWidth * 1, map.Columns );
			ApplyTerrainLayer( _terrainManager, map, _terrainOptions.Terrain.Outside, 0, map.Columns );

			// Otherwise the left-edge of the map will be jagged
			FillOutsideEdge( _terrainManager, map, _terrainOptions.Terrain.Outside );

			ApplyEasyRiver( _terrainManager, map, _terrainOptions.Terrain.River );
		}


		private class CellInitializer : IMapMethod {

			private readonly ITerrainManager _terrainManager;
			private readonly MapGeneratorOptions _mapOptions;
			private readonly MountainsideTerrainGeneratorOptions _terrainOptions;

			public CellInitializer(
				ITerrainManager terrainManager,
				MapGeneratorOptions mapOptions,
				MountainsideTerrainGeneratorOptions terrainOptions
			) {
				_terrainManager = terrainManager;
				_mapOptions = mapOptions;
				_terrainOptions = terrainOptions;
			}

			void IMapMethod.Invoke( ref MapCell cell ) {
				cell.NewTemperature = (byte)_terrainOptions.AmbientTemperature;

				ITerrain terrain = _terrainManager[3];  // Magically know this is soil!
				cell.NewTerrainId = 0;
				cell.NewMoisture = 0;
				cell.TerrainCost = (short)terrain[_terrainOptions.AmbientTemperature].PathingCost;
			}
		}

		// Walks the left-edge of the map filling in to the right where a
		// terrain hasn't otherwise been specified.
		private static void FillOutsideEdge(
			ITerrainManager terrainManager,
			IMap map,
			LayerTerrainOptions terrainOptions
		) {
			ITerrain terrain = terrainManager.GetByIdName( terrainOptions.IdName );
			for( int row = 0; row < map.Rows; row++ ) {

				int counter = 0;
				bool terrainUpdated;
				do {
					terrainUpdated = false;
					ref MapCell cell = ref map.GetCell( counter, row );
					while( cell.TerrainId == 0 ) {
						cell.TerrainId = terrain.Id;
						counter++;
						terrainUpdated = true;
					}

				} while( terrainUpdated );
			}
		}

		private void ApplyTerrainLayer(
			ITerrainManager terrainManager,
			IMap map,
			LayerTerrainOptions terrainOptions,
			int startColumn,
			int endColumn
		) {
			ITerrain terrain = terrainManager.GetByIdName( terrainOptions.IdName );
			for( int row = 0; row < map.Rows; row++ ) {
				// Drunken-walk down the map.  The farther you are from your
				// 'ideal' location the more correction is applied to drag
				// you closer to the ideal.
				int currentColumn = startColumn;
				int delta = startColumn - currentColumn;
				int direction = _random.Next( 10 ) + delta;
				if( direction < 4 ) {
					currentColumn -= 1;
				} else if( direction >= 6 ) {
					currentColumn += 1;
				}

				while( currentColumn < endColumn ) {
					ref MapCell cell = ref map.GetCell( currentColumn, row );
					// Ensures that the cell hasn't already been initialized by
					// something other layer.
					if( cell.TerrainId == 0 ) {
						cell.TerrainId = terrain.Id;
					}
					currentColumn += 1;
				}
			}
		}

		private void ApplyEasyRiver( ITerrainManager terrainManager, IMap map, RiverTerrainOptions riverOptions ) {
			int startColumn = map.Columns / 3;
			int startDrift = map.Columns / 10;

			// The river falls one third of the way across the map, within a
			// margin of +/- 10% of the width of the map
			startColumn += ( _random.Next( startDrift / 2 ) + ( startDrift / 2 ) );
			int currentColumn = startColumn;

			// Controls the length of a step in the drunken-walk.  This is the
			// minimum number of rows we go in a straight line before finding
			// a new column to occupy.
			int runLength = _random.Next( 3 ) + 2;

			ITerrain terrain = terrainManager.GetByIdName( riverOptions.Easy );
			for( int row = 0; row < map.Rows; row++ ) {
				for( int width = -1; width <= 1; width++ ) {
					ref MapCell cell = ref map.GetCell( currentColumn + width, row );
					cell.TerrainId = terrain.Id;
				}
				runLength -= 1;

				if( runLength == 0 ) {
					runLength = _random.Next( 3 ) + 3;
					int delta = startColumn - currentColumn;
					int direction = _random.Next( 10 ) + delta;
					if( direction < 3 ) {
						currentColumn -= 1;
					} else if( direction >= 7 ) {
						currentColumn += 1;
					}
				}
			}
		}
	}
}
