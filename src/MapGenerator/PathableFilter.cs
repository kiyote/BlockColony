using System;
using Surface;

namespace MapGenerator {
	public class PathableFilter : IMapFunction<bool> {

		private readonly TerrainManager _manager;

		public PathableFilter( TerrainManager terrainManager ) {
			_manager = terrainManager ?? throw new ArgumentNullException();
		}

		bool IMapFunction<bool>.Do( ref MapCell cell ) {
			ITerrainAttributes terrain = _manager[ cell.TerrainId ][ cell.Temperature ];
			return terrain.Pathable;
		}
	}
}
