namespace BlockColony.Core.MapGenerator {


	public sealed class MapGeneratorOptions {

		public MapGeneratorOptions(
			short columns,
			short rows
		) {
			Columns = columns;
			Rows = rows;
		}

		public short Columns { get; }

		public short Rows { get; }
	}
}
