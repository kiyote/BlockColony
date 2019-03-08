using System;
using System.Threading;
using NUnit.Framework;
using Pathfinding;
using Surface;
using Work;

namespace Mob.Tests {
	[TestFixture]
	public class ActorTests {

		private Actor _actor;
		private Map _map;
		private JobManager _jobManager;
		private ActorManager _actorManager;
		private PathfindingManager _pathfindingManager;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			const int Rows = 3;
			const int Columns = 3;
			_map = new Map( Columns, Rows, new DefaultInitializer() );

			_pathfindingManager = new PathfindingManager();
			_pathfindingManager.Start();

			_actorManager = new ActorManager( _pathfindingManager );
			_jobManager = new JobManager( _actorManager, _pathfindingManager );
		}

		[OneTimeTearDown]
		public void OneTimeTearDown() {
			_pathfindingManager.Stop();
		}

		[SetUp]
		public void SetUp() {
			_jobManager.Start( _map );

			_actor = new Actor( 0, 0, Locomotion.Walk );
			_actorManager.Add( _actor );
		}

		[TearDown]
		public void TearDown() {
			_jobManager.Stop();
		}

		[Test]
		public void SimulationUpdate_HasJob_PicksUpJob() {
			var gate = new AutoResetEvent( false );
			_actor.JobAssigned += ( _, __ ) => {
				gate.Reset();
			};

			var steps = new Step[ 1 ] {
				new Step(Errand.Dig, 2, 2)
			};
			var activities = new Activity[ 1 ] {
				new Activity(steps)
			};
			var job = new Job( Job.Medium, activities );
			_jobManager.AddJob( job );
			Assert.IsTrue( gate.WaitOne( 500 ) );

			_actorManager.SimulationUpdate();
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
		}
	}
}
