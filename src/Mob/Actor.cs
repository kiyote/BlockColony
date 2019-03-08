using System;
using System.Collections.Generic;
using System.Threading;
using Pathfinding;
using Surface;
using Work;

namespace Mob {
	public class Actor : IPathfindingCallback, IJobFit {
		private Route _route;
		private Route _pendingRoute;
		private Job _job;
		private Job _pendingJob;
		private bool _hasJob;
		private int _currentRouteIndex;

		private int _currentActivity;
		private int _currentStep;

		public const int NoContext = 0;
		public const int MoveContext = 1;
		public const int MeasureContext = 2;

#if DEBUG
		public event EventHandler JobAssigned;
#endif

		public Actor(
			int column,
			int row,
			Locomotion locomotion
		) {
			Column = column;
			Row = row;
			Locomotion = locomotion;
			_currentRouteIndex = -1;
			_currentActivity = -1;
			_currentStep = -1;
		}

		public int Column { get; }

		public int Row { get; }

		public Locomotion Locomotion { get; }

		public bool HasJob { get; private set; }

		// This will be called from the Simulation thread
		public void SimulationUpdate() {
			if( _route == default( Route ) ) {
				_route = Interlocked.Exchange( ref _pendingRoute, default( Route ) );
				_currentRouteIndex = -1;
			}

			if( _job == default( Job ) ) {
				_job = Interlocked.Exchange( ref _pendingJob, default( Job ) );
				_currentActivity = -1;
				_currentStep = -1;
			}

			if( _route != default( Route ) ) {
				if ( _currentRouteIndex == -1) {
					_currentRouteIndex = 0;		
				}
			}
		}

		// Called from the Ui thread
		public int GetDesiredRouteStep() {
			if( _currentRouteIndex < 0 ) {
				return -1;
			}

			return _route[ _currentRouteIndex ];
		}

		// Called from the Ui thread
		public void RouteStepComplete() {
			_currentRouteIndex += 1;
			if ( _currentRouteIndex >= _route.Count) {
				// TODO: What now, genius?!
				_currentRouteIndex = -1;
				_route = default( Route );
			}
		}

		public Errand GetErrand() {
			if (_currentStep == -1) {
				_currentStep = 0;
			}
			return _job.Activity[ _currentActivity ].Step[ _currentStep ].Errand;
		}

		public void ErrandComplete() {
			_currentStep += 1;
			if (_currentStep >= _job.Activity[ _currentActivity ].Step.Length) {
				StepComplete();
			} else {
				var step = _job.Activity[ _currentActivity ].Step[ _currentStep ];
				if ((step.Column != Column)
					|| (step.Row != Row)) {
					// We need to path to the new location
				}
			}
		}

		private void StepComplete() {
			_currentActivity += 1;
			if (_currentActivity >= _job.Activity.Length) {
				_job = default( Job );
				_currentActivity = -1;
				_currentStep = -1;
				_currentRouteIndex = -1;
			}
		}

		// This will be called from the Pathfinding thread
		void IPathfindingCallback.PathFound( Route path, int context ) {
			var oldPendingPath = Interlocked.Exchange( ref _pendingRoute, path );

			if( oldPendingPath != default( Route ) ) {
				// We updated the pending pathing before it was ever seen
				// by the UI thread.
			}
		}

		// This will be called from the Job thread
		Job IJobFit.AssignJob( Job job ) {
			var result = Interlocked.Exchange( ref _pendingJob, job );
#if DEBUG
			JobAssigned?.Invoke( this, EventArgs.Empty );
#endif
			return result;
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