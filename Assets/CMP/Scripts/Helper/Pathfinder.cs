using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.Helper
{
    public static class Pathfinder
    {
        private static readonly Queue<Vector2Int> BfsQueue = new Queue<Vector2Int>();
        private static readonly Dictionary<Vector2Int, Vector2Int> BfsCameFrom = new Dictionary<Vector2Int, Vector2Int>();

        public static Direction GetShortestPathStep(
            GridData gridData, 
            Vector2Int start, 
            Vector2Int target, 
            List<CellType> walkableCells, 
            Direction directionToIgnore = Direction.None)
        {
            BfsQueue.Clear();
            BfsCameFrom.Clear();
            
            BfsQueue.Enqueue(start);
            BfsCameFrom[start] = start;

            Vector2Int? ignoreCell = null;
            if (directionToIgnore != Direction.None)
            {
                ignoreCell = start + directionToIgnore.ToVector2Int();
            }

            while (BfsQueue.Count > 0)
            {
                Vector2Int current = BfsQueue.Dequeue();

                if (current == target) 
                {
                    break;
                }

                foreach (Vector2Int neighbor in current.GetNeighbours())
                {
                    if (ignoreCell.HasValue && neighbor == ignoreCell.Value) 
                    {
                        continue;
                    }

                    if (!BfsCameFrom.ContainsKey(neighbor) && gridData.IsCellMovable(neighbor, walkableCells))
                    {
                        BfsCameFrom[neighbor] = current;
                        BfsQueue.Enqueue(neighbor);
                    }
                }
            }

            if (BfsCameFrom.ContainsKey(target))
            {
                Vector2Int current = target;
                while (BfsCameFrom[current] != start)
                {
                    current = BfsCameFrom[current];
                }
                
                return (current - start).ToDirection();
            }

            return Direction.None;
        }
    }
}