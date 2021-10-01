using System;
using System.Collections.Generic;
using BlockColony.Core.Pathfinding;
using BlockColony.Core.Surface;
using BlockColony.Core.Work;

namespace BlockColony.Core.Mob {
	public sealed class ActorManager : IJobFitProvider {
		private readonly IPathfindingManager _pathfindingManager;
		private readonly List<Actor> _actors;
		private readonly List<Actor> _idle;

		public ActorManager(
			IPathfindingManager pathfindingManager
		) {
			_pathfindingManager = pathfindingManager;

			_actors = new List<Actor>();
			_idle = new List<Actor>();
		}

		public void Add( Actor actor ) {
			_actors.Add( actor );

			_idle.Add( actor );
		}

		public void Remove( Actor actor ) {
			_actors.Remove( actor );
			_idle.Remove( actor );
		}

		public List<Actor> GetIdleActors() {
			return _idle;
		}

		// Called from the UI thread
		public void UiUpdate() {
			// Throw this away once there's code in here.
			if (_actors == default) {
				throw new InvalidOperationException();
			}
		}

		// Called from the simulation thread
		public void SimulationUpdate( IMap map ) {
			if (map == default) {
				throw new ArgumentNullException( nameof( map ) );
			}

			foreach( Actor actor in _actors ) {
				actor.SimulationUpdate();

				if( actor.Errand == Errand.WaitingToPath ) {
					ActivityStep? step = actor.GetActivityStep();
					if (step == default) {
						throw new InvalidOperationException( "SimulationUpdate: Errand waiting to path with no ActivityStep." );
					}
					ref MapCell target = ref map.GetCell( step.Column, step.Row );
					ref MapCell source = ref map.GetCell( actor.Column, actor.Row );
					_pathfindingManager.GetPath( map, ref source, ref target, actor.Locomotion, actor, Actor.MoveContext );
				}
			}
		}

		private void MoveActor( IMap map, Actor actor, MapCell goal ) {
			MakeBusy( actor );
			ref MapCell source = ref map.GetCell( actor.Column, actor.Row );
			_pathfindingManager.GetPath( map, ref source, ref goal, actor.Locomotion, actor, Actor.MoveContext );
		}

		private void MakeBusy( Actor actor ) {
			_idle.Remove( actor );
		}

		IJobFit[] IJobFitProvider.GetAvailable() {
			var result = new List<Actor>();
			for( int i = 0; i < _idle.Count; i++ ) {
				if( _idle[ i ].Errand == Errand.Idle ) {
					result.Add( _idle[ i ] );
				}
			}

			return result.ToArray();
		}
	}
}
