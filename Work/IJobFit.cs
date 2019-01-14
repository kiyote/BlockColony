using System;
using System.Threading.Tasks;

namespace Work {

	public interface IJobFit {
		Task<int> GetFitnessAsync( Job job );
	}
}
