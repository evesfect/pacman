using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class ScatterState : GhostState
    {
        // Notice we omit AiSpawnZone and AiGate. They can never go back!
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
            // The logic runs dynamically in Update
        }

        public override void Update()
        {
            if (GhostBlackboard.MovementController.IsMoving) return;

            Direction nextDirection = GetNextRandomDirection();
            
            if (nextDirection != Direction.None)
            {
                GhostBlackboard.CurrentDirection = nextDirection;
                
                // Using your updated method signature: false for ghost rotation!
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

            // Evaluate all 4 possible directions
            foreach (Direction dir in GameSettings.DirectionsToCheck)
            {
                // Rule: Do not allow going backwards (180 degree turn)
                if (dir == GhostBlackboard.CurrentDirection.Reverse() && GhostBlackboard.CurrentDirection != Direction.None)
                {
                    continue;
                }

                // Rule: Must be a movable cell
                Vector2Int nextCell = currentCell + dir.ToVector2Int();
                if (GhostBlackboard.GridData.IsCellMovable(nextCell, _walkableCells))
                {
                    possibleDirections.Add(dir);
                }
            }

            // If we have valid options (straight or turns), pick one randomly
            if (possibleDirections.Count > 0)
            {
                int randomIndex = Random.Range(0, possibleDirections.Count);
                return possibleDirections[randomIndex];
            }

            // If we have NO valid options, it means we hit a dead end.
            // The only legal move is to break the rule and reverse.
            return GhostBlackboard.CurrentDirection.Reverse();
        }
    }
}