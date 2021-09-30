using System.Threading.Tasks;
using BlockColony.Core.Surface;

namespace BlockColony.Core.Pathfinding {
	public interface IPathfinder {
		Route GetPath( IMap map, ref MapCell start, ref MapCell goal, Locomotion locomotion );

		Task<Route> GetPathAsync( IMap map, int startColumn, int startRow, int goalColumn, int goalRow, Locomotion locomotion );
	}
}
