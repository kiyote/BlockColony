using System;
using BlockColony.Core.Pathfinding.BlueRajah;

namespace BlockColony.Core.Pathfinding.AStar
{
    internal sealed class AStarPriorityQueueNode: FastPriorityQueueNode
    {
		public int CellIndex;
    }
}
