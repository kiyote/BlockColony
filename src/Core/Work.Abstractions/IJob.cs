using System;

namespace BlockColony.Core.Work {
	public interface IJob : IEquatable<IJob> {
		Activity[] Activities { get; }
		int Age { get; }
		int Priority { get; }
		JobState State { get; }

		void SetState( JobState state );
	}
}
