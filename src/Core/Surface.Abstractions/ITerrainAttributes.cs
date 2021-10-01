namespace BlockColony.Core.Surface {
	public interface ITerrainAttributes {

		int Transition { get; }

		string Name { get; }

		TerrainPhase Phase { get; }

		bool Pathable { get; }

		int PathingCost { get; }
	}
}
