using System;
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
		private int _currentRouteIndex;

		private int _currentActivity;
		private int _currentStep;

		public const int NoContext = 0;
		public const int MoveContext = 1;
		public const int MeasureContext = 2;

#if DEBUG
		public event EventHandler JobAssigned;
		public event EventHandler PathAssigned;
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
			Errand = Errand.Idle;
		}

		public int Column { get; private set; }

		public int Row { get; private set; }

		public Locomotion Locomotion { get; }

		public Errand Errand { get; private set; }

		// This will be called from the Simulation thread
		public void SimulationUpdate() {
			if( _route == default( Route ) ) {
				_route = Interlocked.Exchange( ref _pendingRoute, default );

				if( _route != default( Route ) ) {
					_currentRouteIndex = 0;
				}
			}

			if( _job == default( Job ) ) {
				_job = Interlocked.Exchange( ref _pendingJob, default );

				if( _job != default( Job ) ) {
					_currentActivity = 0;
					_currentStep = 0;
					SetRouteRequired();
				}
			}
		}

		public Step GetActivityStep() {
			if( _job == default( Job ) ) {
				return default;
			}

			return _job.Activity[ _currentActivity ].Step[ _currentStep ];
		}

		// Called from the Ui thread
		public int GetDesiredRouteStep() {
			if( _currentRouteIndex < 0 ) {
				return -1;
			}

			return _route[ _currentRouteIndex ];
		}

		// Called from the Ui thread
		public void RouteStepComplete( ref MapCell mapCell ) {
			Column = mapCell.Column;
			Row = mapCell.Row;
			_currentRouteIndex += 1;
			if( _currentRouteIndex >= _route.Count ) {
				_currentRouteIndex = -1;
				_route = default;
				Errand = _job.Activity[ _currentActivity ].Step[ _currentStep ].Errand;
			}
		}

		public void ErrandComplete() {

			if( Errand == Errand.WaitingToPath ) {
				if( _pendingRoute == default( Route ) ) {
					throw new InvalidOperationException( "WaitingToPath marked complete with no route." );
				}
				Errand = Errand.Pathing;
			} else {
				_currentStep += 1;
				if( _currentStep >= _job.Activity[ _currentActivity ].Step.Length ) {
					StepComplete();
				} else {
					SetRouteRequired();
				}
			}
		}

		private void SetRouteRequired() {
			Step step = _job.Activity[ _currentActivity ].Step[ _currentStep ];
			if( ( step.Column != Column )
				|| ( step.Row != Row ) ) {
				Errand = Errand.WaitingToPath;
			} else {
				Errand = step.Errand;
			}
		}

		private void StepComplete() {
			_currentActivity += 1;
			_currentStep = 0;
			if( _currentActivity >= _job.Activity.Length ) {
				_job = default;
				_route = default;
				_currentActivity = -1;
				_currentStep = -1;
				_currentRouteIndex = -1;
				Errand = Errand.Idle;
			}
		}

		// This will be called from the Pathfinding thread
		void IPathfindingCallback.PathFound( Route path, int context ) {
			Route oldPendingPath = Interlocked.Exchange( ref _pendingRoute, path );
#if DEBUG
			PathAssigned?.Invoke( this, EventArgs.Empty );
#endif
			if( oldPendingPath != default( Route ) ) {
				// We updated the pending pathing before it was ever seen by the UI thread.
				throw new InvalidOperationException( "Route received before previous route accepted." );
			}

			if( Errand == Errand.WaitingToPath ) {				
				ErrandComplete();
			} else {
				throw new InvalidOperationException( "Route received with no pending request." );
			}
		}

		// This will be called from the Job thread
		Job IJobFit.AssignJob( Job job ) {
			Job result = Interlocked.Exchange( ref _pendingJob, job );
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
