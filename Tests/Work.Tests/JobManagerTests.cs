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
	public class JobManagerTests {
		private const int DELAY_MS = 500;
		private AutoResetEvent _gate;
		private JobManager _manager;

		[SetUp]
		public void SetUp() {
			_gate = new AutoResetEvent( false );
			_manager = new JobManager();
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
			/*
			_manager.GetPath( _map, ref _map.GetCell( 0, 0 ), ref _map.GetCell( _map.Columns - 1, _map.Rows - 1 ), Locomotion.Walk, _callback, 0 );
			_gate.WaitOne( DELAY_MS );

			Assert.That( _callback.CallbackCount, Is.EqualTo( 1 ) );
			*/
		}

		[Test]
		public void GetPath_ManagerNotStarted_ThrowsException() {
			/*
			var gate = new AutoResetEvent( false );
			var callback = new PathfindingCallback( gate );
			var manager = new JobManager();

			try {
				Assert.That( () => {
					manager.GetPath( _map, ref _map.GetCell( 0, 0 ), ref _map.GetCell( _map.Columns - 1, _map.Rows - 1 ), Locomotion.Walk, callback, 0 );
				}, Throws.InvalidOperationException );
			} finally {
				manager.Stop();
			}
			*/
		}

	}
}
