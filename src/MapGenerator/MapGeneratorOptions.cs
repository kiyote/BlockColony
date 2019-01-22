using System;
using System.Collections.Generic;
using System.Text;

namespace MapGenerator {
	public class RiverTerrainOptions {
		public string Easy { get; set; }

		public string Hard { get; set; }
	}

	public class LayerTerrainOptions {
		public string IdName { get; set; }
	}

	public class TerrainOptions {
		public LayerTerrainOptions Outside { get; set; }

		public LayerTerrainOptions Entry { get; set; }

		public LayerTerrainOptions Middle { get; set; }

		public LayerTerrainOptions Far { get; set; }

		public RiverTerrainOptions River { get; set; }
	}

	public class MapGeneratorOptions {

		public short Columns { get; set; }

		public short Rows { get; set; }

		public int AmbientTemperature { get; set; }

		public TerrainOptions Terrain { get; set; }

	}
}