using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class JoiningGameState : GhostState
    {
        private Vector2Int _targetCell;
        
        // To exit, ghosts are allowed to walk on the Gate, SpawnZone, Empty, and JoinGameCell
        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.AiSpawnZone, 
            CellType.AiGate, 
            CellType.Empty, 
            CellType.JoinGameCell,
            CellType.Pacman
        };

        public JoiningGameState(GhostBlackboard blackboard) : base(blackboard)
        {
        }

        public override void OnEnter()
        {
            // Find where we need to go
            _targetCell = GhostBlackboard.GridData.GetCoordsOfCellType(CellType.JoinGameCell)[0];
        }

        public override void Update()
        {
            if (GhostBlackboard.MovementController.IsMoving) return;

            // 1. Check if we have arrived at the JoinGameCell
            if (GhostBlackboard.MovementController.CurrentCell == _targetCell)
            {
                GhostBlackboard.GhostComponent.ChangeState(
                    new ScatterState(GhostBlackboard),
                    CMP.Scripts.GhostState.Scatter
                );
                return; 
            }

            // 2. Find the next step to take using BFS
            Direction nextStep = GetNextStepTowards(GhostBlackboard.MovementController.CurrentCell, _targetCell);
            
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

        // A standard Breadth-First Search to find the shortest path on the grid
        private Direction GetNextStepTowards(Vector2Int start, Vector2Int target)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            
            queue.Enqueue(start);
            cameFrom[start] = start;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (current == target)
                    break;

                foreach (Vector2Int neighbor in current.GetNeighbours())
                {
                    if (!cameFrom.ContainsKey(neighbor) && GhostBlackboard.GridData.IsCellMovable(neighbor, _walkableCells))
                    {
                        cameFrom[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // If a path was found, backtrack to find the very first step we need to take
            if (cameFrom.ContainsKey(target))
            {
                Vector2Int current = target;
                while (cameFrom[current] != start)
                {
                    current = cameFrom[current];
                }
                
                // Convert the cell difference into a Direction enum
                return (current - start).ToDirection();
            }

            return Direction.None;
        }
    }
}