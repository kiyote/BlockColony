using System;
using System.Threading;
using BlockColony.Core.Surface;
using BlockColony.Core.Shared;

namespace BlockColony.Core.MapGenerator {
	internal sealed class Generator : IMapGenerator {
		private ITerrainManager _terrainManager;
		private MapGeneratorOptions _options;
		private readonly IRandom _random;
		private readonly IMapFactory _mapFactory;

		private IMap _map;
		private event EventHandler _mapCompleted;

		public Generator(
			IMapFactory mapFactory,
			IRandom random
		) {
			_mapFactory = mapFactory;
			_random = random;
		}

		IMap IMapGenerator.Map {
			get {
				return _map;
			}
		}

		// This event is raised on the map generator thread, so don't use this
		// except in exceptionally well controlled circumstances.
		event EventHandler IMapGenerator.MapGenerationCompleted {
			add {
				_mapCompleted += value;
			}

			remove {
				_mapCompleted -= value;
			}
		}

		void IMapGenerator.Build(
			ITerrainManager terrainManager,
			MapGeneratorOptions options
		) {
			_terrainManager = terrainManager;
			_options = options;

			_map = default;
			Thread thread = new Thread( Run ) {
				Name = "Map Generator Thread"
			};
			thread.Start();
			// Thread will terminate when the Run completes which is the
			// completion of the map generation and the population of _map.
		}

		private void Run() {
			IMap map = _mapFactory.Create(
				_options.Columns,
				_options.Rows,
				new CellInitializer(
					_terrainManager,
					_options )
			);

			int layerWidth = map.Columns / 4;
			// Applying layers from far-right to far-left order, otherwise
			// the function wouldn't know when to stop applying its terrain
			ApplyTerrainLayer( _terrainManager, map, _options.Terrain.Far, layerWidth * 3, map.Columns );
			ApplyTerrainLayer( _terrainManager, map, _options.Terrain.Middle, layerWidth * 2, map.Columns );
			ApplyTerrainLayer( _terrainManager, map, _options.Terrain.Entry, layerWidth * 1, map.Columns );
			ApplyTerrainLayer( _terrainManager, map, _options.Terrain.Outside, 0, map.Columns );

			// Otherwise the left-edge of the map will be jagged
			FillOutsideEdge( _terrainManager, map, _options.Terrain.Outside );

			ApplyEasyRiver( _terrainManager, map, _options.Terrain.River );

			Interlocked.Exchange( ref _map, map );
			_mapCompleted?.Invoke( this, EventArgs.Empty );
		}

		private class CellInitializer : IMapMethod {

			private readonly ITerrainManager _terrainManager;
			private readonly MapGeneratorOptions _options;

			public CellInitializer(
				ITerrainManager terrainManager,
				MapGeneratorOptions options
			) {
				_terrainManager = terrainManager;
				_options = options;
			}

			void IMapMethod.Invoke( ref MapCell cell ) {
				cell.NewTemperature = (byte)_options.AmbientTemperature;

				ITerrain terrain = _terrainManager?[ 3 ];  // Magically know this is soil!
				cell.NewTerrainId = 0;
				cell.NewMoisture = 0;
				cell.TerrainCost = (short)terrain[ _options.AmbientTemperature ].PathingCost;
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
