using System;
using System.Collections.Generic;

namespace Pathfinding {
	// A route is a list of map cell indices.  That is, you can call
	// _map.GetCell( route[0] ) to get the cell that this step of the route
	// represents.
	public class Route: List<int> {
	}
}
