using System;
using System.Diagnostics.CodeAnalysis;

namespace Work {
	public abstract class Activity : IEquatable<Activity> {

		public Activity( ActivityStep[] steps ) {
			Steps = steps ?? throw new InvalidOperationException();
		}

		[SuppressMessage(
			"Performance", "CA1819:Properties should not return arrays",
			Justification = "Performance" )]
		public ActivityStep[] Steps { get; }

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public bool Equals( Activity other ) {
			if( other is null ) {
				return false;
			}

			if( ReferenceEquals( other, this ) ) {
				return true;
			}

			int sourceLength = Steps.Length;
			int targetLength = other.Steps.Length;

			if( sourceLength != targetLength ) {
				return false;
			}

			for( int i = 0; i < sourceLength; i++ ) {
				bool found = false;
				ref ActivityStep sourceStep = ref Steps[ i ];
				for( int j = 0; j < targetLength; j++ ) {
					ref ActivityStep targetStep = ref other.Steps[ i ];

					if( sourceStep == targetStep ) {
						found = true;
					}
				}

				if( !found ) {
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
				int stepLength = Steps.Length;
				for( int i = 0; i < stepLength; stepLength++ ) {
					ref ActivityStep step = ref Steps[ i ];
					result = ( result * 31 ) + step.GetHashCode();
				}

				return result;
			}
		}
	}
}
