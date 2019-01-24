using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Pathfinding;
using Surface;
using Work.Actions;

namespace Work.Tests {
	[TestFixture]
	public class JobManagerTests : IJobFitProvider {
		private const int DELAY_MS = 500;
		private AutoResetEvent _gate;
		private JobManager _manager;
		private Map _map;
		private IJobFit[] _fits;
		private PathfindingManager _pathfindingManager;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			_map = new Map( 10, 10, new DefaultInitializer() );
			_pathfindingManager = new PathfindingManager();
			_pathfindingManager.Start();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown() {
			_pathfindingManager.Stop();
		}


		[SetUp]
		public void SetUp() {
			_gate = new AutoResetEvent( false );
			_manager = new JobManager( this, _pathfindingManager );
			_manager.Started += ( _, __ ) => {
				_gate.Set();
			};
			_manager.Start( _map );
			_gate.WaitOne( DELAY_MS );
		}

		[TearDown]
		public void TearDown() {
			_manager.Stop();
			_gate.WaitOne( DELAY_MS );
		}

		[Test]
		public void Start_NotStarted_ThreadStarted() {
			var gate = new AutoResetEvent( false );
			var manager = new JobManager( this, _pathfindingManager );
			var startCount = 0;
			manager.Started += ( _, __ ) => {
				startCount += 1;
				gate.Set();
			};

			manager.Start( _map );

			gate.WaitOne( DELAY_MS );
			manager.Stop();

			Assert.That( startCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void Start_AlreadyStarted_NoEffect() {
			var gate = new AutoResetEvent( false );
			var manager = new JobManager( this, _pathfindingManager );
			var startCount = 0;
			manager.Started += ( _, __ ) => {
				startCount += 1;
				gate.Set();
			};
			manager.Start( _map );
			gate.WaitOne( DELAY_MS );

			manager.Start( _map );
			gate.WaitOne( DELAY_MS );

			manager.Stop();

			Assert.That( startCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void Stop_NotStarted_NoEffect() {
			var gate = new AutoResetEvent( false );
			var manager = new JobManager( this, _pathfindingManager );
			var stopCount = 0;
			manager.Started += ( _, __ ) => {
				gate.Set();
			};
			manager.Stopped += ( _, __ ) => {
				stopCount += 1;
				gate.Set();
			};
			manager.Start( _map );
			gate.WaitOne( DELAY_MS );

			manager.Stop();
			gate.WaitOne( DELAY_MS );

			Assert.That( stopCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void Stop_AlreadyStarted_ThreadStopped() {
			var gate = new AutoResetEvent( false );
			var manager = new JobManager( this, _pathfindingManager );
			var stopCount = 0;
			manager.Started += ( _, __ ) => {
				gate.Set();
			};
			manager.Stopped += ( _, __ ) => {
				stopCount += 1;
				gate.Set();
			};
			manager.Start( _map );
			gate.WaitOne( DELAY_MS );
			manager.Stop();
			gate.WaitOne( DELAY_MS );

			manager.Stop();
			gate.WaitOne( DELAY_MS );

			Assert.That( stopCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void AddJob_OneFit_JobAssigned() {
			var gate = new AutoResetEvent( false );

			var fit = new TestJobFit( gate );
			_fits = new IJobFit[ 1 ] {
				fit
			};

			var job = new Job( Job.Medium, new Activity[ 1 ] {
				new Activity(
					new Step[1] {
						new Step(Errand.MoveTo, _map.HalfColumns, _map.HalfRows)
					}
				)
			} );

			_manager.AddJob( job );
			gate.WaitOne( DELAY_MS );

			Assert.AreSame( job, fit.Job );
		}

		[Test]
		public void AddJob_TwoFits_CloserFitChosen() {
			var gate = new AutoResetEvent( false );

			var fit1 = new TestJobFit( gate );
			var fit2 = new TestJobFit( gate );
			fit2.LocationColumn = _map.HalfColumns - 1;
			fit2.LocationRow = _map.HalfRows - 1;
			_fits = new IJobFit[ 2 ] {
				fit1,
				fit2
			};

			var job = new Job( Job.Medium, new Activity[ 1 ] {
				new Activity(
					new Step[1] {
						new Step(Errand.MoveTo, _map.HalfColumns, _map.HalfRows)
					}
				)
			} );

			_manager.AddJob( job );
			gate.WaitOne( DELAY_MS );

			Assert.IsNull( fit1.Job );
			Assert.AreSame( job, fit2.Job );
		}

		IJobFit[] IJobFitProvider.GetAvailable() {
			return _fits;
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
		}

		private class TestJobFit : IJobFit {

			private AutoResetEvent _gate;

			public TestJobFit(AutoResetEvent gate) {
				_gate = gate;
			}

			public int LocationColumn { get; set; }

			public int LocationRow { get; set; }

			public Locomotion Locomotion { get; set; }

			public Job AssignJob( Job job ) {
				Job = job;
				_gate.Set();
				return null;
			}

			public Job Job { get; set; }
		}
	}
}