using System;
using System.Threading.Tasks;

namespace Work {

	public interface IJobFit {
		int GetFitness( Job job );
	}
}
