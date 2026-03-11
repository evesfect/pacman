using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class ScatterState : GhostState
    {
        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.Empty, 
            CellType.Pacman, 
            CellType.JoinGameCell 
        };

        public ScatterState(GhostBlackboard blackboard) : base(blackboard)
        {
        }

        public override void OnEnter()
        {
        }

        public override void Update()
        {
            if (GhostBlackboard.MovementController.IsMoving) return;

            Direction nextDirection = GetNextRandomDirection();
            
            if (nextDirection != Direction.None)
            {
                GhostBlackboard.CurrentDirection = nextDirection;
                GhostBlackboard.MovementController.TryMoveInDirection(
                    nextDirection, 
                    GameSettings.AiMovementDuration, 
                    _walkableCells, 
                    false); 
            }
        }

        private Direction GetNextRandomDirection()
        {
            List<Direction> possibleDirections = new List<Direction>();
            Vector2Int currentCell = GhostBlackboard.MovementController.CurrentCell;

            foreach (Direction dir in GameSettings.DirectionsToCheck)
            {
                if (dir == GhostBlackboard.CurrentDirection.Reverse() && GhostBlackboard.CurrentDirection != Direction.None)
                {
                    continue;
                }

                Vector2Int nextCell = currentCell + dir.ToVector2Int();
                if (GhostBlackboard.GridData.IsCellMovable(nextCell, _walkableCells))
                {
                    possibleDirections.Add(dir);
                }
            }

            if (possibleDirections.Count > 0)
            {
                int randomIndex = Random.Range(0, possibleDirections.Count);
                return possibleDirections[randomIndex];
            }

            return GhostBlackboard.CurrentDirection.Reverse();
        }
    }
}