using System;

namespace BlockColony.Core.Surface {
	public interface IMapNeighbourMethod {
		void Invoke( ref MapCell source, ref MapCell cell, Directions direction );
	}
}
