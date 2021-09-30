using System;

namespace BlockColony.Core.Mob {

	public sealed class MovementEventArgs : EventArgs {
		public int Column { get; set; }
		public int Row { get; set; }
		public int Milliseconds { get; set; }
		public IMovementCallback Callback { get; set; }
	}

	public interface IMovementCallback {
		void MovementComplete();
	}
}
