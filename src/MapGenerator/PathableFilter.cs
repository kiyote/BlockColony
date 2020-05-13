using System;
using Surface;

namespace MapGenerator {
	public sealed class PathableFilter : IMapFunction<bool> {

		private readonly TerrainManager _manager;

		public PathableFilter( TerrainManager terrainManager ) {
			_manager = terrainManager ?? throw new ArgumentNullException( nameof(terrainManager) );
		}

		bool IMapFunction<bool>.Invoke( ref MapCell cell ) {
			ITerrainAttributes terrain = _manager[ cell.TerrainId ][ cell.Temperature ];
			return terrain.Pathable;
		}
	}
}
