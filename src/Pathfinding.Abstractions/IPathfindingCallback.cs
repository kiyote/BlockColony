using System;

namespace BlockColony.Core.Pathfinding {
	public interface IPathfindingCallback {
		void PathFound( Route route, int context );
	}
}
