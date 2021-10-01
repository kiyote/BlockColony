namespace BlockColony.Core.Work {

	public sealed record Job(
		int Priority,
		Activity[] Activities,
		int Age,
		JobState State
	);
}
