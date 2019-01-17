using System;
using System.Collections.Generic;
using Pathfinding;
using Surface;

namespace Mob {
	public class ActorManager {
		private readonly PathfindingManager _pathfindingManager;
		private readonly List<Actor> _actors;
		private readonly List<Actor> _idle;

		public ActorManager(
			PathfindingManager pathfindingManager
		) {
			_pathfindingManager = pathfindingManager;

			_actors = new List<Actor>();
			_idle = new List<Actor>();
		}

		public void Add(Actor actor) {
			_actors.Add( actor );

			_idle.Add( actor );
		}

		public List<Actor> GetIdleActors() {
			return _idle;
		}

		// Called from the UI thread
		public void UiUpdate() {
		}

		// Called from the simulation thread
		public void SimulationUpdate() {
			foreach (var actor in _actors) {
				actor.SimulationUpdate();
			}
		}

		private void MoveActor(Map map, Actor actor, MapCell goal) {
			MakeBusy( actor );
			ref var source = ref map.GetCell( actor.Column, actor.Row );
			_pathfindingManager.GetPath( map, ref source, ref goal, actor.Locomotion, actor, Actor.MoveContext );
		}

		private void MakeBusy(Actor actor) {
			_idle.Remove( actor );			
		}
	}
}