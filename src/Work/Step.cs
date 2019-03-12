using System;

namespace Work {
	public class Step : IEquatable<Step> {

		public Step(
			Errand errand,
			int column,
			int row
		) {
			Errand = errand;
			Column = column;
			Row = row;
		}

		public Errand Errand { get; }

		public int Column { get; }

		public int Row { get; }

		public bool Equals( Step other ) {
			if( ReferenceEquals( other, null ) ) {
				return false;
			}

			if( ReferenceEquals( other, this ) ) {
				return true;
			}

			return Errand == other.Errand
				&& Column == other.Column
				&& Row == other.Row;
		}

		public override bool Equals( object obj ) {
			return Equals( obj as Step );
		}

		public override int GetHashCode() {
			unchecked {
				int result = (sbyte)Errand;
				result = ( result * 31 ) + Column;
				result = ( result * 31 ) + Row;

				return result;
			}
		}
	}
}