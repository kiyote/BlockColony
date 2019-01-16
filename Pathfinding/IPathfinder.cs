using System;
using System.Threading.Tasks;
using Surface;

namespace Pathfinding {
	public interface IPathfinder {
		Route GetPath( Map map, ref MapCell start, ref MapCell goal, Locomotion locomotion );

		Task<Route> GetPathAsync( Map map, int startColumn, int startRow, int goalColumn, int goalRow, Locomotion locomotion );
	}
}