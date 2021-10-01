using System;
using BlockColony.Core.Surface;

namespace BlockColony.Core.MapGenerator {
	public interface IMapGenerator {

		void Build(
			MapGeneratorOptions options,
			Action<IMap> mapCompleted
		);
	}
}
