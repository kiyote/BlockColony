using System;
using System.Collections.Generic;
using System.Threading;
using Pathfinding;
using Surface;
using Work;

namespace Mob {
	public class Actor : IPathfindingCallback, IJobFit {
		private Route _path;
		private Route _pendingPath;
		private Job _job;
		private Job _pendingJob;
		private bool _hasJob;

		public const int NoContext = 0;
		public const int MoveContext = 1;
		public const int MeasureContext = 2;

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

		public bool HasJob { get; private set; }

		// This will be called from the Simulation thread
		public void SimulationUpdate() {
			if( _path == default( Route ) ) {
				_path = Interlocked.Exchange( ref _pendingPath, default( Route ) );
			}

			if( _job == default( Job ) ) {
				_job = Interlocked.Exchange( ref _pendingJob, default( Job ) );
			}
		}

		// This will be called from the Pathfinding thread
		void IPathfindingCallback.PathFound( Route path, int context ) {
			var oldPendingPath = Interlocked.Exchange( ref _pendingPath, path );

			if( oldPendingPath != default( Route ) ) {
				// We updated the pending pathing before it was ever seen
				// by the UI thread.
			}
		}

		// This will be called from the Job thread
		Job IJobFit.AssignJob( Job job ) {
			return Interlocked.Exchange( ref _pendingJob, job );
		}

		int IJobFit.LocationColumn {
			get {
				return Column;
			}
		}

		int IJobFit.LocationRow {
			get {
				return Row;
			}
		}

		Locomotion IJobFit.Locomotion {
			get {
				return Locomotion;
			}
		}
	}
}