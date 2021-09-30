Actor is a target of multiple threads, so it shouldn't ever try to reach out to do things. Threads should always operate *on* Actor, not the other way around.

SimulationUpdate will cause the Actor to pick up any pending updates from other threads.

It's expected that someone else will check Errand and when it's something that needs handling it will perform it and then call ErrandComplete.

Similarly, CheckDesiredRouteStep != -1 is an indication the Actor wants to move.  Once someone else handles the move, RouteStepComplete should be called.
