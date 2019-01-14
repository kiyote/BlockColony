using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Mob {
	public class ActorManager {
		private List<Actor> _actors;
		private List<Actor> _idle;

		public ActorManager() {
			_actors = new List<Actor>();
			_idle = new List<Actor>();
		}

		public void Add(Actor actor) {
			_actors.Add( actor );

			_idle.Add( actor );
		}

		public async Task Update(CancellationToken token) {
			await Task.WhenAll( _idle.Select( async actor => await actor.FindJob(token).ConfigureAwait(false) ) ).ConfigureAwait(false);
		}

		public List<Actor> GetIdleActors() {
			return _idle;
		}
	}
}