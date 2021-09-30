
namespace BlockColony.Core.Work.Steps {

	public sealed record DigStep(int column, int row ): ActivityStep(column, row, Errand.Dig);
}
