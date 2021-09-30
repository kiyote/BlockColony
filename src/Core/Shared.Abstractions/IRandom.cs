namespace BlockColony.Core.Shared {
	public interface IRandom {
		int Next();
		int Next( int upperBound );
		int Next( int lowerBound, int upperBound );
		bool NextBool();
		void NextBytes( byte[] buffer );
		void NextBytes( byte[] buffer, int bound1, int bound2 );
		double NextDouble();
		uint NextUInt();
		void Reinitialise( int seed );
	}
}
