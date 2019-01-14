using System;

namespace Surface {
	public interface IMapNeighbourMethod {
		void Do( ref MapCell source, ref MapCell cell, Direction direction );
	}
}