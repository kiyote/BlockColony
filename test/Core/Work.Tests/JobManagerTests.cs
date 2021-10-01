using System.Threading;
using NUnit.Framework;
using BlockColony.Core.Pathfinding;
using BlockColony.Core.Surface;
using BlockColony.Core.Work.Steps;

namespace BlockColony.Core.Work.Tests {
#if DEBUG
	[TestFixture]
	public class JobManagerTests : IJobFitProvider, IMapProvider {
		private const int DELAY_MS = 500;
		private AutoResetEvent _gate;
		private JobManager _manager;
		private IMap _map;
		private IJobFit[] _fits;
		private IPathfindingManager _pathfindingManager;

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
			_manager = new JobManager( this, _pathfindingManager, this );
			_manager.Started += ( _, __ ) => {
				_gate.Set();
			};
			_manager.Start();
			_gate.WaitOne( DELAY_MS );
		}

		[TearDown]
		public void TearDown() {
			_manager.Stop();
			_gate.WaitOne( DELAY_MS );
		}

		[Test]
		public void Start_NotStarted_ThreadStarted() {
			using var gate = new AutoResetEvent( false );
			var manager = new JobManager( this, _pathfindingManager, this );
			int startCount = 0;
			manager.Started += ( _, __ ) => {
				startCount += 1;
				gate.Set();
			};

			manager.Start();

			gate.WaitOne( DELAY_MS );
			manager.Stop();
			Assert.That( startCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void Start_AlreadyStarted_NoEffect() {
			using var gate = new AutoResetEvent( false );
			var manager = new JobManager( this, _pathfindingManager, this );
			int startCount = 0;
			manager.Started += ( _, __ ) => {
				startCount += 1;
				gate.Set();
			};
			manager.Start();
			gate.WaitOne( DELAY_MS );

			manager.Start();
			gate.WaitOne( DELAY_MS );

			manager.Stop();

			Assert.That( startCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void Stop_NotStarted_NoEffect() {
			using var gate = new AutoResetEvent( false );
			var manager = new JobManager( this, _pathfindingManager, this );
			int stopCount = 0;
			manager.Started += ( _, __ ) => {
				gate.Set();
			};
			manager.Stopped += ( _, __ ) => {
				stopCount += 1;
				gate.Set();
			};
			manager.Start();
			gate.WaitOne( DELAY_MS );

			manager.Stop();
			gate.WaitOne( DELAY_MS );

			Assert.That( stopCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void Stop_AlreadyStarted_ThreadStopped() {
			using var gate = new AutoResetEvent( false );
			var manager = new JobManager( this, _pathfindingManager, this );
			int stopCount = 0;
			manager.Started += ( _, __ ) => {
				gate.Set();
			};
			manager.Stopped += ( _, __ ) => {
				stopCount += 1;
				gate.Set();
			};
			manager.Start();
			gate.WaitOne( DELAY_MS );
			manager.Stop();
			gate.WaitOne( DELAY_MS );

			manager.Stop();
			gate.WaitOne( DELAY_MS );

			Assert.That( stopCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void AddJob_OneFit_JobAssigned() {
			using var gate = new AutoResetEvent( false );
			var fit = new TestJobFit( gate );
			_fits = new IJobFit[1] {
				fit
			};

			var job = new Job(
				JobManager.Medium,
				new Activity[1] {
					new Activity(
						new ActivityStep[1] {
							new MoveToStep(_map.HalfColumns, _map.HalfRows)
						}
					)
				},
				0,
				JobState.Pending
			);

			_manager.AddJob( job );
			gate.WaitOne( DELAY_MS );

			Assert.AreSame( job, fit.Job );
		}

		[Test]
		public void AddJob_TwoFits_CloserFitChosen() {
			using var gate = new AutoResetEvent( false );
			var fit1 = new TestJobFit( gate );
			var fit2 = new TestJobFit( gate ) {
				LocationColumn = _map.HalfColumns - 1,
				LocationRow = _map.HalfRows - 1
			};
			_fits = new IJobFit[2] {
				fit1,
				fit2
			};

			var job = new Job(
				JobManager.Medium,
				new Activity[1] {
					new Activity(
						new ActivityStep[1] {
							new MoveToStep( _map.HalfColumns, _map.HalfRows )
						}
					)
				},
				0,
				JobState.Pending
			);

			_manager.AddJob( job );
			gate.WaitOne( DELAY_MS );

			Assert.IsNull( fit1.Job );
			Assert.AreSame( job, fit2.Job );
		}

		IJobFit[] IJobFitProvider.GetAvailable() {
			return _fits;
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

		private class TestJobFit : IJobFit {

			private readonly AutoResetEvent _gate;

			public TestJobFit( AutoResetEvent gate ) {
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
#endif
}
