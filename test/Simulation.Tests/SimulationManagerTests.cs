using System;
using NUnit.Framework;
using Mob;
using Work;
using Pathfinding;
using Surface;
using Work.Actions;

namespace Simulation.Tests {
	[TestFixture]
	public class SimulationManagerTests {

		/*
		private JobManager _jobManager;
		private ActorManager _actorManager;
		private SimulationManager _manager;
		private PathfindingManager _pathfindingManager;
		private IPathfinder _pathfinder;
		private Map _map;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			_map = new Map( 10, 10, new DefaultInitializer() );

			_pathfindingManager = new PathfindingManager();
			_actorManager = new ActorManager(_pathfindingManager);
			_jobManager = new JobManager();
			_manager = new SimulationManager(
				_jobManager,
				_actorManager,
				_map );
		}

		[Test]
		public void AssignJobs_OneActorOneJob_JobFound() {
			var job = new Job( Job.Medium, new Activity[] {
				new DigTileAction( ref _map.GetCell( 5, 5 ) )
				} );
			_jobManager.Add( job );

			_actorManager.Add( new Actor(
				column: 1,
				row: 1,
				locomotion: Locomotion.Walk
			) );

			//_manager.AssignJobs( _actorManager, _jobManager, _pathfinder );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
		}
		*/
	}
}
