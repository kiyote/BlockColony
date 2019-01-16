using System;
using System.Collections.Generic;
using System.Threading;
using Pathfinding;
using Surface;

namespace Mob {
	public class Actor : IPathfindingCallback {
		private Route _path;
		private Route _pendingPath;

		public Actor(
			int column,
			int row,
			Locomotion locomotion
		) {
			Column = column;
			Row = row;
			Locomotion = locomotion;
		}

		public int Column { get; }

		public int Row { get; }

		public Locomotion Locomotion { get; }

		// This will be called from the UI thread
		public void UiUpdate() {
		}

		// This will be called from the Simulation thread
		public void SimulationUpdate() {
			if( _path == default( Route ) ) {
				Route localPendingPath = default( Route );

				localPendingPath = Interlocked.Exchange( ref _pendingPath, default( Route ) );
				_path = localPendingPath;
			}
		}

		// This will be called from the Pathfinding thread
		void IPathfindingCallback.PathFound( Route path ) {

			var oldPendingPath = Interlocked.Exchange( ref _pendingPath, path );

			if ( oldPendingPath != default(Route)) {
				// We updated the pending pathing before it was ever seen
				// by the UI thread.
			}
		}
	}
}
