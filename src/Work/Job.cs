using System;

namespace BlockColony.Core.Work {
	internal sealed class Job : IJob {
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
			Activities = activities ?? throw new InvalidOperationException();
			State = JobState.Pending;
		}

		public int Priority { get; }

		public Activity[] Activities { get; }

		public int Age { get; }

		public JobState State { get; private set; }

		public void SetState( JobState state ) {
			State = state;
		}

		public bool Equals( IJob other ) {
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

			int sourceLength = Activities.Length;
			int targetLength = other.Activities.Length;

			if( sourceLength != targetLength ) {
				return false;
			}

			for( int i = 0; i < sourceLength; i++ ) {
				bool found = false;
				ref Activity sourceAction = ref Activities[i];
				for( int j = 0; j < targetLength; j++ ) {
					ref Activity targetAction = ref other.Activities[i];

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
			return Equals( obj as IJob );
		}

		public override int GetHashCode() {
			unchecked {
				int result = Priority;
				result = ( result * 31 ) + Age;
				result = ( result * 31 ) + State.GetHashCode();
				int length = Activities.Length;
				for( int i = 0; i < length; i++ ) {
					result = ( result * 31 ) + Activities[i].GetHashCode();
				}

				return result;
			}
		}
	}
}
