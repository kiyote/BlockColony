using System;
using System.Collections.Generic;
using Surface;
using System.Threading;
using System.Collections.Concurrent;
using Pathfinding;

namespace Work {
	public class JobManager : IPathfindingCallback {
		public const int INITIAL_MAXIMUM = 100;

		private readonly List<Job>[] _jobs;
		private Thread _thread;
		private AutoResetEvent _gate;
		private readonly ConcurrentQueue<Job> _pendingJobs;
		private readonly IJobFitProvider _jobFitProvider;
		private bool _terminated;
		private bool _isRunning;
		private Map _map;
		private PathfindingManager _pathfindingManager;

		private Route[] _foundRoutes;
		private int[] _fitness;
		private int _pendingRoutes;

		public JobManager(
			IJobFitProvider jobFitProvider,
			PathfindingManager pathfindingManager
		) {
			_jobFitProvider = jobFitProvider;
			_jobs = new List<Job>[ Job.PriorityCount ];
			for( int i = 0; i < _jobs.Length; i++ ) {
				_jobs[ i ] = new List<Job>();
			}

			_gate = new AutoResetEvent( false );
			_isRunning = false;
			_pendingJobs = new ConcurrentQueue<Job>();
			_pathfindingManager = pathfindingManager;

			_foundRoutes = new Route[ INITIAL_MAXIMUM ];
			_fitness = new int[ INITIAL_MAXIMUM ];
		}

#if DEBUG
		public event EventHandler Started;
		public event EventHandler Stopped;
#endif 

		public void AddJob( Job job ) {
			if( !_isRunning ) {
				throw new InvalidOperationException( "Attempt to perform job with stopped job manager." );
			}
			_pendingJobs.Enqueue( job );
			_gate.Set();
		}

		public void Start( Map map ) {
			if( !_isRunning ) {
				_map = map;
				_thread = new Thread( Run ) {
					Name = "Job Thread"
				};
				_thread.Start();
			}
		}

		public void Stop() {
			if( _isRunning ) {
				_terminated = true;
				_gate.Set();
				_thread.Join();
			}
		}

		private void Run() {
			_isRunning = true;
#if DEBUG
			Started?.Invoke( this, EventArgs.Empty );
#endif
			while( !_terminated ) {

				Job job = GetNextJob();
				while( job != default( Job ) ) {
					// All the people available to handle the job
					var fits = _jobFitProvider.GetAvailable();
					// Of those, the people who could possibly handle the job
					// (ie - has the right skill)
					fits = FilterSuitable( job, fits );

					if (fits.Length == 0) {
						// No one available to handle the job
						// Re-queue it and leave
						AddJob( job );
						break;
					} 

					// Now we calculate the distance to the job for everyone
					// left, blocking until the pathing is complete
					StartPathing( job, fits );
					// Non-path fitness is "suitability" for the job.  ie - Those 
					// with higher skills are more fit than those with lower skills.
					CalculateNonPathFit( job, fits );
					while( !PathingComplete() && !_terminated ) {
						_gate.WaitOne();
					}

					IJobFit handler = fits[ FindJobFit( fits ) ];

					if( handler == default( IJobFit ) ) {
						// We couldn't find a handler for this job so we should
						// put it in the blocked jobs to be re-queued periodically
						// TODO: create a blocked job queue
					} else {
						Job oldJob = handler.AssignJob( job );
						if( oldJob != default( Job ) ) {
							// We assigned them a new job before they started the
							// old one, so requeue this one
							AddJob( oldJob );
						}
					}

					if( !_terminated ) {
						ClearFoundRoutes();
						job = GetNextJob();
					} else {
						job = default( Job );
					}
				}

				if( !_terminated ) {
					_gate.WaitOne( 1000 ); // Automatically unlock after 1s just in case
				}
			}
			_isRunning = false;
#if DEBUG
			Stopped?.Invoke( this, EventArgs.Empty );
#endif
		}

		// Determines the non-path fitness vs the pathing fitness so that
		// you're more likely to be picked for the job the closer you are,
		// weighted towards people of higher skill at equal distances being
		// chosen.
		private int FindJobFit( IJobFit[] fits ) {
			int result = -1;
			int bestFit = int.MinValue;
			for (int i = 0; i < fits.Length; i++ ) {
				if ((_fitness[i] - _foundRoutes[i].Count) > bestFit) {
					bestFit = (_fitness[ i ] - _foundRoutes[ i ].Count);
					result = i;
				}
			}

			return result;
		}

		private void CalculateNonPathFit( Job job, IJobFit[] fits ) {
			for( int i = 0; i < fits.Length; i++ ) {
				_fitness[ i ] = 100; // TODO: actually calculate a fitness score
			}
		}

		private bool PathingComplete() {
			for( int i = 0; i < _pendingRoutes; i++ ) {
				if( _foundRoutes[ i ] == default( Route ) ) {
					return false;
				}
			}

			return true;
		}

		// First pass to see if any of the supplied IJobFit aren't even
		// able to take on the specified job.  This way we don't do any work
		// that would be discarded anyway.
		private IJobFit[] FilterSuitable( Job job, IJobFit[] fits ) {
			return fits;  // TODO: For now every fit is applicable
		}

		private void ClearFoundRoutes() {
			for( int i = 0; i < _pendingRoutes; i++ ) {
				_foundRoutes[ i ] = default( Route );
				_fitness[ i ] = 0;
			}
		}

		private Job GetNextJob() {
			while( _pendingJobs.TryDequeue( out Job newJob ) ) {
				_jobs[ newJob.Priority ].Add( newJob );
			}

			Job result = default( Job );
			for( int i = 0; i < Job.PriorityCount; i++ ) {
				if( _jobs[ i ].Count > 0 ) {
					result = _jobs[ i ][ 0 ];
					_jobs[ i ].Remove( result );
				}
			}

			return result;
		}

		private void StartPathing( Job job, IJobFit[] fits ) {

			if( fits.Length > _foundRoutes.Length ) {
				_foundRoutes = new Route[ fits.Length ];
				_fitness = new int[ fits.Length ];
			}
			_pendingRoutes = fits.Length;

			for( int i = 0; i < fits.Length; i++ ) {
				var fit = fits[ i ];
				var activity = job.Activity[ 0 ];
				var step = activity.Step[ 0 ];
				ref var location = ref _map.GetCell( fit.LocationColumn, fit.LocationRow );
				ref var target = ref _map.GetCell( step.Column, step.Row );
				_pathfindingManager.GetPath( _map, ref location, ref target, fit.Locomotion, this, i );
			}
		}

		void IPathfindingCallback.PathFound( Route route, int context ) {
			Interlocked.Exchange( ref _foundRoutes[ context ], route );
			_gate.Set();
		}
	}
}