using System;
using NUnit.Framework;
using Mob;
using Work;
using Pathfinding;
using Pathfinding.AStar;
using Surface;
using Work.Actions;

namespace Simulation.Tests {
	[TestFixture]
	public class SimulationManagerTests {

		private JobManager _jobManager;
		private ActorManager _actorManager;
		private SimulationManager _manager;
		private IPathfinder _pathfinder;
		private Map _map;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			_map = new Map( 10, 10, new DefaultInitializer() );

			_jobManager = new JobManager();

			_actorManager = new ActorManager();

			_pathfinder = new AStarPathfinder();
			_manager = new SimulationManager(
				_jobManager,
				_actorManager,
				_pathfinder,
				_map );
		}

		[Test]
		public void AssignJobs_OneActorOneJob_JobFound() {
			var job = new Job( Job.Medium, new Work.Activity[] {
				new DigTileAction( ref _map.GetCell( 5, 5 ) )
				} );
			_jobManager.Add( job );

			_actorManager.Add( new Actor() {
				Column = 1,
				Row = 1
			} );

			_manager.AssignJobs( _actorManager, _jobManager, _pathfinder );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
		}
	}
}
