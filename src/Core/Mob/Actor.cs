using System;
using System.Threading;
using BlockColony.Core.Pathfinding;
using BlockColony.Core.Surface;
using BlockColony.Core.Work;

namespace BlockColony.Core.Mob {
	public sealed class Actor : IPathfindingCallback, IJobFit {
		private Route? _route;
		private Route? _pendingRoute;
		private IJob? _job;
		private IJob? _pendingJob;
		private int _currentRouteIndex;

		private int _currentActivity;
		private int _currentStep;

		public const int NoContext = 0;
		public const int MoveContext = 1;
		public const int MeasureContext = 2;

#if DEBUG
		public event EventHandler? JobAssigned;
		public event EventHandler? PathAssigned;
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

			if( _job == default( IJob ) ) {
				_job = Interlocked.Exchange( ref _pendingJob, default );

				if( _job != default( IJob ) ) {
					_currentActivity = 0;
					_currentStep = 0;
					SetRouteRequired();
				}
			}
		}

		public ActivityStep? GetActivityStep() {
			if( _job == default( IJob ) ) {
				return default;
			}

			return _job.Activities[ _currentActivity ].Steps[ _currentStep ];
		}

		// Called from the Ui thread
		public int GetDesiredRouteStep() {
			if( _currentRouteIndex < 0
				|| _route == default
			) {
				return -1;
			}

			return _route[ _currentRouteIndex ];
		}

		// Called from the Ui thread
		public void RouteStepComplete( ref MapCell mapCell ) {
			if (_route == default) {
				throw new InvalidOperationException( "RouteStepComplete without route." );
			}

			if (_job == default) {
				throw new InvalidOperationException( "RouteStepComplete without job." );
			}

			Column = mapCell.Column;
			Row = mapCell.Row;
			_currentRouteIndex += 1;
			if(	_currentRouteIndex >= _route.Count ) {
				_currentRouteIndex = -1;
				_route = default;
				Errand = _job.Activities[ _currentActivity ].Steps[ _currentStep ].Errand;
			}
		}

		public void ErrandComplete() {
			if (_job == default) {
				throw new InvalidOperationException( "ErrandComplete with no job." );
			}

			if( Errand == Errand.WaitingToPath ) {
				if( _pendingRoute == default( Route ) ) {
					throw new InvalidOperationException( "WaitingToPath marked complete with no route." );
				}
				Errand = Errand.Pathing;
			} else {
				_currentStep += 1;
				if( _currentStep >= _job.Activities[ _currentActivity ].Steps.Length ) {
					StepComplete();
				} else {
					SetRouteRequired();
				}
			}
		}

		private void SetRouteRequired() {
			if (_job == default) {
				throw new InvalidOperationException( "SetRouteRequired with no job." );
			}

			ActivityStep step = _job.Activities[ _currentActivity ].Steps[ _currentStep ];
			if( ( step.Column != Column )
				|| ( step.Row != Row ) ) {
				Errand = Errand.WaitingToPath;
			} else {
				Errand = step.Errand;
			}
		}

		private void StepComplete() {
			if (_job == default) {
				throw new InvalidOperationException( "StepComplete without job." );
			}

			_currentActivity += 1;
			_currentStep = 0;
			if( _currentActivity >= _job.Activities.Length ) {
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
			Route? oldPendingPath = Interlocked.Exchange( ref _pendingRoute, path );
#if DEBUG
			PathAssigned?.Invoke( this, EventArgs.Empty );
#endif
			if( oldPendingPath != default ) {
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
		IJob? IJobFit.AssignJob( IJob job ) {
			IJob? result = Interlocked.Exchange( ref _pendingJob, job );
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
