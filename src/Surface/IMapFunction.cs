using System;

namespace Surface {
	public interface IMapFunction<T> {
		T Invoke( ref MapCell cell );
	}
}
