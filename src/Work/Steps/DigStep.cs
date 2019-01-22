using System;
using System.Collections.Generic;
using System.Text;
using Surface;

namespace Work.Steps
{
    public class DigStep: Step
    {
		public DigStep(ref MapCell cell):
			base(Errand.Dig, cell.Column, cell.Row) {
		}
    }
}
