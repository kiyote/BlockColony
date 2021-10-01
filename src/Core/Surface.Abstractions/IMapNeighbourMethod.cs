using System;

namespace BlockColony.Core.Surface {
	public interface IMapNeighbourMethod {
		void Invoke( IMap map, ref MapCell source, ref MapCell cell, Directions direction );
	}
}
