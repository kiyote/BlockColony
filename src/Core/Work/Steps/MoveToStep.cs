namespace BlockColony.Core.Work.Steps {
	public sealed record MoveToStep(int column, int row): ActivityStep(column, row, Errand.MoveTo);
}
