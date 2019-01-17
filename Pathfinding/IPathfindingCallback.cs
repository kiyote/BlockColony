using System;

namespace Pathfinding {
	public interface IPathfindingCallback {
		void PathFound( Route route, int context );
	}
}
