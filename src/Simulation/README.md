﻿The simulation manager relieves the UI thread from processing internal logic.  
The UI thread will call UiUpdate with the elapsed time and this will wake the simulation thread if it's not already running.  
If the simulation is running behind it will accumulate the elapsed milliseconds while the UiUpdate is called until another simulation tick occurs.
