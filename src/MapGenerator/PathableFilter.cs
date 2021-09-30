using System;
using BlockColony.Core.Surface;

namespace BlockColony.Core.MapGenerator {
	public sealed class PathableFilter : IMapFunction<bool> {

		private readonly ITerrainManager _manager;

		public PathableFilter( ITerrainManager terrainManager ) {
			_manager = terrainManager ?? throw new ArgumentNullException( nameof(terrainManager) );
		}

		bool IMapFunction<bool>.Invoke( ref MapCell cell ) {
			ITerrainAttributes terrain = _manager[ cell.TerrainId ][ cell.Temperature ];
			return terrain.Pathable;
		}
	}
}
