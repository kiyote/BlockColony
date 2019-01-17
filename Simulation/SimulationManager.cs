using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Work;
using Mob;
using System.Threading.Tasks;
using Pathfinding;
using Surface;

namespace Simulation
{
    public class SimulationManager
    {
		/*
		private Thread _thread;
		private AutoResetEvent _gate;
		private bool _terminated;
		private bool _isRunning;
		private ConcurrentQueue<Job> _addedJobs;
		private ConcurrentQueue<Actor> _addedActors;
		private JobManager _jobManager;
		private ActorManager _actorManager;
		private CancellationTokenSource _tokenSource;
		private CancellationToken _token;
		private readonly Map _map;
		private long _uiElapsedMilliseconds;

		public SimulationManager(
			JobManager jobManager,
			ActorManager actorManager,
			Map map
		) {
			_thread = new Thread( Run );
			_gate = new AutoResetEvent( false );
			_terminated = false;
			_addedJobs = new ConcurrentQueue<Job>();
			_jobManager = jobManager;
			_addedActors = new ConcurrentQueue<Actor>();
			_actorManager = actorManager;
			_map = map;

			_tokenSource = new CancellationTokenSource();
		}

		// These events run on the simulation thread, so use these at
		// your own peril!
		public event EventHandler Started;
		public event EventHandler Stopped;

		public void Start() {
			if (!_isRunning) {
				_token = _tokenSource.Token;
				_thread.Start();
			}
		}

		public void Stop() {
			if (_isRunning) {
				_tokenSource.Cancel();
				_terminated = true;
				_gate.Set();
				_thread.Join();
			}
		}

		public void AddJob(Job job) {
			_addedJobs.Enqueue( job );
			_gate.Set();
		}

		public void AddActor(Actor actor) {
			_addedActors.Enqueue( actor );
			_gate.Set();
		}

		public void UiUpdate(long elapsedMilliseconds) {
			Interlocked.Add( ref _uiElapsedMilliseconds, elapsedMilliseconds );
			_gate.Set();
		}

		private void Run() {
			_isRunning = true;
			Started?.Invoke(this, EventArgs.Empty);
			while (!_terminated) {

				while (_addedJobs.TryDequeue(out Job job)) {
					_jobManager.Add( job );
				}

				while (_addedActors.TryDequeue(out Actor actor)) {
					_actorManager.Add( actor );
				}

				long elapsedMilliseconds = Interlocked.Exchange( ref _uiElapsedMilliseconds, 0 );

				if (elapsedMilliseconds > 0) {
					_jobManager.SimulationUpdate();
					_actorManager.SimulationUpdate();
				}

				_gate.WaitOne();
			}
			_isRunning = false;
			Stopped?.Invoke(this, EventArgs.Empty);
		}

		internal void AssignJobs() {
			for (int priority = Job.Critical; priority < Job.PriorityCount; priority++ ) {
				var idle = actorManager.GetIdleActors();
				if ( !idle.Any() ) {
					break;
				}

				var jobs = jobManager.GetOpenJobs( priority );
				var tasks = jobs.SelectMany( job => idle.Select( async actor => {
					var fitness = GetFitness( job, actor );
					var step = job.Activity.First().Step.First();
					var path = await pathfinder.GetPathAsync( _map, actor.Column, actor.Row, step.Column, step.Row, Locomotion.Walk ).ConfigureAwait(false);
					fitness -= path.Count;
					return (Job: job, Actor: actor, Fitness: fitness);
				} ) );

				var final = await Task.WhenAll( tasks ).ConfigureAwait(false);
				var sorted = final.OrderBy( i => i.Job ).ThenBy( i => i.Fitness );
				foreach (var job in sorted.Select(s => s.Job)) {
					var first = sorted.First( s => s.Job == job );
					//TODO: Deal with this here
					Console.WriteLine( first.Actor.ToString() );
				}
			}
		}
	*/
    }
}
