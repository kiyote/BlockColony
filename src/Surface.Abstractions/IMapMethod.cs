using System;

namespace BlockColony.Core.Surface {
	public interface IMapMethod {
		void Invoke( ref MapCell cell );
	}
}
