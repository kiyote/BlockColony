namespace BlockColony.Core.Surface {

	internal record PhaseConfigEntry(
		int Transition,
		string Name,
		TerrainPhase Phase,
		bool Pathable,
		int PathingCost
	) : ITerrainAttributes { }

	internal record PhaseConfig(
		string Name,
		PhaseConfigEntry[] Phases
	);

	internal record TerrainConfig(
		byte Id,
		string IdName,
		string PhaseName
	);

	internal record TerrainFile(
		PhaseConfig[] Phase,
		TerrainConfig[] Terrain
	);
}
