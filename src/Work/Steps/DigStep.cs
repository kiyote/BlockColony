using System;
using Surface;

namespace Work.Steps {
	public sealed class DigStep : ActivityStep {
		public DigStep( ref MapCell cell ) :
			base( Errand.Dig, cell.Column, cell.Row ) {
		}

		public DigStep( int column, int row )
			: base( Errand.Dig, column, row ) {
		}
	}
}
