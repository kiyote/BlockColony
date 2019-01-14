using System;


namespace Surface {
	public interface IMapFunction<T> {
		T Do( ref MapCell cell );
	}
}