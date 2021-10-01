using BlockColony.Core.Surface;

namespace BlockColony.Core.Work {

	public interface IJobFit {
		int LocationColumn { get; }

		int LocationRow { get; }

		Locomotion Locomotion { get; }

		IJob? AssignJob( IJob job );
	}
}
