using System;

namespace BlockColony.Core.Surface {
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

		public override bool Equals( object? obj ) {
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
			HashCode hash = new HashCode();
			hash.Add( Index );
			hash.Add( Row );
			hash.Add( Column );
			hash.Add( TerrainCost );
			hash.Add( TerrainId );
			hash.Add( Temperature );
			hash.Add( Moisture );
			hash.Add( TerrainLevel );
			hash.Add( NewTemperature );
			hash.Add( NewTerrainId );
			hash.Add( NewMoisture );
			return hash.ToHashCode();
		}

		public static bool operator ==( MapCell left, MapCell right ) {
			return left.Equals( right );
		}

		public static bool operator !=( MapCell left, MapCell right ) {
			return !( left == right );
		}
	}
}
