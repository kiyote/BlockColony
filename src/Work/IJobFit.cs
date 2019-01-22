using System;
using System.Threading.Tasks;
using Surface;

namespace Work {

	public interface IJobFit {
		int LocationColumn { get; }

		int LocationRow { get; }

		Locomotion Locomotion { get; }

		Job AssignJob( Job job );
	}
}
