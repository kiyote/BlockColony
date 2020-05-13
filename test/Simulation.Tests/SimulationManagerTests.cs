using System;
using NUnit.Framework;
using System.Threading;
using Mob;
using Surface;

namespace Simulation.Tests {
	[TestFixture]
	public class SimulationManagerTests {
		private const int DELAY_MS = 500;

		private ActorManager _actorManager;
		private IMapProvider _mapProvider;

		[SetUp]
		public void SetUp() {
			_actorManager = default;
			_mapProvider = default;
		}

#if DEBUG
		[Test]
		public void Start_NotStarted_ThreadStarted() {
			using( var gate = new AutoResetEvent( false ) ) {
				var manager = new SimulationManager( _actorManager, _mapProvider );
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
		}

		[Test]
		public void Start_AlreadyStarted_NoEffect() {
			using( var gate = new AutoResetEvent( false ) ) {
				var manager = new SimulationManager( _actorManager, _mapProvider );
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
		}

		[Test]
		public void Stop_NotStarted_NoEffect() {
			using( var gate = new AutoResetEvent( false ) ) {
				var manager = new SimulationManager( _actorManager, _mapProvider );
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
		}

		[Test]
		public void Stop_AlreadyStarted_ThreadStopped() {
			using( var gate = new AutoResetEvent( false ) ) {
				var manager = new SimulationManager( _actorManager, _mapProvider );
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
		}
#endif
	}
}
