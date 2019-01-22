using System;
using System.Collections.Generic;
using System.Text;

namespace Mob {

	public class MovementEventArgs: EventArgs {
		public int Column;
		public int Row;
		public int Milliseconds;
		public IMovementCallback callback;
	}

	public interface IMovementCallback {
		void MovementComplete();
	}
}
