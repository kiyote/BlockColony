using System;
using System.Collections.Generic;
using System.Threading;
using Surface;
using NUnit.Framework;

namespace Pathfinding.Tests {
	[TestFixture]
	public class PathfindingManagerTests {
		private const int DELAY_MS = 500;
		private Map _map;
		private DefaultInitializer _defaultInitializer;
		private AutoResetEvent _gate;
		private PathfindingCallback _callback;
		private PathfindingManager _manager;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			_defaultInitializer = new DefaultInitializer();
			_map = new Map( 10, 10, _defaultInitializer );
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
		}

		[Test]
		public void Start_NotStarted_ThreadStarted() {
			var gate = new AutoResetEvent( false );
			var manager = new PathfindingManager();
			var startCount = 0;
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
			var gate = new AutoResetEvent( false );
			var manager = new PathfindingManager();
			var startCount = 0;
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
			var gate = new AutoResetEvent( false );
			var manager = new PathfindingManager();
			var stopCount = 0;
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
			var gate = new AutoResetEvent( false );
			var manager = new PathfindingManager();
			var stopCount = 0;
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
			var gate = new AutoResetEvent( false );
			var callback = new PathfindingCallback( gate );
			var manager = new PathfindingManager();

			try {
				Assert.That( () => {
					manager.GetPath( _map, ref _map.GetCell( 0, 0 ), ref _map.GetCell( _map.Columns - 1, _map.Rows - 1 ), Locomotion.Walk, callback, 0 );
				}, Throws.InvalidOperationException );
			} finally {
				manager.Stop();
			}
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
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
}