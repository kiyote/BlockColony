using System;
using System.Linq;
using System.Collections.Generic;
using Pathfinding;
using Mob;
using Surface;

namespace Work {
	public class JobManager {
		private readonly List<Job>[] _jobs;
		private readonly PathfindingManager _pathfindingManager;

		public JobManager(
			PathfindingManager pathfindingManager
		) {
			_pathfindingManager = pathfindingManager;
			_jobs = new List<Job>[ Job.PriorityCount ];
			for( int i = 0; i < _jobs.Length; i++ ) {
				_jobs[ i ] = new List<Job>();
			}
		}

		public void Add( Job job ) {
			_jobs[ job.Priority ].Add( job );
		}

		public void SimulationUpdate() {
		}

		public List<Job> GetOpenJobs( int priority ) {
			return _jobs[ priority ];
		}

		public void FindHandler( Job job ) {

		}
	}
}