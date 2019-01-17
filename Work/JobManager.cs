using System;
using System.Collections.Generic;
using Surface;
using System.Threading;
using System.Collections.Concurrent;

namespace Work {
	public class JobManager {
		private readonly List<Job>[] _jobs;
		private Thread _thread;
		private AutoResetEvent _gate;
		private readonly ConcurrentQueue<Job> _pendingJobs;
		private readonly IJobFitProvider _jobFitProvider;
		private bool _terminated;
		private bool _isRunning;

		public JobManager(
			IJobFitProvider jobFitProvider
		) {
			_jobFitProvider = jobFitProvider;
			_jobs = new List<Job>[ Job.PriorityCount ];
			for( int i = 0; i < _jobs.Length; i++ ) {
				_jobs[ i ] = new List<Job>();
			}

			_gate = new AutoResetEvent( false );
			_isRunning = false;
			_pendingJobs = new ConcurrentQueue<Job>();
		}

		// These run on the Pathfinding thread, so use them at your own risk
		public event EventHandler Started;
		public event EventHandler Stopped;

		public void AddJob( Job job ) {
			if( !_isRunning ) {
				throw new InvalidOperationException( "Attempt to perform job with stopped job manager." );
			}
			_pendingJobs.Enqueue( job );
			_gate.Set();
		}

		public void Start() {
			if( !_isRunning ) {
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
			Started?.Invoke( this, EventArgs.Empty );
			while( !_terminated ) {

				Job job = GetNextJob();
				while( job != default( Job ) ) {
					var fits = _jobFitProvider.GetAvailable();
					var handler = FindHandler( job, fits );

					if (!_terminated) {
						job = GetNextJob();
					} else {
						job = default( Job );
					}
				}

				_gate.WaitOne( 1000 ); // Automatically unlock after 1s just in case
			}
			_isRunning = false;
			Stopped?.Invoke( this, EventArgs.Empty );
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

		public IJobFit FindHandler( Job job, IJobFit[] fits ) {
			int fitness = int.MinValue;
			IJobFit result = default( IJobFit );
			for( int i = 0; i < fits.Length; i++ ) {
				var testFitness = fits[ i ].GetFitness( job );
				if( testFitness > fitness ) {
					fitness = testFitness;
					result = fits[ i ];
				}
			}

			return result;
		}

	}
}