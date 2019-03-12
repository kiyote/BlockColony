using System;

namespace Surface {
	internal class TerrainFile {
		public class PhaseConfig {
			public class PhaseConfigEntry : ITerrainAttributes {
				public int Transition { get; set; }
				public string Name { get; set; }
				public TerrainPhase Phase { get; set; }
				public int Colour { get; set; }

				public bool Pathable { get; set; }

				public int PathingCost { get; set; }
			}

			public string Name { get; set; }
			public PhaseConfigEntry[] Phases { get; set; }
		}

		public class TerrainConfig {
			public byte Id { get; set; }

			public string IdName { get; set; }

			public string Phase { get; set; }
		}

		public PhaseConfig[] Phase { get; set; }

		public TerrainConfig[] Terrain { get; set; }
	}
}