using System;
using System.Diagnostics.CodeAnalysis;

namespace Work {
	public class Job : IEquatable<Job> {
		public const int PriorityCount = 10;
		public const int Idle = 8;
		public const int Low = 6;
		public const int Medium = 4;
		public const int High = 2;
		public const int Critical = 0;

		public Job(
			int priority,
			Activity[] activities
		) {
			Priority = priority;
			Activity = activities ?? throw new InvalidOperationException();
			State = JobState.Pending;
		}

		public int Priority { get; }

		[SuppressMessage(
			"Performance", "CA1819:Properties should not return arrays",
			Justification = "Performance" )]
		public Activity[] Activity { get; }

		public int Age { get; }

		public JobState State { get; private set; }

		public void SetState( JobState state ) {
			State = state;
		}

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public bool Equals( Job other ) {
			if( other is null ) {
				return false;
			}

			if( ReferenceEquals( other, this ) ) {
				return true;
			}

			if( ( Priority != other.Priority )
				|| ( Age != other.Age )
				|| ( State != other.State ) ) {

				return false;
			}

			int sourceLength = Activity.Length;
			int targetLength = other.Activity.Length;

			if( sourceLength != targetLength ) {
				return false;
			}

			for( int i = 0; i < sourceLength; i++ ) {
				bool found = false;
				ref Activity sourceAction = ref Activity[ i ];
				for( int j = 0; j < targetLength; j++ ) {
					ref Activity targetAction = ref other.Activity[ i ];

					if( sourceAction == targetAction ) {
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
			return Equals( obj as Job );
		}

		//[Il2CppSetOption( Option.NullChecks, false )]
		//[Il2CppSetOption( Option.ArrayBounds, false )]
		public override int GetHashCode() {
			unchecked {
				int result = Priority;
				result = ( result * 31 ) + Age;
				result = ( result * 31 ) + State.GetHashCode();
				int length = Activity.Length;
				for( int i = 0; i < length; i++ ) {
					result = ( result * 31 ) + Activity[ i ].GetHashCode();
				}

				return result;
			}
		}
	}
}
