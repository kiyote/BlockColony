using System;
using System.Collections.Generic;
using System.Text;
using Surface;

namespace Work.Steps
{
    public sealed class MoveToStep: ActivityStep
    {
		public MoveToStep(ref MapCell cell):
			base(Errand.MoveTo, cell.Column, cell.Row) {
		}
    }
}
