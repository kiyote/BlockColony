using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pathfinding;
using Surface;
using Work.Actions;

namespace Work.Tests {
	[TestFixture]
	public class JobManagerTests {

		private PathfindingManager _pathfindingManager;
		private JobManager _manager;
		private Map _map;

		[SetUp]
		public void SetUp() {
			_map = new Map( 10, 10, new DefaultInitializer() );
			_pathfindingManager = new PathfindingManager();
			_manager = new JobManager( _pathfindingManager );
		}

		[TearDown]
		public void TearDown() {

		}

		[Test]
		public void AssignJobs_OneActorOneJob_JobFound() {
			var job = new Job( Job.Medium, new Activity[] {
				new DigTileAction( ref _map.GetCell( 5, 5 ) )
				} );
			_manager.Add( job );

			/*
			_actorManager.Add( new Actor(
				column: 1,
				row: 1,
				locomotion: Locomotion.Walk
			) );
			*/

			//_manager.AssignJobs( _actorManager, _jobManager, _pathfinder );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
		}
	}
}
