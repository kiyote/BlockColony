using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Surface {

	public class TerrainManager {
		private readonly Dictionary<int, Terrain> _terrain;

		public TerrainManager() {
			_terrain = new Dictionary<int, Terrain>();
		}

		public ITerrain this[ int id ] {
			get {
				return _terrain[ id ];
			}
		}

		public ITerrain GetByIdName( string idName ) {
			return _terrain.First( t => string.Equals( t.Value.IdName, idName, StringComparison.OrdinalIgnoreCase ) ).Value;
		}

		public void Load( TextReader stream ) {
			if (stream == default) {
				throw new ArgumentNullException( nameof( stream ) );
			}

			TerrainFile terrainConfig = JsonConvert.DeserializeObject<TerrainFile>( stream.ReadToEnd() );

			foreach( TerrainFile.TerrainConfig entry in terrainConfig.Terrain ) {
				TerrainFile.PhaseConfig phase = terrainConfig.Phase.Where( p => string.Equals( p.Name, entry.Phase, StringComparison.OrdinalIgnoreCase ) ).First();
				var terrain = new Terrain( entry.Id, entry.IdName, phase.Phases.Select( p => {
					return (transition: p.Transition, attribute: p as ITerrainAttributes);
				} ) );

				_terrain[ terrain.Id ] = terrain;
			}
		}

		public void Load( string terrainFile ) {
			if( !File.Exists( terrainFile ) ) {
				throw new ArgumentException( terrainFile );
			}

			using( var reader = new StreamReader( terrainFile ) ) {
				Load( reader );
			}
		}
	}
}
