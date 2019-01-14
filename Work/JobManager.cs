using System;
using System.Linq;
using System.Collections.Generic;

namespace Work
{
    public class JobManager
    {
		private List<Job>[] _jobs;

		public JobManager() {
			_jobs = new List<Job>[Job.PriorityCount];
			for (int i = 0; i < _jobs.Length; i++) {
				_jobs[i] = new List<Job>();
			}
		}

		public void Add(Job job) {
			_jobs[job.Priority].Add( job );
		}

		public void Update() {
		}

		public List<Job> GetOpenJobs(int priority) {
			return _jobs[priority];
		}
    }
}
