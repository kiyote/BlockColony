using System;
using BlockColony.Core.Surface;

namespace BlockColony.Core.Pathfinding {
	public interface IPathfindingManager: IDisposable {
		bool IsRunning { get; }

#if DEBUG
		event EventHandler Started;
		event EventHandler Stopped;
#endif

		void GetPath(
			IMap map,
			ref MapCell start,
			ref MapCell goal,
			Locomotion locomotion,
			IPathfindingCallback callback,
			int callbackContext
		);
		void Start();
		void Stop();
	}
}
