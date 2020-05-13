using System;

namespace Surface {
	public struct MapCell: IEquatable<MapCell> {
		public int Index { get; set; }
		public short Row { get; set; }
		public short Column { get; set; }

		public short TerrainCost { get; set; }
		public byte TerrainId { get; set; }
		public short Temperature { get; set; }
		public byte Moisture { get; set; }

		/// <summary>
		/// The fill-level of the terrain, from -100 to 100
		/// </summary>
		public sbyte TerrainLevel { get; set; }

		public short NewTemperature { get; set; }
		public byte NewTerrainId { get; set; }
		public byte NewMoisture { get; set; }

		/// <summary>
		/// Flags indicating the reachable cells from this one
		/// </summary>
		/// <seealso cref="Directions"/>
		public byte Walkability { get; set; }

		public override bool Equals( object obj ) {
			if (!(obj is MapCell)) {
				return false;
			}

			return Equals( (MapCell)obj );
		}

		public bool Equals( MapCell other ) {
			return
				Index == other.Index
				&& Row == other.Row
				&& Column == other.Column
				&& TerrainCost == other.TerrainCost
				&& TerrainId == other.TerrainId
				&& Temperature == other.Temperature
				&& Moisture == other.Moisture
				&& TerrainLevel == other.TerrainLevel
				&& NewTemperature == other.NewTemperature
				&& NewTerrainId == other.NewTerrainId
				&& NewMoisture == other.NewMoisture;
		}

		public override int GetHashCode() {
			unchecked {
				int result = 17;
				result = ( result * 31 ) + Index.GetHashCode();
				result = ( result * 31 ) + Row.GetHashCode();
				result = ( result * 31 ) + Column.GetHashCode();
				result = ( result * 31 ) + TerrainCost.GetHashCode();
				result = ( result * 31 ) + TerrainId.GetHashCode();
				result = ( result * 31 ) + Temperature.GetHashCode();
				result = ( result * 31 ) + Moisture.GetHashCode();
				result = ( result * 31 ) + TerrainLevel.GetHashCode();
				result = ( result * 31 ) + NewTemperature.GetHashCode();
				result = ( result * 31 ) + NewTerrainId.GetHashCode();
				result = ( result * 31 ) + NewMoisture.GetHashCode();

				return result;
			}
		}

		public static bool operator ==( MapCell left, MapCell right ) {
			return left.Equals( right );
		}

		public static bool operator !=( MapCell left, MapCell right ) {
			return !( left == right );
		}
	}
}
