using System;

namespace Surface {
	public struct Location: IEquatable<Location> {
		public int Column { get; set; }
		public int Row { get; set; }

		public override bool Equals( object obj ) {
			if (!(obj is Location)) {
				return false;
			}

			return Equals( (Location)obj );
		}

		public bool Equals( Location other ) {
			return ( other != default )
				&& ( Column == other.Column )
				&& ( Row == other.Row );
		}

		public override int GetHashCode() {
			unchecked {
				int result = 31;
				result = ( result * 31 ) + Column.GetHashCode();
				result = ( result * 31 ) + Row.GetHashCode();

				return result;
			}
				
		}

		public static bool operator ==( Location left, Location right ) {
			return left.Equals( right );
		}

		public static bool operator !=( Location left, Location right ) {
			return !( left == right );
		}
	}
}
