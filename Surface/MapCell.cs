using System;

namespace Surface {
	public struct MapCell {
		public int Index;
		public short Row;
		public short Column;

		public short TerrainCost;
		public byte TerrainId;
		public short Temperature;
		public byte Moisture;

		/// <summary>
		/// The fill-level of the terrain, from -100 to 100
		/// </summary>
		public sbyte TerrainLevel;

		public short NewTemperature;
		public byte NewTerrainId;
		public byte NewMoisture;

		/// <summary>
		/// Flags indicating the reachable cells from this one
		/// </summary>
		/// <seealso cref="Direction"/>
		public byte Walkability;
	}
}