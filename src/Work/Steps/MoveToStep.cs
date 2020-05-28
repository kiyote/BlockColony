using System;
using Surface;

namespace Work.Steps
{
    public sealed class MoveToStep: ActivityStep
    {
		public MoveToStep(ref MapCell cell):
			base(Errand.MoveTo, cell.Column, cell.Row) {
		}

		public MoveToStep( int column, int row ) :
			base( Errand.MoveTo, column, row ) {
		}
    }
}
