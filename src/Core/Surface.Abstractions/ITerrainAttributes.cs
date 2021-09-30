namespace BlockColony.Core.Surface {
	public interface ITerrainAttributes {

		string Name { get; }

		TerrainPhase Phase { get; }

		int Colour { get; }

		bool Pathable { get; }

		int PathingCost { get; }
	}
}
