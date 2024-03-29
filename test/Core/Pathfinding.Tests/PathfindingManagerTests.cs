using System.Threading;
using BlockColony.Core.Surface;
using NUnit.Framework;

namespace BlockColony.Core.Pathfinding.Tests {
#if DEBUG
	[TestFixture]
	public class PathfindingManagerTests {
		private const int DELAY_MS = 500;
		private IMap _map;
		private AutoResetEvent _gate;
		private PathfindingCallback _callback;
		private IPathfindingManager _manager;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			IMapFactory factory = new MapFactory();

			_map = factory.Create( 10, 10, DefaultInitializer );
		}

		[SetUp]
		public void SetUp() {
			_gate = new AutoResetEvent( false );
			_callback = new PathfindingCallback( _gate );
			_manager = new PathfindingManager();
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
			_gate.Dispose();
		}

		[Test]
		public void Start_NotStarted_ThreadStarted() {
			using var gate = new AutoResetEvent( false );
			IPathfindingManager manager = new PathfindingManager();
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
			IPathfindingManager manager = new PathfindingManager();
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
			IPathfindingManager manager = new PathfindingManager();
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
			IPathfindingManager manager = new PathfindingManager();
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
		public void GetPath_ValidPath_CallbackReceived() {
			_manager.GetPath( _map, ref _map.GetCell( 0, 0 ), ref _map.GetCell( _map.Columns - 1, _map.Rows - 1 ), Locomotion.Walk, _callback, 0 );
			_gate.WaitOne( DELAY_MS );

			Assert.That( _callback.CallbackCount, Is.EqualTo( 1 ) );
		}

		[Test]
		public void GetPath_ManagerNotStarted_ThrowsException() {
			using var gate = new AutoResetEvent( false );
			var callback = new PathfindingCallback( gate );
			IPathfindingManager manager = new PathfindingManager();

			try {
				Assert.That( () => {
					manager.GetPath( _map, ref _map.GetCell( 0, 0 ), ref _map.GetCell( _map.Columns - 1, _map.Rows - 1 ), Locomotion.Walk, callback, 0 );
				}, Throws.InvalidOperationException );
			} finally {
				manager.Stop();
			}
		}

		private void DefaultInitializer( ref MapCell cell ) {
			cell.TerrainCost = 100;
			cell.Walkability = (byte)Directions.All;
		}

		private class PathfindingCallback : IPathfindingCallback {

			private readonly AutoResetEvent _event;
			private Route _path;

			public PathfindingCallback( AutoResetEvent arEvent ) {
				_event = arEvent;
				CallbackCount = 0;
			}

			public int CallbackCount { get; set; }

			public Route Path {
				get {
					return _path;
				}
			}

			public void PathFound( Route path, int context ) {
				Interlocked.Exchange( ref _path, path );
				CallbackCount += 1;
				_event.Set();
			}
		}
	}
#endif
}
