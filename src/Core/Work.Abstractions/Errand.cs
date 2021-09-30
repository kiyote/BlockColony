using System;

namespace BlockColony.Core.Work {
	public enum Errand {
		Unknown,

		Idle,

		WaitingToPath,

		Pathing,

		MoveTo,

		Dig
	}
}
