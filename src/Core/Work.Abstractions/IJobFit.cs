using BlockColony.Core.Surface;

namespace BlockColony.Core.Work {

	public interface IJobFit {
		int LocationColumn { get; }

		int LocationRow { get; }

		Locomotion Locomotion { get; }

		Job? AssignJob( Job job );
	}
}
