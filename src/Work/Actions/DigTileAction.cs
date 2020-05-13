using System;
using Surface;
using Work.Steps;

namespace Work.Actions
{
    public sealed class DigTileAction: Activity {

		public DigTileAction(ref MapCell cell)
			:base( new ActivityStep[] {
				new MoveToStep( ref cell ),
				new DigStep( ref cell )}
			) {
		}
    }
}
