using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class ChaseState : GhostState
    {
        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.Empty, 
            CellType.Pacman, 
            CellType.JoinGameCell 
        };

        public ChaseState(GhostBlackboard blackboard) : base(blackboard)
        {
        }

        public override void OnEnter()
        {
            // Triggered dynamically
        }

        public override void Update()
        {
            if (GhostBlackboard.MovementController.IsMoving) return;

            Direction nextStep = GhostBlackboard.CurrentDirection;

            // Rule: "Yapay zekalar bir yönde hareket etmeye başladılar ise bir sonraki köşe noktaya gelene kadar yön değiştirmezler."
            // We only recalculate our path if we are at an intersection or a corner.
            if (IsAtCornerOrIntersection())
            {
                Vector2Int pacmanCell = GhostBlackboard.TargetPacman.GetComponent<GridMovementController>().CurrentCell;
                nextStep = GetShortestPathStep(GhostBlackboard.MovementController.CurrentCell, pacmanCell);
            }

            if (nextStep != Direction.None)
            {
                GhostBlackboard.CurrentDirection = nextStep;
                GhostBlackboard.MovementController.TryMoveInDirection(
                    nextStep, 
                    GameSettings.AiMovementDuration, 
                    _walkableCells, 
                    false);
            }
        }

        private bool IsAtCornerOrIntersection()
        {
            Vector2Int currentCell = GhostBlackboard.MovementController.CurrentCell;
            Vector2Int forwardCell = currentCell + GhostBlackboard.CurrentDirection.ToVector2Int();
            
            bool canGoForward = GhostBlackboard.CurrentDirection != Direction.None && 
                                GhostBlackboard.GridData.IsCellMovable(forwardCell, _walkableCells);

            int validOptions = 0;
            foreach (Direction dir in GameSettings.DirectionsToCheck)
            {
                if (dir == GhostBlackboard.CurrentDirection.Reverse()) continue; // Don't count behind us
                if (GhostBlackboard.GridData.IsCellMovable(currentCell + dir.ToVector2Int(), _walkableCells))
                {
                    validOptions++;
                }
            }

            // It's a corner/intersection if we have more than 1 option (a branch) 
            // OR if we have 0 or 1 options but we CANNOT go forward (a sharp turn or dead end).
            return validOptions > 1 || !canGoForward;
        }

        private Direction GetShortestPathStep(Vector2Int start, Vector2Int target)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            
            queue.Enqueue(start);
            cameFrom[start] = start;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (current == target) break;

                foreach (Vector2Int neighbor in current.GetNeighbours())
                {
                    // Prevent turning fully backward during path recalculation
                    if (neighbor == start + GhostBlackboard.CurrentDirection.Reverse().ToVector2Int()) continue;

                    if (!cameFrom.ContainsKey(neighbor) && GhostBlackboard.GridData.IsCellMovable(neighbor, _walkableCells))
                    {
                        cameFrom[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            if (cameFrom.ContainsKey(target))
            {
                Vector2Int current = target;
                while (cameFrom[current] != start)
                {
                    current = cameFrom[current];
                }
                return (current - start).ToDirection();
            }

            // Fallback if no path is found
            return GhostBlackboard.CurrentDirection;
        }
    }
}