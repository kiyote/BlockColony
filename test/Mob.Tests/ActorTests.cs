﻿using System;
using System.Threading;
using NUnit.Framework;
using Pathfinding;
using Surface;
using Work;

namespace Mob.Tests {
	[TestFixture]
	public class ActorTests : IMapProvider {

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
			_jobManager = new JobManager( _actorManager, _pathfindingManager, this );
		}

		[OneTimeTearDown]
		public void OneTimeTearDown() {
			_pathfindingManager.Stop();
		}

		[SetUp]
		public void SetUp() {
			_jobManager.Start();

			_actor = new Actor( 0, 0, Locomotion.Walk );
			_actorManager.Add( _actor );
		}

		[TearDown]
		public void TearDown() {
			_jobManager.Stop();
		}

#if DEBUG
		[Test]
		public void SimulationUpdate_HasJob_PicksUpJob() {
			var gate = new AutoResetEvent( false );
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
			var gate = new AutoResetEvent( false );
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
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
		}

		Map IMapProvider.Get() {
			return _map;
		}

		private Job CreateJob( int column = 2, int row = 2 ) {
			var steps = new Step[ 1 ] {
				new Step(Errand.Dig, column, row)
			};
			var activities = new Activity[ 1 ] {
				new Activity(steps)
			};
			var job = new Job( Job.Medium, activities );

			return job;
		}
	}
}
