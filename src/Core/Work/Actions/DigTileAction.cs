using BlockColony.Core.Work.Steps;

namespace BlockColony.Core.Work.Actions {
	public sealed record DigTileAction(int column, int row): Activity(
		new ActivityStep[] {
			new MoveToStep( column, row ),
			new DigStep( column, row )
		}
	);
}
