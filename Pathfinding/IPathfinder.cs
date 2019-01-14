using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surface;

namespace Pathfinding {
	public interface IPathfinder {
		List<int> GetPath( Map map, ref MapCell start, ref MapCell goal, Locomotion locomotion );

		Task<List<int>> GetPathAsync( Map map, int startColumn, int startRow, int goalColumn, int goalRow, Locomotion locomotion );
	}
}