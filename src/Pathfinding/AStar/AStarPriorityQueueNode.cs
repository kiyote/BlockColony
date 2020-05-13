using System;
using Pathfinding.BlueRajah;

namespace Pathfinding.AStar
{
    internal sealed class AStarPriorityQueueNode: FastPriorityQueueNode
    {
		public int CellIndex;
    }
}
