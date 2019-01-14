using System;

namespace Work {
	public class Step: IEquatable<Step> {

		public Step(
			Errand activity, 
			int column, 
			int row
		) {
			Activity = activity;
			Column = column;
			Row = row;
		}

		public Errand Activity { get; }

		public int Column { get; }

		public int Row { get; }

		public bool Equals( Step other ) {
			if (ReferenceEquals(other, null)) {
				return false;
			}

			if (ReferenceEquals(other, this)) {
				return true;
			}

			return Activity == other.Activity
				&& Column == other.Column
				&& Row == other.Row;
		}

		public override bool Equals( object obj ) {
			return Equals( obj as Step );
		}

		public override int GetHashCode() {
			unchecked {
				int result = (sbyte)Activity;
				result = ( result * 31 ) + Column;
				result = ( result * 31 ) + Row;

				return result;
			}
		}
	}
}