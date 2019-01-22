using System;
using System.Linq;

namespace Work {
	public class Activity: IEquatable<Activity> {

		public Activity(Step[] steps) {
			Step = steps ?? throw new InvalidOperationException();
		}

		public Step[] Step { get; }

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public bool Equals( Activity other ) {
			if (ReferenceEquals(other, null)) {
				return false;
			}

			if (ReferenceEquals(other, this)) {
				return true;
			}

			var sourceLength = Step.Length;
			var targetLength = other.Step.Length;

			if (sourceLength != targetLength) {
				return false;
			}

			for( int i = 0; i < sourceLength; i++ ) {
				bool found = false;
				ref var sourceStep = ref Step[ i ];
				for (int j = 0; j < targetLength; j++ ) {
					ref var targetStep = ref other.Step[ i ];

					if (sourceStep == targetStep) {
						found = true;
					}
				}

				if (!found) {
					return false;
				}
			}

			return true;
		}

		public override bool Equals( object obj ) {
			return Equals( obj as Activity );
		}

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public override int GetHashCode() {
			unchecked {
				int result = 0;
				var stepLength = Step.Length;
				for (int i = 0; i < stepLength; stepLength++) {
					ref var step = ref Step[ i ];
					result = ( result * 31 ) + step.GetHashCode();
				}

				return result;
			}
		}
	}
}
