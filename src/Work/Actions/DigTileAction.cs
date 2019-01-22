using System;
using Surface;
using Work.Steps;

namespace Work.Actions
{
    public class DigTileAction: Activity {

		public DigTileAction(ref MapCell cell)
			:base( new Step[] {
				new MoveToStep( ref cell ),
				new DigStep( ref cell )}
			) {
		}
    }
}
