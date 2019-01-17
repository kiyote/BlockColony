using System;
using System.Collections.Generic;
using System.Text;

namespace Work {
	public interface IJobFitProvider {
		IJobFit[] GetAvailable();
	}
}
