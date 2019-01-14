using System;
using System.Collections.Generic;
using System.Text;

namespace Pathfinding {
	public interface IPathfindingCallback {
		void PathFound( List<int> path );
	}
}
