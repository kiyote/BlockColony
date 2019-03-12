using System;
using System.Threading;
using Surface;

namespace MapGenerator {
	internal class Generator : IMapGenerator {
		private TerrainManager _terrainManager;
		private MapGeneratorOptions _options;
		private short _rows;
		private short _columns;
		private Random _random;

		private Map _map;
		private event EventHandler _mapCompleted;

		Map IMapGenerator.Map {
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
			TerrainManager terrainManager,
			MapGeneratorOptions options
		) {
			_random = new Random();
			_terrainManager = terrainManager;
			_options = options;
			_rows = options.Rows;
			_columns = options.Columns;

			_map = default( Map );
			Thread thread = new Thread( Run );
			thread.Name = "Map Generator Thread";
			thread.Start();
			// Thread will terminate when the Run completes which is the
			// completion of the map generation and the population of _map.
		}

		private void Run() {
			Map map = new Map(
				_options.Columns,
				_options.Rows,
				new CellInitializer(
					_terrainManager,
					_options ) );

			var layerWidth = map.Columns / 4;
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

			private TerrainManager _terrainManager;
			private MapGeneratorOptions _options;

			public CellInitializer(
				TerrainManager terrainManager,
				MapGeneratorOptions options
			) {
				_terrainManager = terrainManager;
				_options = options;
			}

			void IMapMethod.Do( ref MapCell cell ) {
				cell.NewTemperature = (byte)_options.AmbientTemperature;

				var terrain = _terrainManager?[ 3 ];  // Magically know this is soil!
				cell.NewTerrainId = 0;
				cell.NewMoisture = 0;
				cell.TerrainCost = (short)terrain[ _options.AmbientTemperature ].PathingCost;
			}
		}

		// Walks the left-edge of the map filling in to the right where a
		// terrain hasn't otherwise been specified.
		private void FillOutsideEdge(
			TerrainManager terrainManager,
			Map map,
			LayerTerrainOptions terrainOptions
		) {
			var terrain = terrainManager.GetByIdName( terrainOptions.IdName );
			for( int row = 0; row < map.Rows; row++ ) {

				var counter = 0;
				bool terrainUpdated = false;
				do {
					terrainUpdated = false;
					ref var cell = ref map.GetCell( counter, row );
					while( cell.TerrainId == 0 ) {
						cell.TerrainId = terrain.Id;
						counter++;
						terrainUpdated = true;
					}

				} while( terrainUpdated );
			}
		}

		private void ApplyTerrainLayer(
			TerrainManager terrainManager,
			Map map,
			LayerTerrainOptions terrainOptions,
			int startColumn,
			int endColumn
		) {
			var terrain = terrainManager.GetByIdName( terrainOptions.IdName );
			for( int row = 0; row < map.Rows; row++ ) {
				// Drunken-walk down the map.  The farther you are from your
				// 'ideal' location the more correction is applied to drag
				// you closer to the ideal.
				var currentColumn = startColumn;
				var delta = startColumn - currentColumn;
				var direction = _random.Next( 10 ) + delta;
				if( direction < 4 ) {
					currentColumn -= 1;
				} else if( direction >= 6 ) {
					currentColumn += 1;
				}

				while( currentColumn < endColumn ) {
					ref var cell = ref map.GetCell( currentColumn, row );
					// Ensures that the cell hasn't already been initialized by
					// something other layer.
					if( cell.TerrainId == 0 ) {
						cell.TerrainId = terrain.Id;
					}
					currentColumn += 1;
				}
			}
		}

		private void ApplyEasyRiver( TerrainManager terrainManager, Map map, RiverTerrainOptions riverOptions ) {
			var startColumn = map.Columns / 3;
			var startDrift = map.Columns / 10;

			// The river falls one third of the way across the map, within a 
			// margin of +/- 10% of the width of the map
			startColumn += ( _random.Next( startDrift / 2 ) + ( startDrift / 2 ) );
			var currentColumn = startColumn;

			// Controls the length of a step in the drunken-walk.  This is the
			// minimum number of rows we go in a straight line before finding
			// a new column to occupy.
			var runLength = _random.Next( 3 ) + 2;

			var terrain = terrainManager.GetByIdName( riverOptions.Easy );
			for( int row = 0; row < map.Rows; row++ ) {
				for( int width = -1; width <= 1; width++ ) {
					ref var cell = ref map.GetCell( currentColumn + width, row );
					cell.TerrainId = terrain.Id;
				}
				runLength -= 1;

				if( runLength == 0 ) {
					runLength = _random.Next( 3 ) + 3;
					var delta = startColumn - currentColumn;
					var direction = _random.Next( 10 ) + delta;
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