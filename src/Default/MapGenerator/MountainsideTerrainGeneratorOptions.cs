namespace BlockColony.Default.MapGenerator {
	public sealed class RiverTerrainOptions {

		public RiverTerrainOptions(
			string easy,
			string hard
		) {
			Easy = easy;
			Hard = hard;
		}

		public string Easy { get; }

		public string Hard { get; }
	}

	public sealed class LayerTerrainOptions {

		public LayerTerrainOptions(
			string idName
		) {
			IdName = idName;
		}

		public string IdName { get; }
	}

	public sealed class TerrainOptions {

		public TerrainOptions(
			LayerTerrainOptions outside,
			LayerTerrainOptions entry,
			LayerTerrainOptions middle,
			LayerTerrainOptions far,
			RiverTerrainOptions river
		) {
			Outside = outside;
			Entry = entry;
			Middle = middle;
			Far = far;
			River = river;
		}

		public LayerTerrainOptions Outside { get; }

		public LayerTerrainOptions Entry { get; }

		public LayerTerrainOptions Middle { get; }

		public LayerTerrainOptions Far { get; }

		public RiverTerrainOptions River { get; }
	}

	public record MountainsideTerrainGeneratorOptions(
		int AmbientTemperature,
		TerrainOptions Terrain
	);
}
