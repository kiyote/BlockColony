using System;
using System.Threading;
using NUnit.Framework;
using BlockColony.Core.Pathfinding;
using BlockColony.Core.Surface;
using BlockColony.Core.Work;
using BlockColony.Core.Work.Steps;

namespace BlockColony.Core.Mob.Tests {
	[TestFixture]
	public class ActorTests : IMapProvider {

		private Actor _actor;
		private IMap _map;
		private JobManager _jobManager;
		private ActorManager _actorManager;
		private IPathfindingManager _pathfindingManager;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			const int Rows = 3;
			const int Columns = 3;
			_map = new Map( Columns, Rows, new DefaultInitializer() );
		}

		[OneTimeTearDown]
		public void OneTimeTearDown() {
		}

		[SetUp]
		public void SetUp() {
			_pathfindingManager = new PathfindingManager();
			_pathfindingManager.Start();

			_actorManager = new ActorManager( _pathfindingManager );
			_jobManager = new JobManager( _actorManager, _pathfindingManager, this );
			_jobManager.Start();

			_actor = new Actor( 0, 0, Locomotion.Walk );
			_actorManager.Add( _actor );
		}

		[TearDown]
		public void TearDown() {
			_actorManager.Remove( _actor );
			_jobManager.Stop();
			_pathfindingManager.Stop();
		}

#if DEBUG
		[Test]
		public void SimulationUpdate_HasJob_PicksUpJob() {
			using var gate = new AutoResetEvent( false );
			_actor.JobAssigned += ( _, __ ) => {
				gate.Set();
			};
			_actor.PathAssigned += ( _, __ ) => {
				gate.Set();
			};

			_jobManager.AddJob( CreateJob() );
			Assert.IsTrue( gate.WaitOne( 500 ) ); // Wait for the job to be assigned
			gate.Reset();

			_actorManager.SimulationUpdate( _map );

			Assert.IsTrue( gate.WaitOne( 500 ) ); // Wait for the path to be assigned
			gate.Reset();

			_actorManager.SimulationUpdate( _map );
			Assert.AreNotEqual( -1, _actor.GetDesiredRouteStep() );
		}

		[Test]
		public void RouteStepComplete_WalkingPath_DigWhenDone() {
			using var gate = new AutoResetEvent( false );
			_actor.JobAssigned += ( _, __ ) => {
				gate.Set();
			};
			_actor.PathAssigned += ( _, __ ) => {
				gate.Set();
			};

			_jobManager.AddJob( CreateJob() );
			Assert.IsTrue( gate.WaitOne( 500 ) ); // Wait for the job to be assigned
			gate.Reset();
			_actorManager.SimulationUpdate( _map );
			Assert.IsTrue( gate.WaitOne( 500 ) ); // Wait for the path to be assigned
			gate.Reset();
			_actorManager.SimulationUpdate( _map );

			while( _actor.GetDesiredRouteStep() != -1 ) {
				_actor.RouteStepComplete( ref _map.GetCell( _actor.GetDesiredRouteStep() ) );
			}

			Assert.AreEqual( Errand.Dig, _actor.Errand );
		}
#endif

		[Test]
		public void ErrandComplete_LastErrand_BecomesIdle() {
			var fit = _actor as IJobFit;
			fit.AssignJob( CreateJob( _actor.Column, _actor.Row ) );
			_actor.SimulationUpdate();

			Assert.AreEqual( Errand.Dig, _actor.Errand );

			_actor.ErrandComplete();

			Assert.AreEqual( Errand.Idle, _actor.Errand );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Invoke( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Directions.All;
			}
		}

		IMap IMapProvider.Current() {
			return _map;
		}

		private static Job CreateJob( int column = 2, int row = 2 ) {
			var steps = new ActivityStep[1] {
				new DigStep(column, row)
			};
			var activities = new Activity[1] {
				new Activity(steps)
			};
			var job = new Job(
				JobManager.Medium,
				activities,
				0,
				JobState.Pending
			);

			return job;
		}
	}
}
