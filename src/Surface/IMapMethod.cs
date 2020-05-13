using System;

namespace Surface {
	public interface IMapMethod {
		void Invoke( ref MapCell cell );
	}
}
