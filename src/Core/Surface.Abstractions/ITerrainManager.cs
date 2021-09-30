using System.IO;

namespace BlockColony.Core.Surface {
	public interface ITerrainManager {
		ITerrain this[int id] { get; }

		ITerrain GetByIdName( string idName );
		void Load( string terrainFile );
		void Load( TextReader stream );
	}
}
