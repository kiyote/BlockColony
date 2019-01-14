using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Surface;
using Work;

namespace Mob
{
    public class Actor: IJobFit
    {
		private Job _currentJob;
		private bool _movePending;
		private int _moveColumn;
		private int _moveRow;
		private List<int> _path;

		public Actor(
		) {
		}

		public Task FindJob(CancellationToken token) {
			return Task.CompletedTask;
		}

		// For polling by Unity thread
		public bool IsMovePending {
			get {
				return _movePending;
			}
		}

		public int Column { get; set; }

		public int Row { get; set; }

		// Called by the unity thread to pick up the current movement step
		// needed
		public Location GetPendingMove() {
			// Copy the movement locally
			Location result;
			result.Column = _moveColumn;
			result.Row = _moveRow;

			// Reset the movement
			_moveColumn = 0;
			_moveRow = 0;
			_movePending = false;

			// It may now happen that a non-unity thread has set the
			// pending movement already

			// Return the movment
			return result;
		}

		// For Unity thread to call when animation is done
		public void MovementComplete() {
			//TODO: Check to see if there are any steps remaining
			// If so, set the _moveColumn & _moveRow, then set
			// _movePending to true
		}

		public async Task<int> GetFitnessAsync( Job job ) {
			return 100;
		}
	}
}
