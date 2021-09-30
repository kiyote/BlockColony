namespace BlockColony.Core.Surface {
	public interface ITerrain {
		byte Id { get; }

		string IdName { get; }

		ITerrainAttributes this[int celcius] { get; }
	}
}
